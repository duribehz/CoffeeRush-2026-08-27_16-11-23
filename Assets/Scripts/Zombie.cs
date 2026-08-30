using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;
    private Vector3 movement;

    private IInteractable currentInteractable;
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

        if (Input.GetKeyDown(KeyCode.F) && currentInteractable != null)
        {
            currentInteractable.Interact(this);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
            currentInteractable = interactable;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable)
            && interactable == currentInteractable)
            currentInteractable = null;
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