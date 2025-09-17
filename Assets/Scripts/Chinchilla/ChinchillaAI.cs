using System.Collections.Generic;
using UnityEngine;

public class ChinchillaAI : MonoBehaviour, IDraggable
{
    private StateContext _context;
    private ChinchillaState _currentState;
    private List<EvaluatableState> _states;
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
        
        _states = new List<EvaluatableState>
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
        if (_currentState is ForcedState)
        {
            _currentState.Update(_context);
            return;
        }

        EvaluatableState best = null;
        float bestScore = float.MinValue;

        foreach (var state in _states)
        {
            if (state == null)
                continue;

            float score = state.EvaluateScore(_context);
            if (score > bestScore)
            {
                bestScore = score;
                best = state;
            }
        }

        if (best != null)
        {
            ChangeState(best);
        }

        _currentState?.Update(_context);
    }

    public void ChangeState(ChinchillaState state)
    {
        if (state == null || state == _currentState)
            return;

        bool isTargetForced = state is ForcedState;

        if (!isTargetForced && _currentState != null && !_currentState.CanExit())
            return;

        _currentState?.Exit(_context);
        _currentState = state;
        _currentState?.Enter(_context);
    }

    public void OnDragStart(PointerInfo pointerInfo)
    {
        var grabState = ChinchillaStateFactory.Get<GrabbedState>();

        _isBeingDragged = true;

        float dragDistance = pointerInfo.HasHit && !float.IsInfinity(pointerInfo.HitDistance)
            ? pointerInfo.HitDistance
            : grabState.DefaultDistance;

        grabState.SetDragDistance(dragDistance);
        grabState.SetGrabbing(true);
        ChangeState(grabState);
        grabState.UpdatePointer(pointerInfo);
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
    }
}