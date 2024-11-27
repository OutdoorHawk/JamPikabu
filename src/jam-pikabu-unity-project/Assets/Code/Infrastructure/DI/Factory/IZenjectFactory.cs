using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Infrastructure.DI.Factory
{
    public interface IZenjectFactory
    {
        Task<GameObject> Instantiate(AssetReferenceGameObject assetReference);
        Task<GameObject> Instantiate(AssetReferenceGameObject assetReference, Transform parent);
        Task<GameObject> Instantiate(string address, Transform parent);
        GameObject InstantiateSync(AssetReferenceGameObject assetReference, Transform parent);
        void Inject(object instance);
        GameObject Instantiate(GameObject gameObject);
        GameObject Instantiate(GameObject gameObject, Transform parent);
        T Instantiate<T>(T behaviour) where T : MonoBehaviour;
        T Instantiate<T>(T behaviour, Transform parent) where T : MonoBehaviour;
        T Instantiate<T>(T behaviour, Vector3 position, Transform parent = null) where T : MonoBehaviour;
        T Instantiate<T>(T behaviour, Vector3 position, Quaternion quaternion, Transform parent = null)
            where T : MonoBehaviour;

        GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent = null);
    }
}