using UnityEngine;

public class FallingState : ChinchillaState
{
    public override float EvaluateScore(StateContext context)
    {
        return 1;
    }
    
    public override void Enter(StateContext context)
    {
        base.Enter(context);
        Debug.Log("Falling Enter");
        context.Rb.linearVelocity = Vector3.zero;
    }

    public override void Update(StateContext context)
    {
        
    }

    public override void Exit(StateContext context)
    {
        
    }
}
