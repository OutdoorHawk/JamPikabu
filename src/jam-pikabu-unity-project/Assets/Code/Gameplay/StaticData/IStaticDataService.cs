using Cysharp.Threading.Tasks;

namespace Code.Gameplay.StaticData
{
    public interface IStaticDataService
    {
        public UniTaskVoid Load();
        T GetStaticData<T>() where T : class;
    }
}