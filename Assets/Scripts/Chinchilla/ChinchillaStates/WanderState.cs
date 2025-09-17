using UnityEngine;

public class WanderState : ChinchillaState
{
    private Vector3 _targetPos;
    private float _targetYaw;
    private bool _isMoving;
    private bool _isRotating;

    private const float WalkSpeed = 0.3f;
    private const float RunSpeed = 0.5f;
    private const float MinModeDuration = 1.5f;
    private const float MaxModeDuration = 3.5f;
    private const float TurnSpeed = 180f;

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

        context.Motion.TurnSpeed = TurnSpeed;
        context.Motion.ConfigureSpeeds(WalkSpeed, RunSpeed, MinModeDuration, MaxModeDuration);
        context.Motion.StopAll();
        context.Motion.BeginMoveCycle();
    }

    public override void Update(StateContext context)
    {
        if (!_isMoving) return;
        if (_isRotating)
        {
            if (context.Motion.RotateTowardsYaw(_targetYaw)) return;
            _isRotating = false;
        }
        Move(context);
    }

    public override void Exit(StateContext context)
    {
        context.Motion.StopAll();
        _isMoving = false;
        _isRotating = false;
    }

    private void Move(StateContext context)
    {
        float distance = Mathf.Abs(_targetPos.x - context.Rb.position.x);
        if (distance > 0.05f)
        {
            context.Motion.MoveTowardsX(_targetPos.x);
        }
        else
        {
            context.Motion.Arrive();
            _isMoving = false;
        }
    }
}