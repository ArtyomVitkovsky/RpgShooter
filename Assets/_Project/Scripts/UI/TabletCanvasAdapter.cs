using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

public class TabletCanvasAdapter : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;

    private void Awake()
    {
        canvasScaler.matchWidthOrHeight = TabletDetectorUtil.IsTablet() ? 1 : 0;
    }
}
