namespace Code.Infrastructure.SceneContext
{
    public interface ISceneContextProvider
    {
        SceneContextComponent Context { get; }
        void SetContext(SceneContextComponent sceneContextComponent);
        void CleanUp();
    }
}