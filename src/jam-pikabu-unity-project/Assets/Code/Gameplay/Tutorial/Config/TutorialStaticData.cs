using System.Collections.Generic;
using Code.Gameplay.StaticData;
using UnityEngine;

namespace Code.Gameplay.Tutorial.Config
{
    [CreateAssetMenu(menuName = "StaticData/" + nameof(TutorialStaticData), fileName = "Tutorial")]
    public class TutorialStaticData : BaseStaticData
    {
        [SerializeField] private List<TutorialConfig> _configs;

        public List<TutorialConfig> Configs => _configs;
    }
}