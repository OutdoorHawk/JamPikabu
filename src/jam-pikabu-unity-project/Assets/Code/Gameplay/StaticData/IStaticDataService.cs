namespace Code.Gameplay.StaticData
{
    public interface IStaticDataService
    {
        public void Load();
        T GetStaticData<T>() where T : class;
    }
}