using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    //Separates input from actual movement code to better maintenance

    [SerializeField] Player player;

    public void ActionMove(InputAction.CallbackContext context) {
        if (!enabled) return;

        Vector2 input = context.ReadValue<Vector2>();
        player.Move(input);
    }

    public void ActionInteract(InputAction.CallbackContext context) {
        if (!enabled) return;
        if (context.performed) {
            player.Interact();
        }
    }
}
