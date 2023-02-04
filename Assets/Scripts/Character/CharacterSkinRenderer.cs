using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterSkinRenderer : MonoBehaviour
{
    [SerializeField] SkinPiece[] skins;

    private Dictionary<SkinPiece, SkinObject> instancedSkins;

    struct SkinObject {
        public SpriteRenderer renderer;
        public SpriteMask mask;
        public Animator animator;
    }

    private void Start() {

        RefreshSkin();
    }

    public SkinPiece[] GetSkin() {
        return instancedSkins.Keys.ToArray();
    }

    public void RefreshSkin() {
        for(int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        instancedSkins = new Dictionary<SkinPiece, SkinObject>();

        for (int i = 0; i < skins.Length; i++) {

            GameObject skin = new GameObject(skins[i].name, typeof(SpriteRenderer), typeof(Animator));
            skin.transform.SetParent(transform);
            skin.transform.localPosition = Vector3.zero;
            skin.layer = gameObject.layer;

            SpriteRenderer spriteRenderer = skin.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = i + 1;

            Animator animator = skin.GetComponent<Animator>();
            animator.runtimeAnimatorController = skins[i].skinAnimator;

            SkinObject so = new SkinObject() {
                animator = animator,
                renderer = spriteRenderer
            };

            if (skins[i].asSpriteMask) {
                so.mask = skin.AddComponent<SpriteMask>();
            }

            instancedSkins.Add(skins[i], so);

        }
    }

    private void Update() {
        foreach (SkinPiece sp in instancedSkins.Keys) {
            if (instancedSkins[sp].mask != null) {
                //Sync mask sprite to animation
                instancedSkins[sp].mask.sprite = instancedSkins[sp].renderer.sprite;
            }
            if(sp.skinType == SkinPiece.SkinType.Hair) {
                //Check if there is any mask object
                if(instancedSkins.Values.Where(s => s.mask != null).Count() > 0) {
                    instancedSkins[sp].renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
                else {
                    instancedSkins[sp].renderer.maskInteraction = SpriteMaskInteraction.None;
                }
            }
        }
    }


    public void ApplyToAllAnimations(Action<Animator> callback) {
        foreach(SkinPiece sp in instancedSkins.Keys) {
            callback.Invoke(instancedSkins[sp].animator);
        }
    }

    public void SetSkin(SkinPiece[] pieces) {
        Array.Sort(pieces, (a, b) => a.renderOrder - b.renderOrder);
        skins = pieces;

        RefreshSkin();
    }

    public void AddSkin(SkinPiece skin) {
        List<SkinPiece> lst = skins.ToList();
        if (!lst.Contains(skin)) {
            lst.Add(skin);
            skins = lst.ToArray();
            RefreshSkin();
        }
    }

    public void RemoveSkin(SkinPiece skin) {
        List<SkinPiece> lst = skins.ToList();
        if (lst.Contains(skin)) {
            lst.Remove(skin);
            skins = lst.ToArray();
            RefreshSkin();
        }
    }

}
