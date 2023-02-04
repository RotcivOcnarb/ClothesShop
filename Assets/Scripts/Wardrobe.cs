using Cinemachine;
using RotsLib.Popup;
using UnityEngine;
using UnityEngine.InputSystem;

public class Wardrobe : MonoBehaviour {

    [SerializeField] PlayerInput playerInput;
    [SerializeField] CinemachineVirtualCamera wardrobeCam;

    public void OpenWardrobe() {
        PopupManager.Instance.OpenPopup("Wardrobe Window", 50, popup => {
            WardrobeUI ui = popup as WardrobeUI;
            ui.wardrobe = this;
        });
        playerInput.enabled = false;
        wardrobeCam.Priority = 15;
        Player.Instance.PlaceForward();
    }

    public void CloseWardrobe() {
        playerInput.enabled = true;
        wardrobeCam.Priority = 0;
    }
}
