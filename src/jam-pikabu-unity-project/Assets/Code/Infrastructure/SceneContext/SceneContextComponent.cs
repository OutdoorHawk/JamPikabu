using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.SceneContext
{
    public class SceneContextComponent : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _enemySpawnPointsParent;
        [SerializeField] private Transform _generatorsParent;

        private readonly List<Transform> SpawnPoints = new(5);
        
        public Transform PlayerSpawnPoint => _playerSpawnPoint;

        public List<Transform> EnemySpawnPoints => GetSpawnPoints();

        public Transform GeneratorsParent => _generatorsParent;

        [Inject]
        private void Construct(ISceneContextProvider sceneContextProvider)
        {
            sceneContextProvider.SetPlayerSpawnPoint(PlayerSpawnPoint);
            sceneContextProvider.SetComponent(this);
        }

        private List<Transform> GetSpawnPoints()
        {
            if (SpawnPoints.Count != 0)
                return SpawnPoints;
            
            for (int i = 0; i < _enemySpawnPointsParent.childCount; i++) 
                SpawnPoints.Add(_enemySpawnPointsParent.GetChild(i));
            
            return SpawnPoints;
        }
    }
}