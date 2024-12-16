using Code.Infrastructure.Serialization;
using GamePush;
using UnityEngine;

namespace Code.Progress.Writer
{
    public class GamePushProgressReadWrite : IProgressReadWrite
    {
        private const string PROGRESS_KEY = "player_progress";

        public bool HasSavedProgress()
        {
            if (GP_Init.isReady == false)
                return PlayerPrefs.HasKey(PROGRESS_KEY);

            if (GP_Player.Has(PROGRESS_KEY) || PlayerPrefs.HasKey(PROGRESS_KEY))
                return true;

            return false;
        }

        public void WriteProgress(string json)
        {
            if (GP_Init.isReady)
            {
                GP_Player.Set(PROGRESS_KEY, json);

                if (GP_Player.HasAnyCredentials())
                    GP_Player.Sync();
            }

            PlayerPrefs.SetString(PROGRESS_KEY, json);
            PlayerPrefs.Save();
        }

        public T ReadProgress<T>() where T : class
        {
            string progress;

            if (GP_Init.isReady && GP_Player.Has(PROGRESS_KEY))
                progress = GP_Player.GetString(PROGRESS_KEY);
            else
                progress = PlayerPrefs.GetString(PROGRESS_KEY);

            return progress.FromJson<T>();
        }

        public void DeleteProgress()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            if (GP_Init.isReady)
                GP_Player.ResetPlayer();

            if (GP_Player.HasAnyCredentials())
                GP_Player.Sync();
        }
    }
}