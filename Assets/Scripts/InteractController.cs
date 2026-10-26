using System.Collections;
using System.Linq;
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
    [SerializeField] private float maxRadioDistance = 50.0f;
    [SerializeField] private float gunShootInterval = 2.0f;
    [SerializeField] private ParticleSystem gunPS;

    private AudioSource _radioAudio;
    private float _radioVolume;
    private float _gunTimer;

    private void Awake()
    {
        gunLight.enabled = false;
        cameraLight.enabled = false;
        _radioAudio = radioItem.GetComponent<AudioSource>();
    }

    private void Update()
    {
        _gunTimer += Time.deltaTime;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            currentItem = Item.Gun;
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            currentItem = Item.Camera;
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            currentItem = Item.Radio;

        gunItem.gameObject.SetActive(currentItem == Item.Gun);
        cameraItem.gameObject.SetActive(currentItem == Item.Camera);
        radioItem.gameObject.SetActive(currentItem == Item.Radio);

        UpdateRadio();

        if (currentItem == Item.Gun && Mouse.current.leftButton.wasPressedThisFrame)
            OnInteract(currentItem);

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

    private void UpdateRadio()
    {
        var target = 0.0f;

        if (currentItem == Item.Radio)
        {
            var closestDistance = FindObjectsByType<Anomaly>()
                .Select(anomaly => Vector3.Distance(anomaly.transform.position, transform.position))
                .Prepend(float.MaxValue)
                .Min();

            if (closestDistance < maxRadioDistance)
            {
                var t = 1.0f - (closestDistance / maxRadioDistance);
                target = t * t;
            }
        }

        _radioVolume = Mathf.MoveTowards(_radioVolume, target, 2.0f * Time.deltaTime);
        _radioAudio.volume = _radioVolume;
    }

    private void OnInteract(Item item)
    {
        switch (item)
        {
            case Item.Gun when _gunTimer > gunShootInterval && Monster.Current != null:
                _gunTimer = 0.0f;
                StartCoroutine(nameof(Shoot));
                break;
            case Item.Camera: StartCoroutine(nameof(FlashCamera)); break;
        }
    }

    private IEnumerator FlashCamera()
    {
        AudioManager.Instance.PlaySound("shutter");
        cameraLight.enabled = true;
        yield return new WaitForSeconds(cameraFlashDuration);
        cameraLight.enabled = false;
    }

    private IEnumerator Shoot()
    {
        gunLight.enabled = true;
        if (Monster.Current != null)
            Monster.Current.SetVisible(true);

        AudioManager.Instance.PlaySound("shoot");
        gunPS.Play();

        yield return new WaitForSeconds(gunFlashDuration);

        var ray = Camera.main?.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (ray != null && Physics.Raycast(ray.Value, out var hit) &&
            hit.collider.TryGetComponent<Monster>(out var monster))
            monster.EndEncounter();

        gunLight.enabled = false;

        if (Monster.Current != null)
            Monster.Current.SetVisible(false);
    }
}