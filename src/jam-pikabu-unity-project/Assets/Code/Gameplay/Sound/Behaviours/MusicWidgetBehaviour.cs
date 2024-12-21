using Code.Gameplay.Sound.Service;
using Code.Meta.Features.Days.Configs;
using Code.Meta.Features.Days.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Sound.Behaviours
{
    public class MusicWidgetBehaviour : MonoBehaviour
    {
        [SerializeField] private Button _next;
        [SerializeField] private Button _previous;
        [SerializeField] private TMP_Text _songName;

        private ISoundService _soundService;
        private IDaysService _daysService;

        [Inject]
        private void Construct(ISoundService soundService, IDaysService daysService)
        {
            _daysService = daysService;
            _soundService = soundService;
        }

        private void Start()
        {
            InitBossState();
            _next.onClick.AddListener(NextSong);
            _previous.onClick.AddListener(PreviousSong);
            _soundService.OnSongUpdated += UpdateSongName;
            UpdateSongName();
        }

        private void OnDestroy()
        {
            _next.onClick.RemoveListener(NextSong);
            _previous.onClick.RemoveListener(PreviousSong);
            _soundService.OnSongUpdated -= UpdateSongName;
        }

        private void InitBossState()
        {
            DayData dayData = _daysService.GetDayData();
            if (dayData == null)
                return;

            if (dayData.IsBossDay)
            {
                _next.interactable = false;
                _previous.interactable = false;
            }
        }

        private void UpdateSongName()
        {
            AudioClip currentClip = _soundService.GetCurrentMusicClip();
            _songName.text = currentClip.name;
        }

        private void NextSong()
        {
            _soundService.NextSong();
        }

        private void PreviousSong()
        {
            _soundService.PreviousSong();
        }
    }
}