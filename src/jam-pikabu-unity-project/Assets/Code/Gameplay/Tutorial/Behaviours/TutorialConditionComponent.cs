using System;
using Code.Gameplay.Tutorial.Service;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Tutorial.Behaviours
{
    public class TutorialConditionComponent : MonoBehaviour
    {
        [SerializeField] private TutorialTypeId _type;
        [SerializeField] private bool _invertCondition;
        
        public bool CurrentCondition { get; private set; }
        
        public event Action ConditionChanged;

        private LazyInject<ITutorialService> _tutorialService;

        [Inject]
        private void Construct(LazyInject<ITutorialService> tutorialService)
        {
            _tutorialService = tutorialService;
        }

        private void Awake()
        {
            _tutorialService.Value.OnTutorialUpdate += UpdateStatus;
        }

        private void OnDestroy()
        {
            _tutorialService.Value.OnTutorialUpdate -= UpdateStatus;
        }

        private void Start()
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            CurrentCondition = GetCondition();
            ConditionChanged?.Invoke();
        }

        private bool GetCondition()
        {
            bool status = _tutorialService.Value.IsTutorialStartedOrCompleted(_type);
            
            status = _invertCondition
                ? status == false
                : status;
            
            return status;
        }
    }
}