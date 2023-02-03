using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] UnityEvent interactAction;
    [SerializeField] Animator spriteAnimator;

    bool inRange;

    private void Update() {
        spriteAnimator.SetBool("On", inRange);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player player = collision.GetComponent<Player>();
            player.RegisterInteractable(this);
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Player player = collision.GetComponent<Player>();
            player.UnregisterInteractable(this);
            inRange = false;
        }
    }

    public void Interact() {
        interactAction?.Invoke();
    }
}
