using Cysharp.Threading.Tasks;

namespace Code.Gameplay.StaticData
{
    public interface IStaticDataService
    {
        UniTask Load();
        T GetStaticData<T>() where T : class;
        void RegisterHandler(IConfigsInitHandler handler);
        void UnRegisterHandler(IConfigsInitHandler handler);
    }
}