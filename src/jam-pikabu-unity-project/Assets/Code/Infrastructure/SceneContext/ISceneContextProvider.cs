using UnityEngine;

namespace Code.Infrastructure.SceneContext
{
    public interface ISceneContextProvider
    {
        Transform PlayerSpawnPoint { get; }
        Transform TriggersParent { get; }
        SceneContextComponent Context { get; }

        void SetComponent(SceneContextComponent sceneContextComponent);
        void SetPlayerSpawnPoint(Transform playerSpawnPoint);
        void CleanUp();
    }
}