using UnityEngine;

/// <summary>
/// 마우스 포인터 정보.
/// </summary>
public readonly struct PointerInfo
{
    public PointerInfo(Vector2 screenPosition, Ray ray, bool hasHit, RaycastHit hit)
    {
        ScreenPosition = screenPosition;
        Ray = ray;
        HasHit = hasHit;
        Hit = hit;
    }

    /// <summary>
    /// 화면 좌표.
    /// </summary>
    public Vector2 ScreenPosition { get; }

    /// <summary>
    /// 화면 좌표를 기준으로 한 레이.
    /// </summary>
    public Ray Ray { get; }

    /// <summary>
    /// 레이캐스트가 어떤 오브젝트에 닿았는지 여부.
    /// </summary>
    public bool HasHit { get; }

    /// <summary>
    /// 레이캐스트 히트 정보. <see cref="HasHit"/> 이 true일 때만 유효.
    /// </summary>
    public RaycastHit Hit { get; }

    /// <summary>
    /// 맞은 오브젝트. <see cref="HasHit"/> 이 false이면 null.
    /// </summary>
    public GameObject HitObject => HasHit ? Hit.collider.gameObject : null;

    /// <summary>
    /// 카메라로부터 맞은 지점까지의 거리. 맞은 지점이 없으면 무한대.
    /// </summary>
    public float HitDistance => HasHit ? Hit.distance : float.PositiveInfinity;

    /// <summary>
    /// 레이 위의 거리에 해당하는 월드 좌표.
    /// </summary>
    public Vector3 GetWorldPoint(float distance)
    {
        return Ray.GetPoint(distance);
    }
}

/// <summary>
/// 단순 클릭을 처리하는 인터페이스.
/// </summary>
public interface IClickable
{
    void OnClick(PointerInfo pointerInfo);
}

/// <summary>
/// 드래그 가능한 오브젝트 인터페이스.
/// </summary>
public interface IDraggable
{
    void OnDragStart(PointerInfo pointerInfo);
    void OnDrag(PointerInfo pointerInfo);
    void OnDragEnd(PointerInfo pointerInfo);
}

/// <summary>
/// 드롭 타겟 인터페이스.
/// </summary>
public interface IDropTarget
{
    void OnDrop(IDraggable draggable, PointerInfo pointerInfo);
}