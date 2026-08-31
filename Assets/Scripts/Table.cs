using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    public Transform dropPoint;

    public GameObject customerPrefab;
    public Transform spawnPoint;
    public float minSpawnDelay = 10f;
    public float maxSpawnDelay = 15f;

    private float timer;
    private float nextSpawnDelay;
    private Customer currentCustomer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetNextSpawnDelay();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCustomer != null) return;

        timer += Time.deltaTime;
        if (timer >= nextSpawnDelay)
        {
            SpawnCustomer();
            timer = 0f;
        }
    }

    private void SetNextSpawnDelay()
    {
        nextSpawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void SpawnCustomer()
    {
        Transform parent = spawnPoint != null ? spawnPoint : transform;
        GameObject customerObj = Instantiate(customerPrefab, parent.position, parent.rotation, parent);

        Vector3 parentScale = parent.lossyScale;
        customerObj.transform.localScale = new Vector3(
            1f / parentScale.x,
            1f / parentScale.y,
            1f / parentScale.z
        );

        currentCustomer = customerObj.GetComponent<Customer>();
        currentCustomer.Init(this);
    }

    public void Interact(Zombie player)
    {
    if (currentCustomer == null || !player.HasCoffee) return;

    player.Drop(dropPoint);
    currentCustomer.ReceiveCoffee();
    }

    public void CustomerLeft()
    {
        currentCustomer = null;
        timer = 0f;
        SetNextSpawnDelay();
    }
}