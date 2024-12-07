using Code.Gameplay.Sound.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Sound.Behaviours
{
    [RequireComponent(typeof(Button))]
    public class ButtonSoundComponent : MonoBehaviour
    {
        [SerializeField] private SoundTypeId _soundType = SoundTypeId.UI_Click;
        [SerializeField] private SoundTypeId _overrideSound = SoundTypeId.Unknown;

        private ISoundService _soundService;

        private Button _button;

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void Start()
        {
            _button.onClick.AddListener(PlaySound);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(PlaySound);
        }

        private void PlaySound()
        {
            if (_overrideSound is not SoundTypeId.Unknown)
            {
                _soundService.PlaySound(_overrideSound);
            }
            else
            {
                _soundService.PlaySound(_soundType);
            }
        }
    }
}