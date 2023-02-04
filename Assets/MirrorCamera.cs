using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MirrorCamera : MonoBehaviour
{
    [SerializeField] RawImage imageDisplay;

    Camera mainCamera;
    Camera myCamera;

    RenderTexture renderTarget;

    private void Start() {
        mainCamera = Camera.main;
        myCamera = GetComponent<Camera>();
        renderTarget = new RenderTexture(Screen.width, Screen.height, 24);
        renderTarget.Create();
        myCamera.targetTexture = renderTarget;
        imageDisplay.texture = renderTarget;
    }

    private void LateUpdate() {
        transform.position = mainCamera.transform.position;
        myCamera.orthographicSize = mainCamera.orthographicSize;
    }
}
