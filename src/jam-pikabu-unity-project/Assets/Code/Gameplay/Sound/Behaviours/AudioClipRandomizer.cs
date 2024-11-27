using UnityEngine;

namespace Code.Gameplay.Sound.Behaviours
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioClipRandomizer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _clips;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _clips[Random.Range(0, _clips.Length)];
            _audioSource.Play();
        }
    }
}