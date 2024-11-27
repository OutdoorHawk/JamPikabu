using Code.Common.Extensions;
using Code.Gameplay.Tutorial.Service;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Tutorial.Behaviours
{
    public class TutorialLockedGameObject : MonoBehaviour
    {
        [SerializeField] private TutorialTypeId _type;
        [SerializeField] private bool _invertCondition;
        [SerializeField] private bool _destroyAfterInit;

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
            bool canActivateGo = _tutorialService.Value.IsTutorialStartedOrCompleted(_type);

            canActivateGo = _invertCondition
                ? canActivateGo == false
                : canActivateGo;

            if (canActivateGo == false)
                gameObject.DisableElement();
            else
                gameObject.EnableElement();

            if (_destroyAfterInit)
                Destroy(this);
        }
    }
}