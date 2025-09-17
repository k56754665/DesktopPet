using UnityEngine;

public class GrabbedState : ForcedState
{
    private static readonly int Grabbed = Animator.StringToHash("Grabbed");
    private readonly float _defaultDistance = 5f;
    private readonly float _followSpeed = 10f;
    private StateContext _context;

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
        context.Ani.SetBool(Grabbed, _isGrabbing);
        context.Rb.useGravity = false;
        _hasPointer = false;
        _context = context;
    }

    public override void Update(StateContext context)
    {
        if (!_hasPointer)
            return;

        Vector3 targetPosition = _pointerInfo.HasHit ? _pointerInfo.Hit.point : _pointerInfo.GetWorldPoint(_dragDistance);

        context.Rb.MovePosition(Vector3.Lerp(context.Rb.position, targetPosition, Time.deltaTime * _followSpeed));
    }

    public override void Exit(StateContext context)
    {
        context.Rb.useGravity = true;
        _isGrabbing = false;
        context.Ani.SetBool(Grabbed, _isGrabbing);
        _hasPointer = false;
        _dragDistance = _defaultDistance;
        _context = null;
    }

    public void SetGrabbing(bool isGrabbing)
    {
        if (_isGrabbing == isGrabbing)
            return;

        _isGrabbing = isGrabbing;

        if (isGrabbing) return;
        
        _hasPointer = false;
        if (_context != null)
        {
            var fallingState = ChinchillaStateFactory.Get<FallingState>();
            _context.RequestStateChange?.Invoke(fallingState);
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