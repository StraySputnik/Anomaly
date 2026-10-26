using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float loseDuration = 1.0f;
    [SerializeField] private GameObject losePanel;
    
    private Coroutine _coroutine;

    private void Awake()
    {
        Time.timeScale = 1;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        losePanel.SetActive(false);
    }

    public void Lose()
    {
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(nameof(LoseCoroutine));
    }
    
    public void Win()
    {
        if (_coroutine != null) return;
        _coroutine = StartCoroutine(nameof(WinCoroutine));
    }

    private IEnumerator LoseCoroutine()
    {
        Time.timeScale = 0;
        AudioManager.Instance.PlaySound("monster");
        losePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(loseDuration);
        losePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        _coroutine = null;
    }
    
    private IEnumerator WinCoroutine()
    {
        Time.timeScale = 0;
        AudioManager.Instance.PlaySound("monster");
        losePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(loseDuration);
        losePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        _coroutine = null;
    }
}