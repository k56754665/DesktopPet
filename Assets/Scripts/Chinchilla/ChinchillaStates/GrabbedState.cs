using UnityEngine;

public class GrabbedState : ChinchillaState
{
    private readonly Camera _camera;
    private readonly float _distance = 5f;
    private readonly float _followSpeed = 10f;
    private bool _isGrabbing;
    
    public GrabbedState()
    {
        minDuration = Mathf.Infinity;
        _camera = Camera.main;
    }
    
    public override float EvaluateScore(StateContext context)
    {
        return _isGrabbing ? Mathf.Infinity : 0;
    }
    
    public override void Enter(StateContext context)
    {
        base.Enter(context);
        Debug.Log("GrabbedState Enter");
        _isGrabbing = true;
    }

    public override void Update(StateContext context)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _distance;
        Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);

        context.Rb.MovePosition(Vector3.Lerp(context.Rb.position, worldPos, Time.deltaTime * _followSpeed));
    }

    public override void Exit(StateContext context)
    {
        _isGrabbing = false;
    }
}
