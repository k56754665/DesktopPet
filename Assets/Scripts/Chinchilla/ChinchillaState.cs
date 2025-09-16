using UnityEngine;

public abstract class ChinchillaState
{
    protected bool isInterruptible = true;
    protected float minDuration = 0f;
    protected float enterTime;
    
    public abstract float EvaluteScore(StateContext context);

    public virtual void Enter(StateContext context) { enterTime = Time.time; }
    public virtual void Update(StateContext context) { }
    public virtual void Exit(StateContext context) { }

    /// <summary>
    /// 현재 상태를 끝내고 다른 상태로 전환할 수 있는지를 반환하는 공통 함수
    /// </summary>
    public bool CanExit()
    {
        if (Time.time - enterTime < minDuration)
            return false;
        return true;
    }
}
