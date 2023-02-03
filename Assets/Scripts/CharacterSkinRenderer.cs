using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterSkinRenderer : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController[] skins;

    private List<Animator> instancedAnimators;

    private void Start() {

        RefreshSkin();
    }

    public void RefreshSkin() {
        for(int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        instancedAnimators = new List<Animator>();

        for (int i = 0; i < skins.Length; i++) {

            GameObject skin = new GameObject(skins[i].name, typeof(SpriteRenderer), typeof(Animator));
            skin.transform.SetParent(transform);
            skin.transform.localPosition = Vector3.zero;

            SpriteRenderer spriteRenderer = skin.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = i + 1;

            Animator animator = skin.GetComponent<Animator>();
            animator.runtimeAnimatorController = skins[i];
            instancedAnimators.Add(animator);
        }
    }


    public void ApplyToAllAnimations(Action<Animator> callback) {
        foreach(Animator anim in instancedAnimators) {
            callback.Invoke(anim);
        }
    }

    public void SetSkin(SkinPiece[] pieces) {
        Array.Sort(pieces, (a, b) => a.renderOrder - b.renderOrder);
        skins = pieces.Select(p => p.skinAnimator).ToArray();

        RefreshSkin();
    }

}
