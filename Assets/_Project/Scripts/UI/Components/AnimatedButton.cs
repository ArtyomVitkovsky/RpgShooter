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
        if (!canvasGroup.interactable || _isSelected)
        {
            return;
        }

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
        if (!canvasGroup.interactable)
        {
            return;
        }

        if (!isPointerOnButton || eventData.dragging)
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

            return;
        }

        ProcessButtonClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOnButton = true;

        if (!canvasGroup.interactable) return;

        if (_isSelected)
        {
            return;
        }
        
        foreach (var fadeAnimation in fadeAnimations)
        {
            fadeAnimation.PlayAnimation();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOnButton = false;

        if (!canvasGroup.interactable || _isSelected)
        {
            return;
        }
        
        foreach (var fadeAnimation in fadeAnimations)
        {
            fadeAnimation.PlayBackwardsAnimation();
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