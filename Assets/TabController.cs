using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [SerializeField] Toggle[] toggles;
    [SerializeField] GameObject[] panels;

    private void Update() {
        
        for(int i = 0; i < panels.Length; i++) {
            panels[i].SetActive(toggles[i].isOn);
        }

    }
}
