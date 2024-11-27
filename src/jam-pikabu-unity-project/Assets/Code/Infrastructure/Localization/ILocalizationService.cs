using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.Localization
{
    public interface ILocalizationService
    {
        string this[string path] { get; }
        string this[string path, params string[] additional] { get; }
        string this[string path, string variable1] { get; }
        UniTask Initialize();
        void InitLanguageSettings();
        void RegisterHandler(ILocalizationHandler handler);
        void UnregisterHandler(ILocalizationHandler handler);
        void SetLanguage(SystemLanguage language);
    }
}