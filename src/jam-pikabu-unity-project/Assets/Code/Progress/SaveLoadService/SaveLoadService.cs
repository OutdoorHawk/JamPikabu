using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Common;
using Code.Common.Entity;
using Code.Infrastructure.Serialization;
using Code.Progress.Data;
using Code.Progress.Provider;
using Code.Progress.Writer;
using Entitas;
using UnityEngine;
using static Code.Progress.SaveLoadService.ISaveLoadService;

namespace Code.Progress.SaveLoadService
{
    public class SaveLoadService : ISaveLoadService
    {
        private readonly IProgressProvider _progressProvider;
        private readonly IProgressReadWrite _progressReadWrite;
        private readonly GameContext _gameContext;
        private readonly MetaContext _metaContext;
        
        public static string PlayerProgressPath => Path.Combine(Application.persistentDataPath, PROGRESS_KEY);

        public bool HasSavedProgress => _progressReadWrite.HasSavedProgress();

        public SaveLoadService
        (
            IProgressProvider progressProvider,
            IProgressReadWrite readWrite,
            GameContext gameContext,
            MetaContext metaContext
        )
        {
            _gameContext = gameContext;
            _metaContext = metaContext;
            _progressProvider = progressProvider;
            _progressReadWrite = readWrite;
        }

        public void CreateProgress()
        {
            var playerProgress = new PlayerProgress();

            _progressProvider.SetProgressData(playerProgress);
        }

        public void SaveProgress()
        {
            PreserveMetaEntities();
            string progressJson = _progressProvider.Progress.ToJson();
            _progressReadWrite.WriteProgress(progressJson);
        }

        public void LoadProgress()
        {
            PlayerProgress progress = _progressReadWrite.ReadProgress<PlayerProgress>();
            _progressProvider.SetProgressData(progress);
            HydrateProgress();
        }

        private void HydrateProgress()
        {
            HydrateMetaEntities();
        }

        private void HydrateMetaEntities()
        {
            List<EntitySnapshot> snapshots = _progressProvider.EntityData.MetaEntitySnapshots;

            foreach (EntitySnapshot snapshot in snapshots)
            {
                CreateMetaEntity
                    .Empty()
                    .HydrateWith(snapshot);
            }
        }

        private void PreserveMetaEntities()
        {
            _progressProvider.EntityData.MetaEntitySnapshots = _metaContext
                .GetEntities()
                .Where(RequiresSave)
                .Select(e => e.AsSavedEntity())
                .ToList();
        }

        private static bool RequiresSave(MetaEntity e)
        {
            IComponent[] components = e.GetComponents();
            bool hasComponentToSave = components.Any(c => c is ISavedComponent);

            if (hasComponentToSave == false)
                return false;

            bool isNotDestructed = components.Any(c => c is Destructed) == false;

            if (isNotDestructed == false)
                return false;

            return true;
        }
    }
}