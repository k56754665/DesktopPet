using UnityEngine;

public class IdleState : ChinchillaState
{
    public IdleState()
    {
        isInterruptible = true;
        minDuration = 2f;
    }
    
    public override float EvaluteScore(StateContext context)
    {
        return Random.value;
    }

    public override void Enter(StateContext context)
    {
        base.Enter(context);
        Debug.Log("IdleState Enter");
        context.Ani.Play("Idle");
        context.Rb.linearVelocity = Vector3.zero;
    }

    public override void Update(StateContext context)
    {
        
    }

    public override void Exit(StateContext context)
    {
        
    }
}
