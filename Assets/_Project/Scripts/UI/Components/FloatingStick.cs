using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace _Project.Scripts.UI.Components
{
    public class FloatingStick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [InputControl(layout = "Vector2")] [SerializeField]
        private string _controlPath;

        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float movementRange = 50f;

        private RectTransform _areaRect;
        private Vector2 _initialAnchoredPos;

        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }

        void Awake()
        {
            _areaRect = GetComponent<RectTransform>();
            _initialAnchoredPos = container.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            container.gameObject.SetActive(true);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _areaRect, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 localPos);

            container.anchoredPosition = localPos;
            handle.anchoredPosition = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                container, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector2 handlePos);

            Vector2 clampedPos = Vector2.ClampMagnitude(handlePos, movementRange);
            handle.anchoredPosition = clampedPos;

            SendValueToControl(clampedPos / movementRange);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SendValueToControl(Vector2.zero);
            handle.anchoredPosition = Vector2.zero;
            container.anchoredPosition = _initialAnchoredPos;
        }
    }
}