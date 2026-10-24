using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Item : byte
{
    Gun,
    Radio,
    Camera,
}

public interface IInteractable
{
    public bool IsInteractable(Item item);
    public void Interact(Item item);
}

public class InteractController : MonoBehaviour
{
    public Item currentItem;

    [SerializeField] private float interactDistance = 1.0f;
    [SerializeField] private MeshRenderer gunItem;
    [SerializeField] private MeshRenderer cameraItem;
    [SerializeField] private MeshRenderer radioItem;

    [SerializeField] private Light gunLight;
    [SerializeField] private Light cameraLight;
    [SerializeField] private float gunFlashDuration = 0.1f;
    [SerializeField] private float cameraFlashDuration = 0.1f;

    private void Awake()
    {
        gunLight.enabled = false;
        cameraLight.enabled = false;
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            currentItem = Item.Gun;
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            currentItem = Item.Camera;
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            currentItem = Item.Radio;

        gunItem.gameObject.SetActive(false);
        cameraItem.gameObject.SetActive(false);
        radioItem.gameObject.SetActive(false);

        switch (currentItem)
        {
            case Item.Gun: gunItem.gameObject.SetActive(true); break;
            case Item.Camera: cameraItem.gameObject.SetActive(true); break;
            case Item.Radio: radioItem.gameObject.SetActive(true); break;
        }

        if (currentItem == Item.Gun && Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnInteract(currentItem);
        }

        var cam = Camera.main;
        if (!cam) return;
        var ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (!Physics.Raycast(ray, out var hit, interactDistance)) return;
        if (!hit.collider.gameObject.TryGetComponent<IInteractable>(out var interactable)) return;
        if (!interactable.IsInteractable(currentItem)) return;

        if (!Keyboard.current.eKey.isPressed) return;

        interactable.Interact(currentItem);
        OnInteract(currentItem);
    }

    private void OnInteract(Item item)
    {
        switch (item)
        {
            case Item.Gun: StartCoroutine(nameof(Shoot)); break;
            case Item.Camera: StartCoroutine(nameof(FlashCamera)); break;
        }
    }

    private IEnumerator FlashCamera()
    {
        cameraLight.enabled = true;
        yield return new WaitForSeconds(cameraFlashDuration);
        cameraLight.enabled = false;
    }

    private IEnumerator Shoot()
    {
        gunLight.enabled = true;
        Monster.Current?.SetVisible(true);
        yield return new WaitForSeconds(gunFlashDuration);

        var ray = Camera.main?.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (ray != null && Physics.Raycast(ray.Value, out var hit) &&
            hit.collider.gameObject.TryGetComponent<Monster>(out var monster))
            monster.EndEncounter();

        gunLight.enabled = false;
        Monster.Current?.SetVisible(false);
    }
}