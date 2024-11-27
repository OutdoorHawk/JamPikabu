using UnityEngine;

namespace Code.Gameplay.Sound.Behaviours
{
    [RequireComponent(typeof(AudioSource))]
    public class PitchRandomizer : MonoBehaviour
    {
        [SerializeField] private float _minPitch = 0.8f;
        [SerializeField] private float _maxPitch = 1.2f;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
        }
    }
}