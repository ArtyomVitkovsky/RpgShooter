using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimatedButton : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected CanvasGroup canvasGroup;

    [Header("Animations")] 
    [SerializeField] protected ComponentFadeAnimation[] fadeAnimations;
    [SerializeField] private ComponentScaleAnimation[] scaleAnimations;
    
    private readonly Subject<AnimatedButton> _onButtonClick = new Subject<AnimatedButton>();
    public IObservable<AnimatedButton> OnButtonClick => _onButtonClick;

    protected bool isPointerOnButton;
    protected bool _isSelected;

    private int activePointerId = -1;
    private HashSet<int> pointersInside = new HashSet<int>();

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
        Debug.Log($"AnimatedButton OnPointer Down {eventData.pointerId}");
        
        if (!canvasGroup.interactable || _isSelected || activePointerId != -1)
        {
            return;
        }

        eventData.Use();
        activePointerId = eventData.pointerId;
        
        foreach (var scaleAnimation in scaleAnimations)
        {
            scaleAnimation.PlayAnimation();
        }
        
        foreach (var fadeAnimation in fadeAnimations)
        {
            fadeAnimation.PlayAnimation();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"AnimatedButton OnPointer Up {eventData.pointerId}");
        if (eventData.pointerId == activePointerId || eventData.pointerPress == gameObject)
        {
            activePointerId = -1;

            var current = eventData.pointerCurrentRaycast.gameObject;
            bool isOverButton = current != null &&
                                (current == gameObject || current.transform.IsChildOf(transform));

            if (!isOverButton)
            {
                ResetVisuals();
                return;
            }

            ProcessButtonClick().Forget();
        }
    }
    
    private void ResetVisuals()
    {
        foreach (var scaleAnimation in scaleAnimations)
        {
            scaleAnimation.PlayBackwardsAnimation();
        }
        if (!_isSelected)
        {
            foreach (var fadeAnimation in fadeAnimations)
            {
                fadeAnimation.PlayBackwardsAnimation();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointersInside.Add(eventData.pointerId);
        
        isPointerOnButton = true;

        if (!canvasGroup.interactable || _isSelected) return;

        foreach (var fadeAnimation in fadeAnimations)
        {
            fadeAnimation.PlayAnimation();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointersInside.Remove(eventData.pointerId);
        if (pointersInside.Count == 0)
        {
            isPointerOnButton = false;
        }

        if (!canvasGroup.interactable || _isSelected)
        {
            return;
        }
        
        if (pointersInside.Count == 0)
        {
            foreach (var fadeAnimation in fadeAnimations)
            {
                fadeAnimation.PlayBackwardsAnimation();
            }
        }
    }

    public virtual void SetSelected(bool selected)
    {
        _isSelected = selected;

        if (selected)
        {
            foreach (var fadeAnimation in fadeAnimations)
            {
                fadeAnimation.PlayAnimation();
            }
        }
        else if (!isPointerOnButton)
        {
            foreach (var fadeAnimation in fadeAnimations)
            {
                fadeAnimation.PlayBackwardsAnimation();
            }
        }
    }

    public virtual void SetInteractable(bool interactable)
    {
        canvasGroup.interactable = interactable;

        if (!interactable)
        {
            _isSelected = false;
        }
    }

    protected async UniTask ProcessButtonClick()
    {
        var animationTasks = new List<UniTask>(scaleAnimations.Length);
        foreach (var scaleAnimation in scaleAnimations)
        {
            animationTasks.Add(scaleAnimation.PlayBackwardsAnimation());
        }

        await UniTask.WhenAll(animationTasks);
        
        _onButtonClick?.OnNext(this);
    }

    private void OnDisable()
    {
        foreach (var fadeAnimation in fadeAnimations)
        {
            fadeAnimation.PlayBackwardsAnimation();
        }
    }
}