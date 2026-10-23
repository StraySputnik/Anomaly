using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Anomaly : MonoBehaviour, IInteractable
{
    public bool IsCompleted { get; private set; }

    public void Interact(Item item)
    {
        if (item == Item.Camera)
        {
            IsCompleted = true;
        }
    }
}
