using UnityEngine;

namespace Code.Gameplay.Cheats.Behaviours
{
    public class CheatGameObjectDisabler : MonoBehaviour
    {
        private void Awake()
        {
#if !CHEAT
            Destroy(gameObject);
#endif
        }
    }
}