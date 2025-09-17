using UnityEngine;

public class GrabbedState : ForcedState
{
    private static readonly int Grabbed = Animator.StringToHash("Grabbed");
    private readonly float _defaultDistance = 5f;
    private readonly float _followSpeed = 10f;

    private bool _isGrabbing;
    private bool _hasPointer;
    private float _dragDistance;
    private PointerInfo _pointerInfo;

    public GrabbedState()
    {
        _dragDistance = _defaultDistance;
    }

    public float DefaultDistance => _defaultDistance;

    public override void Enter(StateContext context)
    {
        Debug.Log("Enter Grabbed State");
        base.Enter(context);
        context.Ani.SetBool(Grabbed, true);
        context.Rb.useGravity = false;
        _hasPointer = false;
    }

    public override void Update(StateContext context)
    {
        if (!_isGrabbing) 
        {
            // 놓였으면 Falling으로 전환
            var fallingState = ChinchillaStateFactory.Get<FallingState>();
            context.RequestStateChange?.Invoke(fallingState);
            return;
        }

        if (!_hasPointer)
            return;

        Vector3 targetPosition = _pointerInfo.HasHit 
            ? _pointerInfo.Hit.point 
            : _pointerInfo.GetWorldPoint(_dragDistance);

        // GrabPoint 보정
        Vector3 offset = context.GrabPoint.position - context.Rb.position;
        Vector3 correctedTarget = targetPosition - offset;

        context.Rb.MovePosition(Vector3.Lerp(context.Rb.position, correctedTarget, Time.deltaTime * _followSpeed));
    }

    public override void Exit(StateContext context)
    {
        context.Rb.useGravity = true;
        _isGrabbing = false;
        context.Ani.SetBool(Grabbed, _isGrabbing);
        _hasPointer = false;
        _dragDistance = _defaultDistance;
    }

    public void SetGrabbing(bool isGrabbing)
    {
        if (_isGrabbing == isGrabbing)
            return;

        _isGrabbing = isGrabbing;

        if (!isGrabbing)
            _hasPointer = false;
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