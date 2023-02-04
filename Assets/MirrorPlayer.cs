using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPlayer : MonoBehaviour
{

    [SerializeField] float yAxisHeight = 2;
    [SerializeField] CharacterSkinRenderer skinRenderer;

    private void Start() {
        skinRenderer.SetSkin(Player.Instance.GetInventory().GetFullEquippedAttire());
    }

    private void Update() {


        Vector3 playerPosition = Player.Instance.transform.position;
        playerPosition.y = yAxisHeight - playerPosition.y;
        transform.position = playerPosition;

        float speed = Player.Instance.GetSpeed();
        Vector2 direction = Player.Instance.GetDirection();
        direction.y *= -1;

        skinRenderer.ApplyToAllAnimations((animator) => {
            animator.SetFloat("DirectionX", direction.x);
            animator.SetFloat("DirectionY", direction.y);
            animator.SetBool("Walking", speed > 0);
        });

    }
}
