using System.IO;
using Code.Infrastructure.Serialization;
using UnityEngine;
using static Code.Progress.SaveLoadService.ISaveLoadService;

namespace Code.Progress.Writer
{
    public class DefaultFileProgressReadWrite : IProgressReadWrite
    {
        private static string ProgressPath => Path.Combine(Application.persistentDataPath, PROGRESS_KEY);

        public bool HasSavedProgress()
        {
            return File.Exists(ProgressPath);
        }

        public void WriteProgress(string json)
        {
            File.WriteAllText(ProgressPath, json);
        }

        public T ReadProgress<T>() where T : class
        {
            if (!File.Exists(ProgressPath))
                return null;

            string progress = File.ReadAllText(ProgressPath);
            return progress.FromJson<T>();
        }

        public void DeleteProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
            if (!File.Exists(ProgressPath))
                return;

            File.Delete(ProgressPath);
        }
    }
}