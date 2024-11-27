using System.IO;
using Code.Infrastructure.Serialization;
using CrazyGames;
using UnityEngine;

namespace Code.Progress.Writer
{
    public class CrazyGamesProgressReadWrite : IProgressReadWrite
    {
        private const string PROGRESS_KEY = "PlayerProgress";

        private static string ProgressPath => Path.Combine(Application.persistentDataPath, PROGRESS_KEY);

        public bool HasSavedProgress()
        {
            return CrazySDK.IsAvailable 
                ? CrazySDK.Data.HasKey(ProgressPath) 
                : PlayerPrefs.HasKey(ProgressPath);
        }

        public void WriteProgress(string json)
        {
            PlayerPrefs.SetString(ProgressPath, json);
            
            if (CrazySDK.IsAvailable)
                CrazySDK.Data.SetString(ProgressPath, json);
            
            PlayerPrefs.Save();
        }

        public T ReadProgress<T>() where T : class
        {
            string progress;

            if (CrazySDK.IsAvailable && CrazySDK.Data.HasKey(ProgressPath))
                progress = CrazySDK.Data.GetString(ProgressPath);
            else
                progress = PlayerPrefs.GetString(ProgressPath);
            
            return progress.FromJson<T>();
        }

        public void DeleteProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
            if (CrazySDK.IsAvailable)
            {
                CrazySDK.Data.DeleteAll();
            }
        }
    }
}