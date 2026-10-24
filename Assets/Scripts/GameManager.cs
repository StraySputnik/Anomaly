using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float loseDuration = 0.5f;
    [SerializeField] private GameObject losePanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        losePanel.SetActive(false);
    }

    public void Lose()
    {
        StartCoroutine(nameof(LoseCoroutine));
    }

    private IEnumerator LoseCoroutine()
    {
        losePanel.SetActive(true);
        yield return new WaitForSeconds(loseDuration);
        losePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}