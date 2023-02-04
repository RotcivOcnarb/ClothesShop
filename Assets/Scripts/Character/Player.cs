using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public static Player Instance;

    //Parameters
    [SerializeField] float moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] CharacterSkinRenderer skinRenderer;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] CharacterSkinRenderer mirrorSkin;

    //Internal
    private Vector2 direction;
    private float speed;
    private Rigidbody2D body;
    private List<Interactable> interactablesInRange;
    private int coins = 1000;

    private void Awake() {
        Instance = this;
        body = GetComponent<Rigidbody2D>();
        inventory.Initialize();

    }
    private void Start() {
        skinRenderer.SetSkin(inventory.GetFullEquippedAttire());
        interactablesInRange = new List<Interactable>();
    }

    public int GetCoins() {
        return coins;
    }

    public bool SpendCoins(int amount) {
        if (coins >= amount) {
            coins -= amount;
            return true;
        }
        return false;
    }

    public void AddCoins(int amount) {
        coins += amount;
    }

    public void PlaceForward() {
        direction = Vector2.down;
        speed = 1;
        Update();
        speed = 0;
    }

    public void SetSkin(SkinPiece[] skin) {
        foreach(SkinPiece sp in skin) {
            inventory.EquipPiece(sp);
        }
        skinRenderer.SetSkin(inventory.GetFullEquippedAttire());
        mirrorSkin.SetSkin(inventory.GetFullEquippedAttire());
    }

    public float GetSpeed() {
        return speed;
    }

    public Vector2 GetDirection() {
        return direction;
    }

    public PlayerInventory GetInventory() {
        return inventory;
    }

    public void EquipPiece(SkinPiece piece) {
        inventory.EquipPiece(piece);
        skinRenderer.SetSkin(inventory.GetFullEquippedAttire());
        mirrorSkin.SetSkin(inventory.GetFullEquippedAttire());
    }

    public void UnequipPiece(SkinPiece piece) {
        inventory.UnequipPiece(piece);
        skinRenderer.SetSkin(inventory.GetFullEquippedAttire());
        mirrorSkin.SetSkin(inventory.GetFullEquippedAttire());
    }

    public void Move(Vector2 input) {
        if (input.magnitude > 0) {
            direction = input.normalized;
        }
        speed = input.magnitude;
    }

    private void Update() {
        skinRenderer.ApplyToAllAnimations((animator) => {
            animator.SetFloat("DirectionX", direction.x);
            animator.SetFloat("DirectionY", direction.y);
            animator.SetBool("Walking", speed > 0);
        });
    }

    public void Interact() {
        if(interactablesInRange.Count > 0) {
            interactablesInRange[0].Interact();
        }
    }

    private void FixedUpdate() {
        Vector2 targetVelocity = direction * speed * moveSpeed;
        body.AddForce((targetVelocity - body.velocity) * acceleration, ForceMode2D.Force);
    }

    public void RegisterInteractable(Interactable interactable) {
        if (interactablesInRange.Contains(interactable)) return;
        interactablesInRange.Add(interactable);
    }

    public void UnregisterInteractable(Interactable interactable) {
        if (!interactablesInRange.Contains(interactable)) return;
        interactablesInRange.Remove(interactable);
        
    }
}
