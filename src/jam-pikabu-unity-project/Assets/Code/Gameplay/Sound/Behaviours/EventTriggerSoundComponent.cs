using Code.Gameplay.Sound.Service;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Code.Gameplay.Sound.Behaviours
{
    [RequireComponent(typeof(EventTrigger))]
    public class EventTriggerSoundComponent : MonoBehaviour
    {
        [SerializeField] private SoundTypeId _soundType = SoundTypeId.UI_Click;
        [SerializeField] private EventTrigger.Entry _entry;
        
        private ISoundService _soundService;
        
        private EventTrigger _trigger;

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }

        private void Awake()
        {
            _trigger = GetComponent<EventTrigger>();
        }

        private void Start()
        {
            _entry.callback.AddListener(PlaySound);
            _trigger.triggers.Add(_entry);
        }

        private void OnDestroy()
        {
            _trigger.triggers.Clear();
        }

        private void PlaySound(BaseEventData _)
        {
            _soundService.PlaySound(_soundType);
        }
    }
}