using System.Collections.Generic;
using Code.Common.Extensions;
using UnityEngine;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class MapBlock : MonoBehaviour
    {
        public RectTransform LevelsParent;

        public List<LevelButton> LevelButtons { get; private set; } = new();

        private void Awake()
        {
            LevelButtons.RefreshList(LevelsParent.GetComponentsInChildren<LevelButton>());
        }
    }
}