﻿using UnityEngine;

namespace Code.Gameplay.StaticData
{
    /*public interface IStaticData
    {
    }*/

    public abstract partial class BaseStaticData : ScriptableObject
    {
        public virtual void OnConfigPreInit()
        {
        }

        public virtual void OnConfigInit()
        {
        }
    }
}