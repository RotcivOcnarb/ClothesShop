using RotsLib.Popup;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WardrobeUI : PopupWindow {

    [SerializeField] Transform[] panelParents;
    [SerializeField] GameObject inventorySlotPrefab;
    public Wardrobe wardrobe;

    List<Dictionary<SkinPiece, InventorySlot>> instancedSlots;

    private void Start() {
        instancedSlots = new List<Dictionary<SkinPiece, InventorySlot>>();

        for (int i = 0; i < Enum.GetValues(typeof(SkinPiece.SkinType)).Length; i++) {
            Dictionary<SkinPiece, InventorySlot> slots = new Dictionary<SkinPiece, InventorySlot>();
            SkinPiece[] skins = Player.Instance.GetInventory().GetInventory().Where(p => (int)p.skinType == i).ToArray();
            foreach(SkinPiece sk in skins) {
                InventorySlot slot = Instantiate(inventorySlotPrefab, panelParents[i]).GetComponent<InventorySlot>();
                slot.skinPiece = sk;
                slot.parentUi = this;
                if (Player.Instance.GetInventory().GetEquippedPiece((SkinPiece.SkinType) i).Contains(sk)) {
                    slot.equipped = true;
                }
                slots.Add(sk, slot);
            }
            instancedSlots.Add(slots);
        }
    }

    public void RefreshLayout(SkinPiece.SkinType type) {
        int index = (int)type;
        foreach(SkinPiece sp in instancedSlots[index].Keys) {
            instancedSlots[index][sp].equipped =
                Player.Instance.GetInventory().GetEquippedPiece(type).Contains(sp);
        }
    }

    public void CloseWardrobe() {
        wardrobe.CloseWardrobe();
        ClosePopup();
    }
}
