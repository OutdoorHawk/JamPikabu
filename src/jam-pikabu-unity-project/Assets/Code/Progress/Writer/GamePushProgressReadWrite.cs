using Code.Gameplay.StaticData;
using Code.Infrastructure.Serialization;
using GamePush;
using UnityEngine;
using static Code.Progress.SaveLoadService.ISaveLoadService;

namespace Code.Progress.Writer
{
    public class GamePushProgressReadWrite : IProgressReadWrite
    {
        private readonly IStaticDataService _staticData;
        
        private float _lastSyncTime;

        public GamePushProgressReadWrite(IStaticDataService staticData)
        {
            _staticData = staticData;
        }

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
                TrySyncProgress(json);

            PlayerPrefs.SetString(PROGRESS_KEY, json);
            PlayerPrefs.Save();
        }

        public T ReadProgress<T>() where T : class
        {
            string progress = PlayerPrefs.GetString(PROGRESS_KEY);

#if !UNITY_EDITOR
            if (GP_Init.isReady && GP_Player.HasAnyCredentials() && GP_Player.Has(PROGRESS_KEY))
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

        private void TrySyncProgress(string json)
        {
            GP_Player.Set(PROGRESS_KEY, json);

            if (CheckCanSync(json))
            {
                GP_Player.Sync();
                _lastSyncTime = Time.time;
            }
        }

        private bool CheckCanSync(string json)
        {
            if (GP_Player.HasAnyCredentials() == false)
                return false;

            bool hasDiffs = json.Equals(PlayerPrefs.GetString(PROGRESS_KEY)) == false;

            if (hasDiffs)
                return false;

            if (_lastSyncTime == 0)
                return true;

            float diff = Time.time - _lastSyncTime;

            var configStaticData = _staticData.Get<BuildConfigStaticData>();
            
            if (diff > configStaticData.SyncIntervalSeconds)
                return true;

            return false;
        }
    }
}