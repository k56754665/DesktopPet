using UnityEngine;

public class WanderState : ChinchillaState
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private Vector3 _targetPos;
    private float _moveSpeed = 0.5f;
    private float _targetYaw;
    private float _turnSpeed = 180f;
    private bool _isMoving;
    private bool _isRotating;
    
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

        MonitorBounds bounds = context.Bounds;

        // X축 랜덤 목적지 (y,z는 현 위치 유지)
        float randX = Random.Range(bounds.Left, bounds.Right);
        _targetPos = new Vector3(randX, context.Rb.transform.position.y, context.Rb.transform.position.z);

        // 방향 설정 (왼쪽/오른쪽)
        _targetYaw = randX < context.Rb.transform.position.x ? 90f : 270f;
        _isRotating = true;
        context.Rb.linearVelocity = Vector3.zero;
        context.Ani?.SetFloat(Speed, _moveSpeed);

        _isMoving = true;
    }

    public override void Update(StateContext context)
    {
        if (!_isMoving) return;

        Transform rbTransform = context.Rb.transform;

        if (_isRotating)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, _targetYaw, 0f);
            rbTransform.rotation = Quaternion.RotateTowards(rbTransform.rotation, targetRotation, _turnSpeed * Time.deltaTime);

            if (Quaternion.Angle(rbTransform.rotation, targetRotation) <= 0.1f)
            {
                rbTransform.rotation = targetRotation;
                _isRotating = false;
            }
            else
            {
                context.Rb.linearVelocity = new Vector3(0f, context.Rb.linearVelocity.y, 0f);
                return;
            }
        }

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
            context.Ani?.SetFloat(Speed, 0f);
            _isMoving = false;
        }
    }

    public override void Exit(StateContext context)
    {
        context.Rb.linearVelocity = Vector3.zero;
        context.Ani?.SetFloat(Speed, 0f);
        _isMoving = false;
        _isRotating = false;
    }
}