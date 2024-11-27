using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Tutorial.Behaviours
{
    public class TutorialInteractableSelectableComponent : MonoBehaviour
    {
        [SerializeField] private TutorialConditionComponent _conditionComponent;
        [SerializeField] private Selectable _component;
        
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
            _component.interactable = _conditionComponent.CurrentCondition;
        }
    }
}