using UnityEngine;

public class WanderState : ChinchillaState
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private Vector3 _targetPos;
    private float _currentSpeed;
    private float _targetYaw;
    private float _turnSpeed = 180f;
    private bool _isMoving;
    private bool _isRotating;

    private const float WalkSpeed = 0.3f;
    private const float RunSpeed = 0.5f;
    private const float MinModeDuration = 1.5f;
    private const float MaxModeDuration = 3.5f;
    private float _nextSpeedSwitchTime;

    public WanderState()
    {
        minDuration = 2f;
    }

    public override float EvaluateScore(StateContext context)
    {
        return _isMoving ? 2f : Random.Range(0.1f, 0.5f);
    }

    public override void Enter(StateContext context)
    {
        base.Enter(context);
        Debug.Log("WanderState Enter");

        MonitorBounds bounds = context.Bounds;
        float randX = Random.Range(bounds.Left, bounds.Right);
        _targetPos = new Vector3(randX, context.Rb.position.y, context.Rb.position.z);

        _targetYaw = randX < context.Rb.position.x ? 90f : 270f;
        _isRotating = true;
        _isMoving = true;

        context.Rb.linearVelocity = Vector3.zero;
        ChooseNextSpeed(context);
    }

    public override void Update(StateContext context)
    {
        if (!_isMoving) return;
        if (_isRotating)
        {
            if (Rotate(context)) return;
        }
        Move(context);
    }

    public override void Exit(StateContext context)
    {
        context.Rb.linearVelocity = Vector3.zero;
        context.Ani?.SetFloat(Speed, 0f);
        _isMoving = false;
        _isRotating = false;
        _currentSpeed = 0f;
    }

    private bool Rotate(StateContext context)
    {
        // 회전 중일 때는 항상 WalkSpeed 적용
        context.Ani?.SetFloat(Speed, WalkSpeed);

        Transform rbTransform = context.Rb.transform;
        Quaternion targetRot = Quaternion.Euler(0f, _targetYaw, 0f);

        rbTransform.rotation = Quaternion.RotateTowards(
            rbTransform.rotation, targetRot, _turnSpeed * Time.deltaTime);

        if (Quaternion.Angle(rbTransform.rotation, targetRot) > 0.01f)
        {
            context.Rb.linearVelocity = new Vector3(0f, context.Rb.linearVelocity.y, 0f);
            return true; // 아직 회전 중
        }

        rbTransform.rotation = targetRot;
        _isRotating = false;
        return false;
    }

    private void Move(StateContext context)
    {
        float distance = Mathf.Abs(_targetPos.x - context.Rb.position.x);
        if (distance > 0.05f) MoveTowardsTarget(context);
        else Arrive(context);
    }

    private void MoveTowardsTarget(StateContext context)
    {
        if (Time.time >= _nextSpeedSwitchTime)
            ChooseNextSpeed(context);

        float dir = Mathf.Sign(_targetPos.x - context.Rb.position.x);
        Vector3 vel = context.Rb.linearVelocity;
        context.Rb.linearVelocity = new Vector3(dir * _currentSpeed, vel.y, 0f);
    }

    private void Arrive(StateContext context)
    {
        Vector3 vel = context.Rb.linearVelocity;
        context.Rb.linearVelocity = new Vector3(0f, vel.y, 0f);
        context.Ani?.SetFloat(Speed, 0f);
        _isMoving = false;
        _currentSpeed = 0f;
    }

    private void ChooseNextSpeed(StateContext context)
    {
        _currentSpeed = Random.value < 0.4f ? RunSpeed : WalkSpeed;
        context.Ani?.SetFloat(Speed, _currentSpeed);
        _nextSpeedSwitchTime = Time.time + Random.Range(MinModeDuration, MaxModeDuration);
    }
}
