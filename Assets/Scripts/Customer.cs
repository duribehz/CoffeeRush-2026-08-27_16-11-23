using UnityEngine;

public class Customer : MonoBehaviour
{
    public float minPatience = 10f;
    public float maxPatience = 15f;

    private Table table;
    private float patience;
    private float timer;
    private bool served;

    public void Init(Table ownerTable)
    {
        table = ownerTable;
        patience = Random.Range(minPatience, maxPatience);
    }

    void Update()
    {
        if (served) return;

        timer += Time.deltaTime;
        if (timer >= patience)
        {
            Leave();
        }
    }

    public void ReceiveCoffee()
    {
        served = true;
        Invoke(nameof(Leave), 1f);
    }

    private void Leave()
    {
        if (table.dropPoint.childCount > 0)
            Destroy(table.dropPoint.GetChild(0).gameObject);

        table.CustomerLeft();
        Destroy(gameObject);
    }
}