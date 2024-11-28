using UnityEngine;

namespace Code.Gameplay.StaticData
{
    public interface IStaticData
    {
    }

    public abstract class BaseStaticData : ScriptableObject, IStaticData
    {
        public virtual void OnConfigPreInit()
        {
            
        }

        public virtual void OnConfigInit()
        {
            
        }
    }
}