using UnityEngine;

namespace Code.Gameplay.Tutorial.Behaviours
{
    public class TutorialBehaviourComponent : MonoBehaviour
    {
        [SerializeField] private TutorialConditionComponent _conditionComponent;
        [SerializeField] private MonoBehaviour _component;
        
        private void Awake()
        {
            _conditionComponent.ConditionChanged += UpdateStatus;
        }

        private void OnDestroy()
        {
            _conditionComponent.ConditionChanged -= UpdateStatus;
        }

        private void UpdateStatus()
        {
            _component.enabled = _conditionComponent.CurrentCondition;
        }
    }
}