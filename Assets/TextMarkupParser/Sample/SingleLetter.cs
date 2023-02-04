using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleLetter : MonoBehaviour
{

    Vector3 originalPosition;
    TextMesh textMesh;

    public Color color;
    public float size;
    public bool wobble;
    public bool wave;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        originalPosition = transform.position;

    }

    void Update()
    {
        textMesh.color = color;
        transform.localScale = Vector3.one * size;

        if (wobble) {
            Vector3 dif = new Vector3(
                Mathf.PerlinNoise(Time.time*5f, originalPosition.x*10) * 2 - 1,
                Mathf.PerlinNoise(originalPosition.x*10, Time.time*5f) * 2 - 1,
                0
                ) * 0.2f;
            transform.position = originalPosition + dif;
        }
        else if (wave) {
            transform.position = originalPosition + new Vector3(0, Mathf.Sin(originalPosition.x*3f + Time.time*5f) * 0.2f, 0);
        }

    }
}
