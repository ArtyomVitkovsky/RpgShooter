using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class ComponentScaleAnimation : MonoBehaviour
{
    [SerializeField] protected Transform targetTransform;
    [SerializeField] private float duration = 0.2f;

    [Title("Scale Settings")]
    [SerializeField] private bool useTargetScale;
    
    [ShowIf("useTargetScale")]
    [SerializeField] private Vector3 targetScale;

    [Title("Curve Configuration")]
    [SerializeField] private bool separateAxes;
    
    [SerializeField] private bool useCustomOutCurve;

    [HideIf("separateAxes")]
    [BoxGroup("Uniform Curves")]
    [LabelText("Scale In Curve")]
    [SerializeField] private AnimationCurve scaleCurve;

    [HideIf("separateAxes")]
    [BoxGroup("Uniform Curves")]
    [ShowIf("useCustomOutCurve")]
    [LabelText("Scale Out Curve")]
    [SerializeField] private AnimationCurve scaleCurveOut;

    [HideIf("separateAxes")]
    [FoldoutGroup("Uniform Curves/Auto Setup")]
    [LabelText("Setup IN")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetups;

    [HideIf("separateAxes")]
    [ShowIf("useCustomOutCurve")]
    [FoldoutGroup("Uniform Curves/Auto Setup")]
    [LabelText("Setup OUT")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetupsOut;

    [ShowIf("separateAxes")]
    [BoxGroup("Separate Axes IN")]
    [SerializeField] private AnimationCurve scaleCurveX = AnimationCurve.Linear(0, 0, 1, 1);
    
    [ShowIf("separateAxes")]
    [BoxGroup("Separate Axes IN")]
    [SerializeField] private AnimationCurve scaleCurveY = AnimationCurve.Linear(0, 0, 1, 1);
    
    [ShowIf("separateAxes")]
    [BoxGroup("Separate Axes IN")]
    [SerializeField] private AnimationCurve scaleCurveZ = AnimationCurve.Linear(0, 0, 1, 1);

    [ShowIf("separateAxes")]
    [FoldoutGroup("Separate Axes IN/Auto Setup")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetupsX;
    
    [ShowIf("separateAxes")]
    [FoldoutGroup("Separate Axes IN/Auto Setup")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetupsY;
    
    [ShowIf("separateAxes")]
    [FoldoutGroup("Separate Axes IN/Auto Setup")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetupsZ;

    [ShowIf("@this.separateAxes && this.useCustomOutCurve")]
    [BoxGroup("Separate Axes OUT")]
    [SerializeField] private AnimationCurve scaleCurveXOut = AnimationCurve.Linear(0, 1, 1, 0);
    
    [ShowIf("@this.separateAxes && this.useCustomOutCurve")]
    [BoxGroup("Separate Axes OUT")]
    [SerializeField] private AnimationCurve scaleCurveYOut = AnimationCurve.Linear(0, 1, 1, 0);
    
    [ShowIf("@this.separateAxes && this.useCustomOutCurve")]
    [BoxGroup("Separate Axes OUT")]
    [SerializeField] private AnimationCurve scaleCurveZOut = AnimationCurve.Linear(0, 1, 1, 0);

    [ShowIf("@this.separateAxes && this.useCustomOutCurve")]
    [FoldoutGroup("Separate Axes OUT/Auto Setup")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetupsXOut;
    
    [ShowIf("@this.separateAxes && this.useCustomOutCurve")]
    [FoldoutGroup("Separate Axes OUT/Auto Setup")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetupsYOut;
    
    [ShowIf("@this.separateAxes && this.useCustomOutCurve")]
    [FoldoutGroup("Separate Axes OUT/Auto Setup")]
    [SerializeField] private KeyFrameSetup[] keyFrameSetupsZOut;


    protected CancellationTokenSource cts;
    protected Vector3 initialScale;

    [Button]
    public void GenerateCurves()
    {
        if (separateAxes)
        {
            scaleCurveX = CreateCurveFromSetup(keyFrameSetupsX);
            scaleCurveY = CreateCurveFromSetup(keyFrameSetupsY);
            scaleCurveZ = CreateCurveFromSetup(keyFrameSetupsZ);
        }
        else
        {
            scaleCurve = CreateCurveFromSetup(keyFrameSetups);
        }

        if (useCustomOutCurve)
        {
            if (separateAxes)
            {
                scaleCurveXOut = CreateCurveFromSetup(keyFrameSetupsXOut);
                scaleCurveYOut = CreateCurveFromSetup(keyFrameSetupsYOut);
                scaleCurveZOut = CreateCurveFromSetup(keyFrameSetupsZOut);
            }
            else
            {
                scaleCurveOut = CreateCurveFromSetup(keyFrameSetupsOut);
            }
        }
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
        if (targetTransform == null)
        {
            targetTransform = transform;
        }

        initialScale = targetTransform.localScale;
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

    public virtual void StopAnimation()
    {
        cts?.Cancel();
        targetTransform.localScale = initialScale; 
    }

    protected virtual async UniTask ProcessAnimation(CancellationToken token, bool backwards)
    {
        var timeElapsed = 0f;

        AnimationCurve activeCurveX, activeCurveY, activeCurveZ, activeCurveUniform;
        
        bool invertTime;

        if (backwards)
        {
            if (useCustomOutCurve)
            {
                activeCurveUniform = scaleCurveOut;
                activeCurveX = scaleCurveXOut;
                activeCurveY = scaleCurveYOut;
                activeCurveZ = scaleCurveZOut;
                invertTime = false;
            }
            else
            {
                activeCurveUniform = scaleCurve;
                activeCurveX = scaleCurveX;
                activeCurveY = scaleCurveY;
                activeCurveZ = scaleCurveZ;
                invertTime = true;
            }
        }
        else
        {
            activeCurveUniform = scaleCurve;
            activeCurveX = scaleCurveX;
            activeCurveY = scaleCurveY;
            activeCurveZ = scaleCurveZ;
            invertTime = false;
        }

        try
        {
            while (!token.IsCancellationRequested && timeElapsed < duration)
            {
                var factor = timeElapsed / duration;
                
                var evalTime = invertTime ? 1 - factor : factor; 

                Vector3 currentScaleCalculated;

                if (separateAxes)
                {
                    float stepX = activeCurveX.Evaluate(evalTime);
                    float stepY = activeCurveY.Evaluate(evalTime);
                    float stepZ = activeCurveZ.Evaluate(evalTime);

                    if (useTargetScale)
                    {
                        currentScaleCalculated = new Vector3(
                            Mathf.LerpUnclamped(initialScale.x, targetScale.x, stepX),
                            Mathf.LerpUnclamped(initialScale.y, targetScale.y, stepY),
                            Mathf.LerpUnclamped(initialScale.z, targetScale.z, stepZ)
                        );
                    }
                    else
                    {
                        currentScaleCalculated = new Vector3(
                            initialScale.x * stepX,
                            initialScale.y * stepY,
                            initialScale.z * stepZ
                        );
                    }
                }
                else
                {
                    var step = activeCurveUniform.Evaluate(evalTime);
                
                    if (useTargetScale)
                    {
                        currentScaleCalculated = Vector3.LerpUnclamped(initialScale, targetScale, step);
                    }
                    else
                    {
                        currentScaleCalculated = initialScale * step;
                    }
                }

                targetTransform.localScale = currentScaleCalculated;

                await UniTask.NextFrame(cancellationToken: token);

                timeElapsed += Time.deltaTime;
            }

            if (!token.IsCancellationRequested)
            {
                var finalEvalTime = invertTime ? 0 : 1;
                
                Vector3 finalScale;

                if (separateAxes)
                {
                    float stepX = activeCurveX.Evaluate(finalEvalTime);
                    float stepY = activeCurveY.Evaluate(finalEvalTime);
                    float stepZ = activeCurveZ.Evaluate(finalEvalTime);

                    if (useTargetScale)
                    {
                        finalScale = new Vector3(
                            Mathf.LerpUnclamped(initialScale.x, targetScale.x, stepX),
                            Mathf.LerpUnclamped(initialScale.y, targetScale.y, stepY),
                            Mathf.LerpUnclamped(initialScale.z, targetScale.z, stepZ)
                        );
                    }
                    else
                    {
                        finalScale = new Vector3(
                            initialScale.x * stepX,
                            initialScale.y * stepY,
                            initialScale.z * stepZ
                        );
                    }
                }
                else
                {
                    var factor = activeCurveUniform.Evaluate(finalEvalTime);
                    if (useTargetScale)
                    {
                        finalScale = Vector3.LerpUnclamped(initialScale, targetScale, factor);
                    }
                    else
                    {
                        finalScale = initialScale * factor;
                    }
                }
                
                targetTransform.localScale = finalScale;
            }
        }
        catch (OperationCanceledException)
        {
            targetTransform.localScale = initialScale;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            targetTransform.localScale = initialScale;
        }
    }
}