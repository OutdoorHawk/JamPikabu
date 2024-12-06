using UnityEngine;

namespace Code.Gameplay.Common.Behaviours
{
    public class DisableOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}