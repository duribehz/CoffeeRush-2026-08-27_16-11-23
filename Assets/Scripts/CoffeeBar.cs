using UnityEngine;

public class CoffeeBar : MonoBehaviour, IInteractable
{
    public GameObject coffeePrefab;

    public void Interact(Zombie player)
    {
        GameObject coffee = Instantiate(coffeePrefab);
        player.PickUp(coffee);
    }
}