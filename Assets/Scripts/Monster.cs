using UnityEngine;

public class Monster : MonoBehaviour
{
    private const float LoseDistance = 2.0f;

    public static Monster Current { get; private set; }

    [SerializeField] private float speed;
    [SerializeField] private float footstepInterval = 0.25f;
    [SerializeField] private bool alwaysVisible;

    private bool _ended;
    private float _footstepTimer;

    private void Start()
    {
        if (!alwaysVisible)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

        GetComponent<Collider>().enabled = false;
    }

    private void Update()
    {
        if (Current != this) return;

        var player = GameObject.FindWithTag("Player");
        var dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist < LoseDistance)
        {
            GameManager.Instance.Lose();
            return;
        }

        if (GetComponent<SpriteRenderer>().enabled) return;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);

        _footstepTimer += Time.deltaTime;
        if (_footstepTimer < footstepInterval) return;
        _footstepTimer -= footstepInterval;
        GetComponent<AudioSource>().Play();
    }

    public void BeginEncounter()
    {
        if (Current != null) return;
        if (_ended) return;
        GetComponent<Collider>().enabled = true;
        Current = this;
    }

    public void EndEncounter()
    {
        if (Current == null || Current != this) return;

        AudioManager.Instance.PlaySound("breath");
        GetComponent<Collider>().enabled = false;
        SetVisible(false);
        Current = null;
        _ended = true;
    }

    public void SetVisible(bool visible)
    {
        if (alwaysVisible) return;
        GetComponent<SpriteRenderer>().enabled = visible;
    }
}