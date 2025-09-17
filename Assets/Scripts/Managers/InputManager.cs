using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private LayerMask _chinchillaLayer;

    private InputSystem_Actions _actions;
    private GameObject _grabbedChinchilla;

    private void Awake()
    {
        _camera = Camera.main;
        _actions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        Debug.Log("InputManager: OnEnable");
        _actions.Player.Enable();
        _actions.Player.Click.started += OnClickStarted;
        _actions.Player.Click.canceled += OnClickCanceled;
    }

    private void OnDisable()
    {
        _actions.Player.Click.started -= OnClickStarted;
        _actions.Player.Click.canceled -= OnClickCanceled;
        _actions.Player.Disable();
    }

    private void OnClickStarted(InputAction.CallbackContext ctx)
    {
        Debug.Log("InputManager: OnClickStarted");
        
        // 1. UI 위인지 확인
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // 2. Raycast으로 친칠라 체크
        Vector2 screenPos = _actions.Player.Point.ReadValue<Vector2>();
        Ray ray = _camera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _chinchillaLayer))
        {
            _grabbedChinchilla = hit.collider.gameObject;
            Debug.Log("Chinchilla grabbed: " + _grabbedChinchilla.name);

            // 상태머신 전환
            var sm = _grabbedChinchilla.GetComponent<ChinchillaAI>();
            sm?.ChangeState(new GrabbedState());
        }
    }

    private void OnClickCanceled(InputAction.CallbackContext ctx)
    {
        if (_grabbedChinchilla != null)
        {
            Debug.Log("Chinchilla released!");

            _grabbedChinchilla = null;
        }
    }

    public Vector2 GetMousePosition()
    {
        return _actions.Player.Point.ReadValue<Vector2>();
    }
}
