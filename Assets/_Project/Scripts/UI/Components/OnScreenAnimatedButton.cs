using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class OnScreenAnimatedButton : OnScreenControl,
    IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [InputControl(layout = "Button")]
    [SerializeField] private string _controlPath;

    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animations")] 
    [SerializeField] private ComponentFadeAnimation[] fadeAnimations;
    [SerializeField] private ComponentScaleAnimation[] scaleAnimations;

    private int _activePointerId = -1;
    private bool _isPointerOverButton;

    protected override string controlPathInternal
    {
        get => _controlPath;
        set => _controlPath = value;
    }

    private void OnValidate()
    {
        if (!TryGetComponent(out CanvasGroup currentCanvasGroup))
        {
            currentCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup ??= currentCanvasGroup;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canvasGroup.interactable || _activePointerId != -1)
        {
            return;
        }

        _activePointerId = eventData.pointerId;
        eventData.useDragThreshold = false;

        PlayPressAnimations();
        SendValueToControl(1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != _activePointerId)
        {
            return;
        }

        _activePointerId = -1;
        SendValueToControl(0f);

        if (!_isPointerOverButton)
        {
            ResetVisuals();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!canvasGroup.interactable)
        {
            return;
        }

        _isPointerOverButton = true;

        foreach (var fadeAnimation in fadeAnimations)
        {
            fadeAnimation.PlayAnimation();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerId != _activePointerId)
        {
            _isPointerOverButton = false;
        }

        if (!canvasGroup.interactable)
        {
            return;
        }

        if (!_isPointerOverButton)
        {
            foreach (var fadeAnimation in fadeAnimations)
            {
                fadeAnimation.PlayBackwardsAnimation();
            }
        }
    }

    private void PlayPressAnimations()
    {
        foreach (var scaleAnimation in scaleAnimations)
        {
            scaleAnimation.PlayAnimation();
        }

        foreach (var fadeAnimation in fadeAnimations)
        {
            fadeAnimation.PlayAnimation();
        }
    }

    private void ResetVisuals()
    {
        foreach (var scaleAnimation in scaleAnimations)
        {
            scaleAnimation.PlayBackwardsAnimation();
        }

        foreach (var fadeAnimation in fadeAnimations)
        {
            fadeAnimation.PlayBackwardsAnimation();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _activePointerId = -1;
        SendValueToControl(0f);
        ResetVisuals();
    }
}

