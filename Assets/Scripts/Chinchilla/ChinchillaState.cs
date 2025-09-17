using UnityEngine;

public abstract class ChinchillaState
{
    protected float minDuration = 0f;
    protected float enterTime;

    public virtual void Enter(StateContext context) { enterTime = Time.time; }
    public virtual void Update(StateContext context) { }
    public virtual void Exit(StateContext context) { }

    /// <summary>
    /// 현재 상태를 끝내고 다른 상태로 전환할 수 있는지를 반환하는 공통 함수
    /// </summary>
    public virtual bool CanExit()
    {
        if (Time.time - enterTime < minDuration)
            return false;
        return true;
    }
}

/// <summary>
/// 유틸리티 기반으로 점수를 평가해 자동 전환되는 상태
/// </summary>
public abstract class EvaluatableState : ChinchillaState
{
    public abstract float EvaluateScore(StateContext context);
}

/// <summary>
/// 외부 이벤트로 강제로 진입하며 내부 로직이 완료되면 스스로 다음 상태를 호출하는 상태
/// </summary>
public abstract class ForcedState : ChinchillaState
{
    public override bool CanExit()
    {
        // 강제 상태는 스스로 전환 시점을 결정하므로 최소 지속 시간과 무관하게 언제든 종료 가능하다.
        return true;
    }
}