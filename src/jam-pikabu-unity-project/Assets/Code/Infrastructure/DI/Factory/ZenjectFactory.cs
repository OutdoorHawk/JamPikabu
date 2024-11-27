using System.Threading.Tasks;
using Code.Infrastructure.AssetManagement.AssetProvider;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Code.Infrastructure.DI.Factory
{
    /// <summary>
    /// Used for instantiate objects that need an injection (spawn in DontDestroyOnLoad if parent is null)
    /// </summary>
    public class ZenjectFactory : IZenjectFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly DiContainer _container;

        public ZenjectFactory(IAssetProvider assetProvider, DiContainer container)
        {
            _assetProvider = assetProvider;
            _container = container;
        }

        public async Task<GameObject> Instantiate(AssetReferenceGameObject assetReference)
        {
            GameObject loaded = await _assetProvider.Load<GameObject>(assetReference);
            return _container.InstantiatePrefab(loaded);
        }

        public async Task<GameObject> Instantiate(AssetReferenceGameObject assetReference, Transform parent)
        {
            GameObject loaded = await _assetProvider.Load<GameObject>(assetReference);
            return _container.InstantiatePrefab(loaded, parent);
        }

        public async Task<GameObject> Instantiate(string address, Transform parent)
        {
            GameObject loaded = await _assetProvider.Load<GameObject>(address);
            return _container.InstantiatePrefab(loaded, parent);
        }

        public GameObject InstantiateSync(AssetReferenceGameObject assetReference, Transform parent)
        {
            AsyncOperationHandle<GameObject> asyncOperationHandle =
                Addressables.LoadAssetAsync<GameObject>(assetReference);
            
            GameObject loaded = asyncOperationHandle.WaitForCompletion();
            return _container.InstantiatePrefab(loaded, parent);
        }
        
        public void Inject(object instance)
        {
            _container.Inject(instance);
        }

        public GameObject Instantiate(GameObject gameObject)
        {
            return _container.InstantiatePrefab(gameObject);
        }

        public GameObject Instantiate(GameObject gameObject, Transform parent)
        {
            return _container.InstantiatePrefab(gameObject, parent);
        }  
        
        public GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return _container.InstantiatePrefab(gameObject, position, rotation, parent);
        }

        public T Instantiate<T>(T behaviour) where T : MonoBehaviour
        {
            return _container.InstantiatePrefab(behaviour).GetComponent<T>();
        }

        public T Instantiate<T>(T behaviour, Transform parent) where T : MonoBehaviour
        {
            return _container.InstantiatePrefab(behaviour, parent).GetComponent<T>();
        }

        public T Instantiate<T>(T behaviour, Vector3 position, Transform parent = null) where T : MonoBehaviour =>
            _container.InstantiatePrefab(behaviour, position, Quaternion.identity, parent).GetComponent<T>();

        public T Instantiate<T>(T behaviour, Vector3 position, Quaternion quaternion, Transform parent = null)
            where T : MonoBehaviour =>
            _container.InstantiatePrefab(behaviour, position, quaternion, parent).GetComponent<T>();
    }
}