using System;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.SceneLoading
{
    public interface ISceneLoader
    {
        void LoadScene(SceneTypeId sceneID, Func<UniTask> loadOperation = null, Action onLoaded = null);
        void LoadScene(string sceneName, Func<UniTask> loadOperation = null, Action onLoaded = null);
    }
}