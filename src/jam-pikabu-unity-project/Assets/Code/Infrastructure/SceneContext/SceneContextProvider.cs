namespace Code.Infrastructure.SceneContext
{
    public class SceneContextProvider : ISceneContextProvider
    {
        public SceneContextComponent Context { get; private set; }

        public void SetContext(SceneContextComponent sceneContextComponent)
        {
            Context = sceneContextComponent;
        }

        public void CleanUp()
        {
            Context = null;
        }
    }
}