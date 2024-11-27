using UnityEngine;

namespace Code.Infrastructure.SceneContext
{
    public class SceneContextProvider : ISceneContextProvider
    {
        public Transform PlayerSpawnPoint { get; private set; }
        public Transform TriggersParent { get; private set; }

        public SceneContextComponent Context { get; private set; }

        public void SetComponent(SceneContextComponent sceneContextComponent)
        {
            Context = sceneContextComponent;
        }

        public void SetPlayerSpawnPoint(Transform playerSpawnPoint)
        {
            PlayerSpawnPoint = playerSpawnPoint;
        }

        public void CleanUp()
        {
        }
    }
}