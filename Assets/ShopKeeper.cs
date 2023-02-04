using RotsLib.Popup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] DialogObject dialog;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Cinemachine.CinemachineVirtualCamera shopCam;
    [SerializeField] CharacterSkinRenderer skinPreview;

    public void OpenDialog() {
        playerInput.enabled = false;
        PopupManager.Instance.OpenPopup("DialogPopup", 50, popup => {
            DialogPopup dp = popup as DialogPopup;
            dp.OpenDialog(dialog);
            dp.OnClose += (sender, args) => {
                shopCam.Priority = 15;
                skinPreview.gameObject.SetActive(true);
                PopupManager.Instance.OpenPopup("Shop UI", 50, popup => {
                    ShopUI ui = popup as ShopUI;
                    ui.skinRenderer = skinPreview;
                    ui.OnClose += (s, a) => CloseDialog();
                    ui.Initialize();
                });
            };
        }); 
    }

    public void CloseDialog() {
        playerInput.enabled = true;
        shopCam.Priority = 0;
        skinPreview.gameObject.SetActive(false);
    }
}
