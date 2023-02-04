using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerInventory
{
    [SerializeField] List<SkinPiece> inventory;

    Dictionary<string, List<SkinPiece>> equippedClothes;
    Dictionary<string, int> equippedMaxAmount;

    public void Initialize() {
        equippedClothes = new Dictionary<string, List<SkinPiece>>();
        equippedMaxAmount = new Dictionary<string, int>() {
            { "Hair", 1 },
            { "Head", 1 },
            { "Skin", 1 },
            { "Upper", 1 },
            { "Lower", 1 },
            { "Accessory", -1 },
        };

        foreach(string type in equippedMaxAmount.Keys) {
            equippedClothes.Add(type, new List<SkinPiece>());
        }
        
        foreach(SkinPiece sp in inventory) {
            if (sp.skinType == SkinPiece.SkinType.Head) continue;
            if (sp.skinType == SkinPiece.SkinType.Accessory) continue;
            if(equippedClothes[sp.skinType.ToString()].Count == 0) {
                equippedClothes[sp.skinType.ToString()].Add(sp);
            }
        }

    }

    public void AddToInventory(params SkinPiece[] skin) {
        inventory.AddRange(skin);
        inventory = inventory.Distinct().ToList();
    }

    public void RemoveFromInventory(params SkinPiece[] skin) {
        foreach(SkinPiece s in skin) {
            if (inventory.Contains(s)) {
                inventory.Remove(s);
            }
        }
    }

    public List<SkinPiece> GetInventory() {
        return inventory;
    }

    public void EquipPiece(SkinPiece piece) {
        if (!inventory.Contains(piece)) return;
        string skinType = piece.skinType.ToString();
        if (!equippedMaxAmount.ContainsKey(skinType)) return;

        equippedClothes[skinType].Add(piece);

        if (equippedMaxAmount[skinType] <= 0) return;
            
        //Shifts list and lose the last one
        while(equippedClothes[skinType].Count > equippedMaxAmount[skinType]) {
            equippedClothes[skinType].RemoveAt(0);
        }
    }

    public void UnequipPiece(SkinPiece piece) {
        string skinType = piece.skinType.ToString();
        if (!equippedClothes[skinType].Contains(piece)) return;
        if (piece.skinType == SkinPiece.SkinType.Skin) return;

        equippedClothes[skinType].Remove(piece);
    }

    public SkinPiece[] GetEquippedPiece(SkinPiece.SkinType skinType) {
        if (equippedClothes == null) return new SkinPiece[0];
        if (!equippedClothes.ContainsKey(skinType.ToString())) return new SkinPiece[0];
        return equippedClothes[skinType.ToString()].ToArray();
    }

    public SkinPiece[] GetFullEquippedAttire() {
        List<SkinPiece> attire = new List<SkinPiece>();
        foreach(string k in equippedClothes.Keys) {
            attire = attire.Concat(equippedClothes[k]).ToList();
        }
        return attire.ToArray();
    }

    public PlayerInventory Clone() {
        PlayerInventory clone = new PlayerInventory();
        clone.inventory = this.inventory.ToList();
        clone.equippedMaxAmount = GameSettings.CloneDictionary(this.equippedMaxAmount);
        clone.equippedClothes = GameSettings.CloneDictionary(this.equippedClothes);

        List<string> keys = clone.equippedClothes.Keys.ToList();

        foreach(string key in keys) {
            clone.equippedClothes[key] = clone.equippedClothes[key].ToList();
        }

        return clone;

    }


}
