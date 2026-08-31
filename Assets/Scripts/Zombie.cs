using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;
    private Rigidbody rb;
    private Vector3 movement;

    private readonly List<IInteractable> nearbyInteractables = new();
    private GameObject heldCoffee;
    public Transform handSocket;

    public bool HasCoffee => heldCoffee != null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        movement = new Vector3(x, 0f, z).normalized;

        IInteractable interactable = GetClosestInteractable();
        if (Input.GetKeyDown(KeyCode.F) && interactable != null)
        {
            interactable.Interact(this);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        if (movement.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable)
            && !nearbyInteractables.Contains(interactable))
            nearbyInteractables.Add(interactable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
            nearbyInteractables.Remove(interactable);
    }

    private IInteractable GetClosestInteractable()
    {
        IInteractable closest = null;
        float closestDistance = float.PositiveInfinity;

        foreach (IInteractable interactable in nearbyInteractables)
        {
            Component component = interactable as Component;
            if (component == null) continue;

            Vector3 offset = component.transform.position - transform.position;
            offset.y = 0f;
            float distance = offset.sqrMagnitude;

            if (distance < closestDistance)
            {
                closest = interactable;
                closestDistance = distance;
            }
        }

        return closest;
    }

    public void PickUp(GameObject coffee)
    {
        heldCoffee = coffee;
        heldCoffee.transform.SetParent(handSocket);
        heldCoffee.transform.localPosition = Vector3.zero;
        heldCoffee.transform.localRotation = Quaternion.identity;
    }

    public void Drop(Transform dropPoint)
    {
        if (heldCoffee == null) return;
        heldCoffee.transform.SetParent(dropPoint);
        heldCoffee.transform.localPosition = Vector3.zero;
        heldCoffee.transform.localRotation = Quaternion.identity;
        heldCoffee = null;
    }
}
