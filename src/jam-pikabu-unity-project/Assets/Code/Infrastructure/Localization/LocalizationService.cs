using System;
using System.Collections.Generic;
using Code.Common.Logger.Service;
using Code.Infrastructure.DI.Installers;
using Code.Progress.Provider;
using Code.Progress.SaveLoadService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Code.Infrastructure.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly LocalizedString _localizedString = new();

        public string this[string path] => GetLocalizedString(path);
        public string this[string path, params string[] additional] => GetLocalizedString(path, additional);
        public string this[string path, string variable1] => GetLocalizedString(path, variable1);

        private StringVariable _stringVariable;
        private ISaveLoadService _saveLoadService;
        private IProgressProvider _progressProvider;
        private List<ILocalizationHandler> _handlers;
        private ILoggerService _logger;

        [Inject]
        private void Construct
        (
            ISaveLoadService saveLoadService,
            IProgressProvider progressProvider,
            List<ILocalizationHandler> handlers,
            ILoggerService logger
        )
        {
            _logger = logger;
            _handlers = handlers;
            _progressProvider = progressProvider;
            _saveLoadService = saveLoadService;
        }

        public async UniTask Initialize()
        {
            LocalizationSettings.SelectedLocaleChanged += NotifyLocaleChanged;
            UniTaskCompletionSource source = new UniTaskCompletionSource();
            var operation = LocalizationSettings.InitializationOperation;
            operation.Completed += _ => { source.TrySetResult(); };
            await source.Task;
        }

        public void InitLanguageSettings()
        {
            ref string savedLanguage = ref _progressProvider.Progress.PlayerSettings.savedLanguageString;

            if (string.IsNullOrEmpty(savedLanguage))
                LoadLangFromSystem();
            else
                LoadSavedLanguage(savedLanguage);

            savedLanguage = LocalizationSettings.SelectedLocale.LocaleName;
            _saveLoadService.SaveProgress();
        }

        public void RegisterHandler(ILocalizationHandler handler)
        {
            _handlers.Add(handler);
        }

        public void UnregisterHandler(ILocalizationHandler handler)
        {
            _handlers.Remove(handler);
        }
        
        public void SetLanguage(SystemLanguage language)
        {
            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                if (locale.Identifier.CultureInfo != null && 
                    locale.Identifier.CultureInfo.EnglishName == language.ToString())
                {
                    LocalizationSettings.SelectedLocale = locale;
                    NotifyLocaleChanged(locale);
                    _progressProvider.Progress.PlayerSettings.savedLanguageString = locale.LocaleName;
                    _saveLoadService.SaveProgress();
                    return;
                }
            }
            
            _logger.LogError($"Locale for language '{language}' not found.");
        }
        
        private string GetLocalizedString(string path, params string[] additional)
        {
            string text;

            try
            {
                string[] keys = path.Split("/");
                string table = keys[0];
                string entry = keys[1];

                _localizedString.TableReference = table;
                _localizedString.TableEntryReference = entry;

                TryAddSmartValue(additional);

                text = _localizedString.GetLocalizedString();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                text = path;
            }


            return text.Replace("\r", "");
        }

        private void TryAddSmartValue(string[] additional)
        {
            if (additional is not { Length: > 0 })
                return;

            for (int i = 0; i < additional.Length; i++)
            {
                _localizedString.Add(i.ToString(), new StringVariable { Value = additional[i] });
            }
        }

        private void LoadSavedLanguage(string savedLanguage)
        {
            foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
            {
                if (locale.LocaleName != savedLanguage)
                    continue;

                LocalizationSettings.Instance.SetSelectedLocale(locale);
                return;
            }
        }

        private static void LoadLangFromSystem()
        {
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                if (Application.systemLanguage.ToString() != LocalizationSettings.AvailableLocales.Locales[i].Identifier.CultureInfo.EnglishName)
                    continue;

                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                return;
            }

            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                if (SystemLanguage.English.ToString() != LocalizationSettings.AvailableLocales.Locales[i].Identifier.CultureInfo.EnglishName)
                    continue;

                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
                return;
            }
        }

        private void NotifyLocaleChanged(Locale locale)
        {
            foreach (var handler in _handlers)
                handler.OnLanguageChanged(locale);
        }
    }
}