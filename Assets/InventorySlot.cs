using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public SkinPiece skinPiece;
    public bool equipped;
    public WardrobeUI parentUi;

    [SerializeField] Image iconImage;

    Image frameImage;

    private void Awake() {
        frameImage = GetComponent<Image>();
        frameImage.material = new Material(frameImage.material);
    }

    private void Update() {

        iconImage.sprite = skinPiece.thumbnailIcon;
        frameImage.materialForRendering.SetFloat("_HueShift", equipped ? 0.2f : 0f);
        frameImage.transform.localScale = Vector3.one * (equipped ? 1.1f : 1f);
    }

    public void Click() {
        Player.Instance.EquipPiece(skinPiece);
        parentUi.RefreshLayout(skinPiece.skinType);
    }
}
