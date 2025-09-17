using UnityEngine;

public class FallingState : ForcedState
{
    private const float GroundCheckDelay = 0.05f;
    private const float GroundCheckPadding = 0.05f;
    private const float LandingVelocityThreshold = 0.5f; // 착지 인정할 y속도 임계치

    private Collider _collider;
    private bool _isFalling;

    public override void Enter(StateContext context)
    {
        base.Enter(context);
        Debug.Log("Falling Enter");

        _collider ??= context.Rb.GetComponent<Collider>();
        _isFalling = true;
        
        context.Motion?.StopHorizontal();
    }

    public override void Update(StateContext context)
    {
        if (!_isFalling)
            return;

        if (Time.time - enterTime < GroundCheckDelay)
            return;

        if (context.Rb.linearVelocity.y < -LandingVelocityThreshold)
            return;

        if (CheckGround(context))
        {
            Land(context);
        }
    }

    public override void Exit(StateContext context)
    {
        _isFalling = false;
    }

    private bool CheckGround(StateContext context)
    {
        float castDistance = GroundCheckPadding + (_collider != null ? _collider.bounds.extents.y : 0f);

        Vector3 origin = context.Rb.position + Vector3.up * GroundCheckPadding;

        // Ground 전용 레이어만 감지 (필요시 레이어마스크 교체)
        int groundMask = LayerMask.GetMask("Ground");
        if (groundMask == 0) groundMask = Physics.DefaultRaycastLayers;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, castDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.rigidbody != context.Rb)
            {
                return true;
            }
        }

        // Bounds 보조 체크
        float bottom = context.Bounds.Bottom;
        if (context.Rb.position.y <= bottom + GroundCheckPadding)
        {
            return true;
        }

        return false;
    }

    private void Land(StateContext context)
    {
        if (!_isFalling)
            return;

        _isFalling = false;

        // 위로 튀는 속도만 제거
        if (context.Rb.linearVelocity.y > 0f)
        {
            Vector3 vel = context.Rb.linearVelocity;
            vel.y = 0f;
            context.Rb.linearVelocity = vel;
        }
        
        context.RequestStateChange?.Invoke(ChinchillaStateFactory.Get<IdleState>());
    }
}
