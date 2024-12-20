using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.MapBlocks.Behaviours
{
    public class AvailableIngredientsView : MonoBehaviour
    {
        public HorizontalLayoutGroup Layout;
        public GameObject IconTemplate;

        [Inject]
        private void Construct()
        {

        }
    }
}