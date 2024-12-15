using System.Collections.Generic;
using Code.Common.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Meta.Features.MainMenu.Behaviours
{
    public class MapContainer : MonoBehaviour
    {
        public ScrollRect MainScroll;
       
        public List<MapBlock> MapBlocks { get; private set; } = new();
        
        private void Awake()
        {
            MapBlocks.RefreshList(MainScroll.content.GetComponentsInChildren<MapBlock>());
        }
    }
}