using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public SkinPiece skinPiece;
    public bool selected;

    [SerializeField] Image iconImage;
    [SerializeField] GameObject ownsObject;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] GameObject priceObject;
    [SerializeField] Color greenColor;
    [SerializeField] Color redColor;
    [SerializeField] Image priceFrame;

    public ShopUI parentUI;

    Image frameImage;

    private void Awake() {
        frameImage = GetComponent<Image>();
        frameImage.material = new Material(frameImage.material);
    }

    private void Update() {
        iconImage.sprite = skinPiece.thumbnailIcon;
        frameImage.materialForRendering.SetFloat("_HueShift", selected ? 0.2f : 0f);
        frameImage.transform.localScale = Vector3.one * (selected ? 1.1f : 1f);

        bool playerOwns = Player.Instance.GetInventory().GetInventory().Contains(skinPiece);
        ownsObject.SetActive(playerOwns);
        priceObject.SetActive(!playerOwns);
        priceText.text = string.Format("<sprite=74> {0}", skinPiece.price);

        priceFrame.color = Player.Instance.GetCoins() >= skinPiece.price ? greenColor : redColor;
    }

    public void Click() {
        if (!selected) {
            parentUI.EquipSkin(skinPiece);
        }
        else {
            parentUI.UnequipSkin(skinPiece);
        }
        parentUI.RefreshLayout(skinPiece.skinType);
    }

}
