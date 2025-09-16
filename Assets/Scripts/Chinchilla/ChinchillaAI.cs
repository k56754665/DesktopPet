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
            Bounds = new MonitorBounds(),
            Hunger = 0,
            Tiredness = 0,
            Sleepy = 0,
        };
        
        _states = new List<ChinchillaState>
        {
            new IdleState(),
            new WanderState(),
        };
    }

    private void Update()
    {
        // 점수 평가
        ChinchillaState best = _states.OrderByDescending(a => a.EvaluteScore(_context)).First();
        
        // 상태 전환 조건 확인
        if (best != _currentState && (_currentState == null || _currentState.CanExit()))
        {
            _currentState?.Exit(_context);
            _currentState = best;
            _currentState?.Enter(_context);
        }
        
        // 현재 상태 실행
        _currentState?.Update(_context);
    }
}
