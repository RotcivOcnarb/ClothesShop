using Rotslib.Transition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoad : MonoBehaviour
{
    [SerializeField] Transition defaultTransition;

    private void Awake() {
        Transition.defaultTransitionPrefab = defaultTransition;
    }
}
