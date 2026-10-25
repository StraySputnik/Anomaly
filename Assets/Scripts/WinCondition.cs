using System.Linq;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (FindObjectsByType<Anomaly>().Any(a => !a.IsCompleted)) return;
        GameManager.Instance.Win();
    }
}
