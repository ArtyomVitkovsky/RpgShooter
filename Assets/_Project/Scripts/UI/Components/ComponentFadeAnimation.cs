using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ComponentFadeAnimation : MonoBehaviour
{
    [Header("Target")] 
    [SerializeField] private bool useCanvasGroup;
    [ShowIf("useCanvasGroup")][SerializeField] private CanvasGroup targetCanvasGroup;
    [HideIf("useCanvasGroup")][SerializeField] private Graphic targetGraphic;

    [Header("Values")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float minAlpha = 0f;

    [Title("Curve Configuration")]
    [SerializeField] private bool useCustomOutCurve;

    [BoxGroup("Fade IN Settings")]
    [LabelText("Curve IN")]
    [SerializeField] private AnimationCurve alphaCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [BoxGroup("Fade IN Settings")]
    [FoldoutGroup("Fade IN Settings/Auto Setup")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetups;

    [ShowIf("useCustomOutCurve")]
    [BoxGroup("Fade OUT Settings")]
    [LabelText("Curve OUT")]
    [SerializeField] private AnimationCurve alphaCurveOut = AnimationCurve.Linear(0, 1, 1, 0);

    [ShowIf("useCustomOutCurve")]
    [BoxGroup("Fade OUT Settings")]
    [FoldoutGroup("Fade OUT Settings/Auto Setup")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetupsOut;

    protected CancellationTokenSource cts;
    protected Color initialColor;
    protected float initialAlpha;
    
    private float _currentNormalizedTime = 0f;

    [Button]
    public void GenerateCurves()
    {
        alphaCurve = CreateCurveFromSetup(keyFrameSetups);

        if (useCustomOutCurve)
        {
            alphaCurveOut = CreateCurveFromSetup(keyFrameSetupsOut);
        }
    }

    [Button]
    private void PlayIN()
    {
        PlayAnimation();
    }
    
    [Button]
    private void PlayOUT()
    {
        PlayBackwardsAnimation();
    }
    
    private AnimationCurve CreateCurveFromSetup(KeyFrameSetup[] setups)
    {
        if (setups == null || setups.Length == 0) 
            return AnimationCurve.Linear(0, 0, 1, 1);

        var keyframes = new Keyframe[setups.Length];

        for (var i = 0; i < setups.Length; i++)
        {
            keyframes[i] = new Keyframe(
                setups[i].Time,
                setups[i].Value,
                setups[i].InTangent,
                setups[i].OutTangent);
        }

        return new AnimationCurve(keyframes);
    }

    protected virtual void Awake()
    {
        if (targetGraphic == null)
        {
            targetGraphic = GetComponent<Graphic>();
        }

        if (targetGraphic != null)
        {
            initialColor = targetGraphic.color;
            _currentNormalizedTime = Mathf.InverseLerp(minAlpha, maxAlpha, initialColor.a);
        }
        
        if (targetCanvasGroup != null)
        {
            initialAlpha = targetCanvasGroup.alpha;
            _currentNormalizedTime = Mathf.InverseLerp(minAlpha, maxAlpha, initialAlpha);
        }
    }

    public virtual void StopAnimation()
    {
        cts?.Cancel();
        ResetAlpha();
        _currentNormalizedTime = 0f;
    }

    private void ResetAlpha()
    {
        if (targetGraphic != null)
        {
            targetGraphic.color = initialColor;
        }

        if (targetCanvasGroup != null)
        {
            targetCanvasGroup.alpha = initialAlpha;
        }
    }

    public virtual async UniTask PlayAnimation()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();

        await ProcessAnimation(cts.Token, false);
    }

    public virtual async UniTask PlayBackwardsAnimation()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();

        await ProcessAnimation(cts.Token, true);
    }

    protected virtual async UniTask ProcessAnimation(CancellationToken token, bool backwards)
    {
        if (targetGraphic == null && targetCanvasGroup == null)
        {
            return;
        }

        try
        {
            while (!token.IsCancellationRequested)
            {
                float delta = Time.deltaTime / duration;

                if (backwards)
                {
                    _currentNormalizedTime -= delta;
                }
                else
                {
                    _currentNormalizedTime += delta;
                }

                _currentNormalizedTime = Mathf.Clamp01(_currentNormalizedTime);

                float curveValue;

                if (backwards && useCustomOutCurve)
                {
                    curveValue = alphaCurveOut.Evaluate(1f - _currentNormalizedTime);
                }
                else
                {
                    curveValue = alphaCurve.Evaluate(_currentNormalizedTime);
                }
                
                var newAlpha = Mathf.LerpUnclamped(minAlpha, maxAlpha, curveValue);
                ApplyAlpha(newAlpha);

                if (backwards && _currentNormalizedTime <= 0f) break;
                if (!backwards && _currentNormalizedTime >= 1f) break;

                await UniTask.NextFrame(cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ResetAlpha();
        }
    }
    
    private void ApplyAlpha(float alpha)
    {
        if (useCanvasGroup)
        {
            targetCanvasGroup.alpha = alpha;
        }
        else if (targetGraphic != null)
        {
            var currentColor = targetGraphic.color;
            currentColor.a = alpha;
            targetGraphic.color = currentColor;
        }
    }
}