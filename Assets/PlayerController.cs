using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    //Separates input from actual movement code to better maintenance

    [SerializeField] Player player;

    public void ActionMove(InputAction.CallbackContext context) {
        Vector2 input = context.ReadValue<Vector2>();
        player.Move(input);
    }
}
