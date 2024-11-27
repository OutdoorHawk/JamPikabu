using System.Collections;
using Code.Gameplay.Sound;
using Code.Gameplay.Sound.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Settings.Window
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private SoundVolumeTypeId _type;
        [SerializeField] private Slider _slider;

        private ISoundService _soundService;
        private Coroutine _testSound;

        private readonly WaitForSeconds Delay = new(0.4f);

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }

        private void Start()
        {
            float volumeForChannel = _soundService.GetVolumeForChannel(_type);
            _slider.SetValueWithoutNotify(volumeForChannel);
            _slider.onValueChanged.AddListener(UpdateVolume);
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveListener(UpdateVolume);
        }

        private void UpdateVolume(float arg0)
        {
            _soundService.SetVolume(_type, arg0);

            if (_type is SoundVolumeTypeId.SfxVolume)
            {
                _testSound ??= StartCoroutine(PlayTestSoundRoutine());
            }
        }

        private IEnumerator PlayTestSoundRoutine()
        {
            _soundService.PlaySound(SoundTypeId.UI_Click);
            yield return Delay;
            _testSound = null;
        }
    }
}