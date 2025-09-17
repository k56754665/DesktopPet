using System;
using System.Collections.Generic;
using UnityEngine;

public static class ChinchillaStateFactory
{
    private static readonly Dictionary<Type, ChinchillaState> _instances = new();
    private static bool _initialized;

    /// <summary>
    /// 최초 초기화: 모든 상태를 여기서 생성해둠
    /// </summary>
    private static void Initialize()
    {
        if (_initialized) return;

        // 이 부분에 프로젝트 내 모든 상태를 new 해서 등록
        _instances[typeof(IdleState)] = new IdleState();
        _instances[typeof(WanderState)] = new WanderState();
        _instances[typeof(GrabbedState)] = new GrabbedState();
        _instances[typeof(FallingState)] = new FallingState();

        _initialized = true;
        Debug.Log("[StateFactory] All states initialized.");
    }

    /// <summary>
    /// 타입 기반으로 상태 가져오기
    /// </summary>
    public static T Get<T>() where T : ChinchillaState
    {
        Initialize();

        if (_instances.TryGetValue(typeof(T), out var state))
            return (T)state;

        Debug.LogError($"[StateFactory] State of type {typeof(T).Name} not found!");
        return null;
    }
}