using UnityEngine;

public class RocoMotion
{
    private readonly Rigidbody _rb;
    private readonly Animator _animator;
    private readonly int _speedHash;

    private float _walkSpeed = 0.3f;
    private float _runSpeed = 0.5f;
    private float _minModeDuration = 1.5f;
    private float _maxModeDuration = 3.5f;
    private float _nextSpeedSwitchTime;
    private float _currentHorizontalSpeed;

    public float TurnSpeed { get; set; } = 180f;
    public float RotationTolerance { get; set; } = 0.01f;

    public RocoMotion(Rigidbody rb, Animator animator)
    {
        _rb = rb;
        _animator = animator;
        _speedHash = Animator.StringToHash("Speed");
    }

    public void ConfigureSpeeds(float walkSpeed, float runSpeed, float minDuration, float maxDuration)
    {
        _walkSpeed = walkSpeed;
        _runSpeed = runSpeed;
        _minModeDuration = minDuration;
        _maxModeDuration = maxDuration;
    }

    public void BeginMoveCycle()
    {
        PickNextSpeed();
    }

    public void StopAll()
    {
        _rb.linearVelocity = Vector3.zero;
        _animator?.SetFloat(_speedHash, 0f);
        _currentHorizontalSpeed = 0f;
        _nextSpeedSwitchTime = 0f;
    }

    public void StopHorizontal()
    {
        Vector3 vel = _rb.linearVelocity;
        _rb.linearVelocity = new Vector3(0f, vel.y, 0f);
        _animator?.SetFloat(_speedHash, 0f);
        _currentHorizontalSpeed = 0f;
    }

    public bool RotateTowardsYaw(float targetYaw)
    {
        _animator?.SetFloat(_speedHash, _walkSpeed);

        Transform rbTransform = _rb.transform;
        Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);

        rbTransform.rotation = Quaternion.RotateTowards(
            rbTransform.rotation,
            targetRot,
            TurnSpeed * Time.deltaTime);

        if (Quaternion.Angle(rbTransform.rotation, targetRot) > RotationTolerance)
        {
            Vector3 vel = _rb.linearVelocity;
            _rb.linearVelocity = new Vector3(0f, vel.y, 0f);
            return true;
        }

        rbTransform.rotation = targetRot;
        return false;
    }

    public void MoveTowardsX(float targetX)
    {
        if (Time.time >= _nextSpeedSwitchTime)
        {
            PickNextSpeed();
        }

        float dir = Mathf.Sign(targetX - _rb.position.x);
        Vector3 vel = _rb.linearVelocity;
        _rb.linearVelocity = new Vector3(dir * _currentHorizontalSpeed, vel.y, 0f);
    }

    public void Arrive()
    {
        Vector3 vel = _rb.linearVelocity;
        _rb.linearVelocity = new Vector3(0f, vel.y, 0f);
        _animator?.SetFloat(_speedHash, 0f);
        _currentHorizontalSpeed = 0f;
    }

    private void PickNextSpeed()
    {
        _currentHorizontalSpeed = Random.value < 0.4f ? _runSpeed : _walkSpeed;
        _animator?.SetFloat(_speedHash, _currentHorizontalSpeed);
        _nextSpeedSwitchTime = Time.time + Random.Range(_minModeDuration, _maxModeDuration);
    }
}