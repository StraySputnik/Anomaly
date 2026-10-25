using System.Linq;
using UnityEngine;

public class Blocker : MonoBehaviour
{
    [SerializeField] private Anomaly[] anomaliesRequired;
    [SerializeField] private Anomaly[] anomaliesBlocking;

    private Collider _collider;
    private Renderer _renderer;
    private bool _closed = true;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        switch (_renderer.enabled)
        {
            case true when !_closed:
                AudioManager.Instance.PlaySound("jumpscare0");
                break;
            case false when _closed:
                AudioManager.Instance.PlaySound("jumpscare1");
                break;
        }

        _renderer.enabled = _closed;
        _collider.enabled = _closed;

        if (anomaliesRequired.All(a => a.IsCompleted)) _closed = false;
        if (anomaliesBlocking.All(a => a.IsCompleted)) _closed = true;
    }
}