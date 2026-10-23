using UnityEngine;
using UnityEngine.InputSystem;

public enum Item : byte
{
    Light,
    Radio,
    Camera,
}

public interface IInteractable
{
    public void Interact(Item item);
}

public class InteractController : MonoBehaviour
{
    public Item currentItem;

    [SerializeField] private float interactDistance = 1.0f;
    [SerializeField] private MeshRenderer lightItem;
    [SerializeField] private MeshRenderer cameraItem;
    [SerializeField] private MeshRenderer radioItem;

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            currentItem = Item.Light;
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            currentItem = Item.Camera;
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            currentItem = Item.Radio;

        lightItem.gameObject.SetActive(false);
        cameraItem.gameObject.SetActive(false);
        radioItem.gameObject.SetActive(false);

        switch (currentItem)
        {
            case Item.Light: lightItem.gameObject.SetActive(true); break;
            case Item.Camera: cameraItem.gameObject.SetActive(true); break;
            case Item.Radio: radioItem.gameObject.SetActive(true); break;
        }

        if (!Keyboard.current.eKey.isPressed) return;

        var cam = Camera.main;
        if (!cam) return;
        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (!Physics.Raycast(ray, out var hit, interactDistance)) return;
        if (hit.collider.gameObject.TryGetComponent<IInteractable>(out var interactable))
            interactable.Interact(currentItem);
    }
}