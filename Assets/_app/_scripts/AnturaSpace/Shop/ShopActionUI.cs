﻿using Antura.Audio;
using Antura.Core;
using Antura.Database;
using Antura.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.AnturaSpace
{
    public class ShopActionUI : MonoBehaviour
    {
        public Image iconUI;
        public TextMeshProUGUI amountUI;
        public UIButton buttonUI;

        private ShopAction shopAction;

        public void SetAction(ShopAction shopAction)
        {
            this.shopAction = shopAction;
            iconUI.sprite = shopAction.iconSprite;
            amountUI.text = shopAction.bonesCost.ToString();
            UpdateAction();
        }

        public void UpdateAction()
        {
            bool isLocked = shopAction.IsLocked;
            buttonUI.Lock(isLocked);
        }

        public void OnClick()
        {
            if (ShopDecorationsManager.I.ShopContext == ShopContext.Purchase)
            {
                if (!shopAction.IsLocked)
                {
                    shopAction.PerformAction();
                }
                else
                {
                    ErrorFeedback();
                }
            }
        }

        public void OnDrag()
        {
            if (ShopDecorationsManager.I.ShopContext == ShopContext.Purchase)
            {
                if (!shopAction.IsLocked)
                {
                    shopAction.PerformDrag();
                }
                else
                {
                    ErrorFeedback();
                }
            }
        }

        void ErrorFeedback()
        {
            AudioManager.I.PlaySound(Sfx.KO);

            if (shopAction.NotEnoughBones)
            {
                // TODO: change this
                AudioManager.I.PlayDialogue(LocalizationDataId.ReservedArea_SectionDescription_Error);
            }
            else
            {
                AudioManager.I.PlayDialogue(shopAction.errorLocalizationID);
            }
        }

    }
}