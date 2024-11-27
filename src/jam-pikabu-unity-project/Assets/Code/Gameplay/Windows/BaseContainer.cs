using UnityEngine;

namespace Code.Gameplay.Windows
{
    public abstract class BaseContainer : MonoBehaviour
    {
        public virtual void Initialize()
        {
            SubscribeUpdates();
        }
        
        protected virtual void SubscribeUpdates()
        {
        }

        protected virtual void Unsubscribe()
        {
        }

        public virtual void Cleanup()
        {
            Unsubscribe();
        }
    }
}