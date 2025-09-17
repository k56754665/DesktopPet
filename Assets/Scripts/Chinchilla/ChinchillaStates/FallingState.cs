using UnityEngine;

public class FallingState : ChinchillaState
{
    private const float GroundCheckDelay = 0.05f;
    private const float GroundCheckPadding = 0.05f;
    private const float LandingVelocityThreshold = 0.1f;

    private Collider _collider;
    private bool _isFalling;

    public override float EvaluateScore(StateContext context)
    {
        return _isFalling ? float.MaxValue : 0f;
    }

    public override void Enter(StateContext context)
    {
        base.Enter(context);
        Debug.Log("Falling Enter");

        _collider ??= context.Rb.GetComponent<Collider>();
        _isFalling = true;
        
        context.Motion?.StopHorizontal();
        context.Rb.linearVelocity = Vector3.zero;
    }

    public override void Update(StateContext context)
    {
        if (!_isFalling)
            return;

        if (Time.time - enterTime < GroundCheckDelay)
            return;

        if (Mathf.Abs(context.Rb.linearVelocity.y) > LandingVelocityThreshold)
            return;

        if (TryGetGroundPoint(context, out Vector3 groundPoint))
        {
            Land(context, groundPoint);
        }
    }

    public override void Exit(StateContext context)
    {
        _isFalling = false;
    }

    private bool TryGetGroundPoint(StateContext context, out Vector3 groundPoint)
    {
        float castDistance = GroundCheckPadding;

        if (_collider != null)
        {
            castDistance += _collider.bounds.extents.y;
        }

        Vector3 origin = context.Rb.position + Vector3.up * GroundCheckPadding;
        RaycastHit[] hits = Physics.RaycastAll(
            origin,
            Vector3.down,
            castDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in hits)
        {
            if (hit.rigidbody == context.Rb)
                continue;

            groundPoint = hit.point;
            return true;
        }

        float bottom = context.Bounds.Bottom;
        if (context.Rb.position.y <= bottom + GroundCheckPadding)
        {
            groundPoint = new Vector3(context.Rb.position.x, bottom, context.Rb.position.z);
            return true;
        }

        groundPoint = Vector3.zero;
        return false;
    }

    private void Land(StateContext context, Vector3 groundPoint)
    {
        if (!_isFalling)
            return;

        _isFalling = false;
        context.Rb.linearVelocity = Vector3.zero;

        if (groundPoint != Vector3.zero)
        {
            Vector3 targetPos = context.Rb.position;

            if (_collider != null)
            {
                targetPos.y = groundPoint.y + _collider.bounds.extents.y;
            }
            else
            {
                targetPos.y = groundPoint.y;
            }

            context.Rb.MovePosition(targetPos);
        }

        context.RequestStateChange?.Invoke(ChinchillaStateFactory.Get<IdleState>());
    }
}