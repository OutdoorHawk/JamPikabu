using System;
using Code.Gameplay.Features.RoundState.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Features.RoundState.Behaviours
{
    public class CurrentDayViewBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dayText;

        private IRoundStateService _roundStateService;

        [Inject]
        private void Construct(IRoundStateService roundStateService)
        {
            _roundStateService = roundStateService;
        }

        private void Start()
        {
            _roundStateService.OnDayBegin += UpdateText;
            UpdateText();
        }

        private void OnDestroy()
        {
            _roundStateService.OnDayBegin -= UpdateText;
        }

        private void UpdateText()
        {
            _dayText.text = $"{_roundStateService.CurrentDay}/{_roundStateService.MaxDays}";
        }
    }
}