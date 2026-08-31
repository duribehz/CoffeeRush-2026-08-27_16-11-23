using UnityEngine;

public class CoffeeBar : MonoBehaviour, IInteractable
{
    public GameObject coffeePrefab;

    public void Interact(Zombie player)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameFinished) return;

        GameObject coffee = Instantiate(coffeePrefab);
        player.PickUp(coffee);
    }
}
