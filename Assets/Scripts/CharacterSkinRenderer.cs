using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkinRenderer : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController[] skins;

    private List<Animator> instancedAnimators;

    private void Start() {

        instancedAnimators = new List<Animator>();

        for(int i = 0; i < skins.Length; i++) {

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

}
