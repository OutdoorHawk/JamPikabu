using UnityEngine.Localization;

namespace Code.Infrastructure.Localization
{
    public interface ILocalizationHandler
    {
        void OnLanguageChanged(Locale locale);
    }
}