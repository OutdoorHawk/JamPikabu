using UnityEngine;
using Zenject;

namespace Code.Infrastructure.SceneContext
{
    public class SceneContextComponent : MonoBehaviour
    {
        public Transform HookSpawnPoint;
        public Transform BoxSpawnPoint;
        public Transform LootParent;
        public Transform[] LootSpawnPoints;

        [Inject]
        private void Construct(ISceneContextProvider sceneContextProvider)
        {
            sceneContextProvider.SetContext(this);
        }
    }
}