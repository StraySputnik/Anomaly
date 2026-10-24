using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Anomaly : MonoBehaviour, IInteractable
{
    public bool IsCompleted { get; private set; }

    public bool IsInteractable(Item item)
    {
        if (item == Item.Camera) return !IsCompleted;
        return false;
    }

    public void Interact(Item item)
    {
        if (item == Item.Camera) IsCompleted = true;
    }
}