using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class FloatingStick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerExitHandler
{
    [InputControl(layout = "Vector2")] [SerializeField] private string _controlPath;
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float movementRange = 50f;

    private RectTransform _areaRect;
    private Vector2 _initialAnchoredPos;
    private int _activePointerId = -1; 

    protected override string controlPathInternal { get => _controlPath; set => _controlPath = value; }

    void Awake()
    {
        _areaRect = GetComponent<RectTransform>();
        _initialAnchoredPos = container.anchoredPosition;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ResetStick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_activePointerId != -1) return;

        _activePointerId = eventData.pointerId;
        eventData.useDragThreshold = false; 

        container.gameObject.SetActive(true);
        UpdateJoystick(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != _activePointerId) return;
        UpdateJoystick(eventData);
    }

    private void UpdateJoystick(PointerEventData eventData)
    {
        if (eventData.pointerEnter == gameObject && container.anchoredPosition == _initialAnchoredPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_areaRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos);
            container.anchoredPosition = localPos;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, eventData.position, eventData.pressEventCamera, out Vector2 handlePos);
        Vector2 clampedPos = Vector2.ClampMagnitude(handlePos, movementRange);
        handle.anchoredPosition = clampedPos;

        SendValueToControl(clampedPos / movementRange);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != _activePointerId) return;

        ResetStick();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerId != _activePointerId) return;

        ResetStick();
    }

    private void ResetStick()
    {
        _activePointerId = -1;
        SendValueToControl(Vector2.zero);
        handle.anchoredPosition = Vector2.zero;
        container.anchoredPosition = _initialAnchoredPos;
    }
}