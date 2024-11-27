using UnityEngine.Localization;

namespace Code.Infrastructure.Localization
{
    public static class LocalizationExtensions
    {
        public static string GetLocalizedStringSafe(this LocalizedString locale)
        {
            if (locale.IsEmpty)
                return string.Empty;

            return locale.GetLocalizedString();
        }
    }
}