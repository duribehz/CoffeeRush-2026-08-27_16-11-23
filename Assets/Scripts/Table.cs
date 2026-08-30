using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    public Transform dropPoint;
    public void Interact(Zombie player)
    {
        player.Drop(dropPoint);
    }
}