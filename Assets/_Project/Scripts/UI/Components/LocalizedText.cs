using _Project.Scripts.Core.Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Components
{
    public class LocalizedText : MonoBehaviour
    {
        [Inject] ILocalizationService _localizationService;
        
        [SerializeField] private string localizationKey;
        [SerializeField] private TMP_Text text;

        public void SetLocalizationKey(string key)
        {
            localizationKey = key;
        }

        public void UpdateLocalizedText()
        {
            text.text = _localizationService.GetLocalizedString(localizationKey);
        }
    }
}