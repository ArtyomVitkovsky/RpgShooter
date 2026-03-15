using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Project.Scripts.UI
{
    public class UICameraStackBinder : MonoBehaviour
    {
        void Start()
        {
            var uiCamera = GetComponent<Camera>();
        
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                var cameraData = mainCamera.GetUniversalAdditionalCameraData();
            
                if (!cameraData.cameraStack.Contains(uiCamera))
                {
                    cameraData.cameraStack.Add(uiCamera);
                }
            }
            else
            {
                Debug.LogWarning("Main Camera not found");
            }
        }
    }
}