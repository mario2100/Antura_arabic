﻿using Antura.Core;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopActionsManager : MonoBehaviour
    {
        [Header("Debug")]
        public bool addDebugBones = false;

        public ShopActionsPanelUI ShopActionsPanelUi;
        public ShopDecorationsManager ShopDecorationsManager;

        private ShopAction[] shopActions;

        void Start()
        {
            if (addDebugBones) {
                AppManager.I.Player.AddBones(50);
            }

            // Setup the decorations manager
            var shopState = AppManager.I.Player.CurrentShopState;
            ShopDecorationsManager.Initialise(shopState);

            // Setup actions
            shopActions = GetComponentsInChildren<ShopAction>();
            foreach (var shopAction in shopActions) {
                shopAction.InitialiseLockedState();
            }
            ShopActionsPanelUi.SetActions(shopActions);

        }

    }
}