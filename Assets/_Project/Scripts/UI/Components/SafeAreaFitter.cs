using UnityEngine;

namespace _Project.Scripts.Utils
{
    public enum SafeMode
    {
        All,           // Учитывать все стороны (верх, низ, лево, право)
        OnlyTop,       // Только верхнюю границу (челка)
        OnlyBottom,    // Только нижнюю (полоса навигации)
        TopAndBottom   // Верх и низ, игнорируя бока
    }
    
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private SafeMode mode = SafeMode.All;

        private RectTransform _rectTransform;
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            Refresh();
        }

        private void Update()
        {
            // Проверяем изменение SafeArea каждый кадр (на случай поворота экрана)
            if (_lastSafeArea != UnityEngine.Device.Screen.safeArea)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            Rect safeArea = UnityEngine.Device.Screen.safeArea;
            _lastSafeArea = safeArea;

            // Конвертируем пиксельные координаты в нормализованные (0.0 - 1.0)
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= UnityEngine.Device.Screen.width;
            anchorMin.y /= UnityEngine.Device.Screen.height;
            anchorMax.x /= UnityEngine.Device.Screen.width;
            anchorMax.y /= UnityEngine.Device.Screen.height;

            // Применяем фильтрацию сторон в зависимости от выбранного режима
            switch (mode)
            {
                case SafeMode.OnlyTop:
                    anchorMin.x = 0;
                    anchorMin.y = 0;
                    anchorMax.x = 1;
                    // anchorMax.y остается из safeArea
                    break;

                case SafeMode.OnlyBottom:
                    // anchorMin.y остается из safeArea
                    anchorMin.x = 0;
                    anchorMax.x = 1;
                    anchorMax.y = 1;
                    break;

                case SafeMode.TopAndBottom:
                    anchorMin.x = 0;
                    anchorMax.x = 1;
                    // Y берем из safeArea
                    break;

                case SafeMode.All:
                    // Используем все рассчитанные значения
                    break;
            }

            // Устанавливаем якоря
            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;

            // Сбрасываем смещения (Offsets), чтобы RectTransform точно прилегал к якорям
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }
    }
}