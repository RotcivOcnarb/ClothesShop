using Rotslib.Utils;
using RotsLib.Popup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : PopupWindow
{
    [SerializeField] bool keepsSkinAfterClose;
    [SerializeField] SkinPiece.SkinType[] sellTypes;
    [SerializeField] Transform[] panelParents;
    [SerializeField] Toggle[] tabToggles;
    [SerializeField] GameObject shopSlotPrefab;
    [SerializeField] GameObject infoWindow;

    [SerializeField] Image infoThumbnailImage;
    [SerializeField] TextMeshProUGUI infoTitle;
    [SerializeField] TextMeshProUGUI infoDescription;
    [SerializeField] TextMeshProUGUI infoPrice;
    [SerializeField] Button infoPurchaseButton;
    [SerializeField] TextMeshProUGUI coinsHUD;

    public CharacterSkinRenderer skinRenderer;

    List<Dictionary<SkinPiece, ShopSlot>> instancedSlots;
    PlayerInventory previewInventory;

    Vector2 previewDirection = Vector2.down;

    SkinPiece selectedInfo;

    public void Initialize() {
        instancedSlots = new List<Dictionary<SkinPiece, ShopSlot>>();
        previewInventory = Player.Instance.GetInventory().Clone();
        previewInventory.AddToInventory(GameSettings.GetDefaultSettings().allSkins);
        skinRenderer.SetSkin(previewInventory.GetFullEquippedAttire());

        for (int i = 0; i < Enum.GetValues(typeof(SkinPiece.SkinType)).Length; i++) {
            if (!sellTypes.Contains((SkinPiece.SkinType)i)) {
                tabToggles[i].gameObject.SetActive(false);
                instancedSlots.Add(new Dictionary<SkinPiece, ShopSlot>());
                continue;
            }

            Dictionary<SkinPiece, ShopSlot> slots = new Dictionary<SkinPiece, ShopSlot>();
            SkinPiece[] skins = GameSettings.GetDefaultSettings().allSkins.Where(p => (int)p.skinType == i).ToArray();
            foreach (SkinPiece sk in skins) {
                ShopSlot slot = Instantiate(shopSlotPrefab, panelParents[i]).GetComponent<ShopSlot>();
                slot.skinPiece = sk;
                slot.parentUI = this;

                if (skinRenderer.GetSkin().Contains(sk)) {
                    slot.selected = true;
                }

                slots.Add(sk, slot);
            }
            instancedSlots.Add(slots);
        }

        OnClose += (s, a) => {
            if (keepsSkinAfterClose) {
                Player.Instance.SetSkin(previewInventory.GetFullEquippedAttire());
            }
        };
    }

    private void Update() {
        if (skinRenderer == null) return;
        skinRenderer.ApplyToAllAnimations(animator => {
            animator.SetFloat("DirectionX", previewDirection.x);
            animator.SetFloat("DirectionY", previewDirection.y);
        });

        coinsHUD.text = string.Format("<sprite=74>{0}", Player.Instance.GetCoins());

        infoWindow.SetActive(selectedInfo != null);

        if (selectedInfo != null) {
            infoThumbnailImage.sprite = selectedInfo.thumbnailIcon;
            infoTitle.text = selectedInfo.displayName;
            infoDescription.text = selectedInfo.description;
            if (Player.Instance.GetInventory().GetInventory().Contains(selectedInfo)) {
                infoPrice.text = string.Format("Owned");
                infoPurchaseButton.interactable = false;
            }
            else {
                infoPrice.text = string.Format("<sprite=74> {0}", selectedInfo.price);
                infoPurchaseButton.interactable = true; //Dps tenho q calcular moeda
            }
        }
    }
    public void TurnPreviewLeft() {
        previewDirection = previewDirection.Rotate(-90);
    }

    public void TurnPreviewRight() {
        previewDirection = previewDirection.Rotate(90);
    }

    public void RefreshLayout(SkinPiece.SkinType type) {
        int index = (int)type;
        foreach (SkinPiece sp in instancedSlots[index].Keys) {
            instancedSlots[index][sp].selected =
                skinRenderer.GetSkin().Contains(sp);
        }
    }

    public void EquipSkin(SkinPiece skin) {
        previewInventory.EquipPiece(skin);
        skinRenderer.SetSkin(previewInventory.GetFullEquippedAttire());
        selectedInfo = skin;
    }

    public void UnequipSkin(SkinPiece skin) {
        previewInventory.UnequipPiece(skin);
        skinRenderer.SetSkin(previewInventory.GetFullEquippedAttire());
        selectedInfo = skin;
    }

    public void PurchaseSelected() {
        if (selectedInfo == null) return;
        if (Player.Instance.SpendCoins(selectedInfo.price)) {
            Player.Instance.GetInventory().AddToInventory(selectedInfo);
        }
    }

    

}
