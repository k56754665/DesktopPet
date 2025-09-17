using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 마우스 입력을 단일 진입점으로 처리하는 매니저.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private LayerMask _interactionLayer = ~0;
    [SerializeField] private float _rayDistance = 200f;

    private Camera _camera;
    private InputSystem_Actions _actions;
    private PointerInfo _pointerInfo;
    private IDraggable _activeDraggable;
    private IClickable _pendingClickable;
    private GameObject _pendingClickObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("중복된 InputManager 인스턴스가 감지되었습니다. 기존 인스턴스를 유지합니다.");
            enabled = false;
            return;
        }

        Instance = this;
        _camera = Camera.main;
        _actions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        if (_actions == null)
            _actions = new InputSystem_Actions();

        _actions.Player.Enable();
        _actions.Player.Click.started += OnPointerDown;
        _actions.Player.Click.canceled += OnPointerUp;
    }

    private void OnDisable()
    {
        if (_actions == null)
            return;

        _actions.Player.Click.started -= OnPointerDown;
        _actions.Player.Click.canceled -= OnPointerUp;
        _actions.Player.Disable();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        _actions?.Dispose();
    }

    private void Update()
    {
        UpdatePointerInfo();

        if (_activeDraggable != null)
        {
            _activeDraggable.OnDrag(_pointerInfo);
        }
    }

    private void OnPointerDown(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        UpdatePointerInfo();

        if (!_pointerInfo.HasHit)
        {
            _pendingClickable = null;
            _pendingClickObject = null;
            return;
        }

        var targetCollider = _pointerInfo.Hit.collider;
        if (targetCollider == null)
            return;

        var draggable = targetCollider.GetComponentInParent<IDraggable>();
        if (draggable != null)
        {
            _activeDraggable = draggable;
            _activeDraggable.OnDragStart(_pointerInfo);
            _pendingClickable = null;
            _pendingClickObject = null;
            return;
        }

        var clickable = targetCollider.GetComponentInParent<IClickable>();
        if (clickable != null)
        {
            _pendingClickable = clickable;
            _pendingClickObject = _pointerInfo.HitObject;
        }
        else
        {
            _pendingClickable = null;
            _pendingClickObject = null;
        }
    }

    private void OnPointerUp(InputAction.CallbackContext ctx)
    {
        UpdatePointerInfo();

        if (_activeDraggable != null)
        {
            var draggable = _activeDraggable;
            _activeDraggable = null;
            draggable.OnDragEnd(_pointerInfo);

            if (_pointerInfo.HasHit)
            {
                var dropTarget = _pointerInfo.Hit.collider.GetComponentInParent<IDropTarget>();
                dropTarget?.OnDrop(draggable, _pointerInfo);
            }

            return;
        }

        if (_pendingClickable != null)
        {
            if (_pointerInfo.HasHit && _pointerInfo.HitObject == _pendingClickObject)
            {
                _pendingClickable.OnClick(_pointerInfo);
            }

            _pendingClickable = null;
            _pendingClickObject = null;
        }
    }

    private void UpdatePointerInfo()
    {
        if (_actions == null)
        {
            _pointerInfo = default;
            return;
        }

        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                _pointerInfo = default;
                return;
            }
        }

        Vector2 screenPos = _actions.Player.Point.ReadValue<Vector2>();
        Ray ray = _camera.ScreenPointToRay(screenPos);
        RaycastHit[] hits = Physics.RaycastAll(ray, _rayDistance, _interactionLayer, QueryTriggerInteraction.Ignore);

        RaycastHit closestHit = default;
        bool hasHit = false;

        if (hits.Length > 0)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            closestHit = hits[0];
            hasHit = true;
        }

        _pointerInfo = new PointerInfo(screenPos, ray, hasHit, closestHit);
    }

    /// <summary>
    /// 현재 포인터 정보를 반환한다.
    /// </summary>
    public PointerInfo CurrentPointerInfo => _pointerInfo;
}