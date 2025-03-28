﻿using System;
using Code.Common.Extensions;
using Code.Meta.UI.Shop.WindowService;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.UI.Shop.Window
{
    public class ShopTabButton : MonoBehaviour
    {
        public ShopTabTypeId TabType;
        public Button Button;
        public GameObject OpenedTabIcon;
        public GameObject ClosedTabIcon;
        public GameObject LockedButtonTabIcon;
        public GameObject Pin;

        private IShopWindowService _shopWindowService;

        [Inject]
        private void Construct(IShopWindowService shopWindowService)
        {
            _shopWindowService = shopWindowService;
        }

        private void Awake()
        {
            LockedButtonTabIcon.DisableElement();
        }

        private void Start()
        {
            Button.onClick.AddListener(SelectTab);
            _shopWindowService.OnSelectionChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(SelectTab);
            _shopWindowService.OnSelectionChanged -= Refresh;
        }

        public void SetLocked()
        {
            LockedButtonTabIcon.EnableElement();
        }

        public void SetUnlocked()
        {
            LockedButtonTabIcon.DisableElement();
        }

        private void SelectTab()
        {
            _shopWindowService.SetTabSelected(TabType);
        }

        private void Refresh()
        {
            OpenedTabIcon.SetActive(_shopWindowService.SelectedTab == TabType);
            ClosedTabIcon.SetActive(_shopWindowService.SelectedTab != TabType);
        }
    }
}