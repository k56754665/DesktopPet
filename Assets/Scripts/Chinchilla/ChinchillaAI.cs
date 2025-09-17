using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChinchillaAI : MonoBehaviour, IDraggable
{
    private StateContext _context;
    private ChinchillaState _currentState;
    private List<ChinchillaState> _states;
    private Animator _ani;
    private Rigidbody _rb;
    private bool _isBeingDragged;
    
    private void Start()
    {
        _ani = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
        
        _context = new StateContext
        {
            Ani = _ani,
            Rb = _rb,
            Bounds = MonitorUtil.GetBounds(Camera.main),
            Motion = new RocoMotion(_rb, _ani),
            RequestStateChange = ChangeState,
            Hunger = 0,
            Tiredness = 0,
            Sleepy = 0,
        };
        
        _states = new List<ChinchillaState>
        {
            ChinchillaStateFactory.Get<IdleState>(),
            ChinchillaStateFactory.Get<WanderState>(),
        };
    }

    private void Update()
    {
        UpdateStates();
    }

    private void UpdateStates()
    {
        ChinchillaState best = _states.OrderByDescending(a => a.EvaluateScore(_context)).First();
        ChangeState(best);
        _currentState?.Update(_context);
    }

    public void ChangeState(ChinchillaState state)
    {
        if (state == _currentState || (_currentState != null && !_currentState.CanExit())) return;

        _currentState?.Exit(_context);
        _currentState = state;
        _currentState?.Enter(_context);
    }

    public void OnDragStart(PointerInfo pointerInfo)
    {
        var grabbstate = ChinchillaStateFactory.Get<GrabbedState>();

        _isBeingDragged = true;

        float dragDistance = pointerInfo.HasHit && !float.IsInfinity(pointerInfo.HitDistance)
            ? pointerInfo.HitDistance
            : grabbstate.DefaultDistance;

        grabbstate.SetDragDistance(dragDistance);
        grabbstate.SetGrabbing(true);
        ChangeState(grabbstate);
        grabbstate.UpdatePointer(pointerInfo);
    }

    public void OnDrag(PointerInfo pointerInfo)
    {
        if (!_isBeingDragged)
            return;

        ChinchillaStateFactory.Get<GrabbedState>().UpdatePointer(pointerInfo);
    }

    public void OnDragEnd(PointerInfo pointerInfo)
    {
        if (!_isBeingDragged)
            return;
        
        var grabbedState = ChinchillaStateFactory.Get<GrabbedState>();

        Debug.Log("OnDragEnd");
        grabbedState.UpdatePointer(pointerInfo);
        grabbedState.SetGrabbing(false);
        _isBeingDragged = false;
        ChangeState(ChinchillaStateFactory.Get<FallingState>());
    }
}