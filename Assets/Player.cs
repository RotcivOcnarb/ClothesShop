using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //Parameters
    [SerializeField] float moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] Animator spriteAnimator;

    //Internal
    private Vector2 direction;
    private float speed;
    private Rigidbody2D body;
    

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 input) {
        if(input.magnitude > 0) {
            direction = input.normalized;
        }
        speed = input.magnitude;
    }

    private void Update() {
        if (speed > 0) {
            spriteAnimator.SetFloat("DirectionX", direction.x * speed);
            spriteAnimator.SetFloat("DirectionY", direction.y * speed);
        }
        spriteAnimator.SetBool("Walking", speed > 0);
    }

    private void FixedUpdate() {
        Vector2 targetVelocity = direction * speed * moveSpeed;
        body.AddForce((targetVelocity - body.velocity) * acceleration, ForceMode2D.Force);
    }
}