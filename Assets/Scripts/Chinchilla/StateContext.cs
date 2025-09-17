using UnityEngine;
using System;

public class StateContext
{
    public Animator Ani;
    public Rigidbody Rb;
    public MonitorBounds Bounds;
    public RocoMotion Motion;
    public Action<ChinchillaState> RequestStateChange;
    public Transform GrabPoint;
    public float Hunger;
    public float Tiredness;
    public float Sleepy;
}
