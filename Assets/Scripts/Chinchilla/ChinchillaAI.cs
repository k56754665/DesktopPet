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
    private GrabbedState _grabbedState;
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
            Hunger = 0,
            Tiredness = 0,
            Sleepy = 0,
        };
        
        _states = new List<ChinchillaState>
        {
            new IdleState(),
            new WanderState(),
            new GrabbedState(),
        };

        _grabbedState = _states.OfType<GrabbedState>().FirstOrDefault();
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
        if (_grabbedState == null)
            return;

        _isBeingDragged = true;

        float dragDistance = pointerInfo.HasHit && !float.IsInfinity(pointerInfo.HitDistance)
            ? pointerInfo.HitDistance
            : _grabbedState.DefaultDistance;

        _grabbedState.SetDragDistance(dragDistance);
        _grabbedState.SetGrabbing(true);
        ChangeState(_grabbedState);
        _grabbedState.UpdatePointer(pointerInfo);
    }

    public void OnDrag(PointerInfo pointerInfo)
    {
        if (!_isBeingDragged || _grabbedState == null)
            return;

        _grabbedState.UpdatePointer(pointerInfo);
    }

    public void OnDragEnd(PointerInfo pointerInfo)
    {
        if (!_isBeingDragged || _grabbedState == null)
            return;

        _grabbedState.UpdatePointer(pointerInfo);
        _grabbedState.SetGrabbing(false);
        _isBeingDragged = false;
    }
}