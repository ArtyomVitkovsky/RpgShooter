using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace _Project.Scripts.UI.Components
{
    public class OnScreenTouchpad : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [InputControl(layout = "Vector2")] [SerializeField]
        private string _controlPath;

        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SendValueToControl(Vector2.zero);
        }

        public void OnDrag(PointerEventData eventData)
        {
            SendValueToControl(eventData.delta);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SendValueToControl(Vector2.zero);
        }

    }
}