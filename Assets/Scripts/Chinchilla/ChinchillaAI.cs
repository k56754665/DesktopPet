using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChinchillaAI : MonoBehaviour
{
    private StateContext _context;
    private ChinchillaState _currentState;
    private List<ChinchillaState> _states;
    private Animator _ani;
    private Rigidbody _rb;
    
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
}
