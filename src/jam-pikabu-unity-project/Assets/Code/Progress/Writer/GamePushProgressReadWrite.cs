using Code.Gameplay.StaticData;
using Code.Infrastructure.Serialization;
using GamePush;
using UnityEngine;
using static Code.Progress.SaveLoadService.ISaveLoadService;

namespace Code.Progress.Writer
{
    public class GamePushProgressReadWrite : IProgressReadWrite
    {
        public bool HasSavedProgress()
        {
#if !UNITY_EDITOR
            if (GP_Init.isReady && GP_Player.Has(PROGRESS_KEY))
                return true;
#endif
            if (PlayerPrefs.HasKey(PROGRESS_KEY))
                return true;

            return false;
        }

        public void WriteProgress(string json)
        {
            if (GP_Init.isReady)
            {
                bool hasDiffs = json.Equals(GP_Player.GetString(PROGRESS_KEY)) == false;
                GP_Player.Set(PROGRESS_KEY, json);

                if (GP_Player.HasAnyCredentials() && hasDiffs)
                    GP_Player.Sync();
            }

            PlayerPrefs.SetString(PROGRESS_KEY, json);
            PlayerPrefs.Save();
        }

        public T ReadProgress<T>() where T : class
        {
            string progress = PlayerPrefs.GetString(PROGRESS_KEY);

#if !UNITY_EDITOR
            if (GP_Init.isReady && GP_Player.Has(PROGRESS_KEY))
            {
                progress = GP_Player.GetString(PROGRESS_KEY);
            }
#endif
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