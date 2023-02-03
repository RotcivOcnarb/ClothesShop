using UnityEngine;

[CreateAssetMenu(fileName = "New Skin", menuName = "Skin Piece")]
public class SkinPiece : ScriptableObject {

    public enum SkinType {
        Skin,
        Hair,
        Head,
        Upper,
        Lower,
        Accessory
    }

    public Sprite thumbnailIcon;
    public RuntimeAnimatorController skinAnimator;
    public SkinType skinType;
    public string displayName;
    [TextArea]
    public string description;
    public int price;
    public int renderOrder;
    public bool asSpriteMask;

}
