using UnityEngine;
using Zenject;

namespace Code.Infrastructure.SceneContext
{
    public class SceneContextComponent : MonoBehaviour
    {
        public Transform HookSpawnPoint;
        public Transform BoxSpawnPoint;
        public Transform LootSpawnPoint;

        [Inject]
        private void Construct(ISceneContextProvider sceneContextProvider)
        {
            sceneContextProvider.SetContext(this);
        }
    }
}