using UnityEngine;

public class WanderState : ChinchillaState
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private Vector3 _targetPos;
    private float _moveSpeed = 1f;
    private bool _isMoving = false;
    
    public WanderState()
    {
        isInterruptible = true;
        minDuration = 2f;
    }
    
    public override float EvaluteScore(StateContext context)
    {
        return _isMoving ? 2f : Random.Range(0.1f, 0.5f);
    }

    public override void Enter(StateContext context)
    {
        base.Enter(context);
        Debug.Log("WanderState Enter");
        context.Ani?.SetFloat(Speed, _moveSpeed);

        MonitorBounds bounds = context.Bounds;

        // X축 랜덤 목적지 (y,z는 현 위치 유지)
        float randX = Random.Range(bounds.Left, bounds.Right);
        _targetPos = new Vector3(randX, context.Rb.transform.position.y, context.Rb.transform.position.z);

        // 방향 설정 (왼쪽/오른쪽)
        if (randX < context.Rb.transform.position.x)
            context.Rb.transform.rotation = Quaternion.Euler(0, 90f, 0); // 왼쪽
        else
            context.Rb.transform.rotation = Quaternion.Euler(0, 270f, 0);   // 오른쪽

        _isMoving = true;
    }

    public override void Update(StateContext context)
    {
        if (!_isMoving) return;

        Vector3 currentPos = context.Rb.position;
        float distance = Mathf.Abs(_targetPos.x - currentPos.x);

        if (distance > 0.05f)
        {
            // X축으로만 이동
            float dir = Mathf.Sign(_targetPos.x - currentPos.x);
            context.Rb.linearVelocity = new Vector3(dir * _moveSpeed, context.Rb.linearVelocity.y, 0f);
        }
        else
        {
            // 목적지 도착
            context.Rb.linearVelocity = Vector3.zero;
            _isMoving = false;
        }
    }

    public override void Exit(StateContext context)
    {
        context.Rb.linearVelocity = Vector3.zero;
        context.Ani?.SetFloat(Speed, 0f);
        _isMoving = false;
    }
}
