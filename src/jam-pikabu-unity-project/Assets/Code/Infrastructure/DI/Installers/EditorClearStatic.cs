using UnityEngine;

namespace Code.Infrastructure.DI.Installers
{
    public class EditorClearStatic : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            Contexts.sharedInstance = null;
        }
    }
}