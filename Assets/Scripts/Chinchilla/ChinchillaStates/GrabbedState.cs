using UnityEngine;

public class GrabbedState : ChinchillaState
{
    private readonly float _defaultDistance = 5f;
    private readonly float _followSpeed = 10f;

    private bool _isGrabbing;
    private bool _hasPointer;
    private float _dragDistance;
    private PointerInfo _pointerInfo;

    public GrabbedState()
    {
        minDuration = Mathf.Infinity;
        _dragDistance = _defaultDistance;
    }

    public float DefaultDistance => _defaultDistance;

    public override float EvaluateScore(StateContext context)
    {
        return _isGrabbing ? Mathf.Infinity : 0f;
    }

    public override void Enter(StateContext context)
    {
        Debug.Log("Enter Grabbed State");
        base.Enter(context);
        context.Rb.useGravity = false;
        _hasPointer = false;
    }

    public override void Update(StateContext context)
    {
        if (!_hasPointer)
            return;

        Vector3 targetPosition;

        if (_pointerInfo.HasHit)
        {
            targetPosition = _pointerInfo.Hit.point;
        }
        else
        {
            targetPosition = _pointerInfo.GetWorldPoint(_dragDistance);
        }

        context.Rb.MovePosition(Vector3.Lerp(context.Rb.position, targetPosition, Time.deltaTime * _followSpeed));
    }

    public override void Exit(StateContext context)
    {
        context.Rb.useGravity = true;
        _isGrabbing = false;
        _hasPointer = false;
        _dragDistance = _defaultDistance;
    }

    public void SetGrabbing(bool isGrabbing)
    {
        _isGrabbing = isGrabbing;
        if (!isGrabbing)
        {
            _hasPointer = false;
        }
    }

    public void UpdatePointer(PointerInfo pointerInfo)
    {
        _pointerInfo = pointerInfo;
        _hasPointer = true;
    }

    public void SetDragDistance(float distance)
    {
        if (float.IsInfinity(distance) || float.IsNaN(distance))
        {
            _dragDistance = _defaultDistance;
        }
        else
        {
            _dragDistance = Mathf.Max(0.1f, distance);
        }
    }
}