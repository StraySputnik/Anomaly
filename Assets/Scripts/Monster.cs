using UnityEngine;

public class Monster : MonoBehaviour
{
    private const float LoseDistance = 2.0f;

    public static Monster Current { get; private set; }

    [SerializeField] private float speed;

    private bool _ended;

    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
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

        if (!GetComponent<SpriteRenderer>().enabled)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * speed);
        }
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
        if (Current  == null || Current != this) return;
        GetComponent<Collider>().enabled = false;
        Current = null;
        _ended = true;
    }

    public void SetVisible(bool visible)
    {
        GetComponent<SpriteRenderer>().enabled = visible;
    }
}