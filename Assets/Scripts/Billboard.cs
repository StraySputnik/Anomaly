using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(GameObject.FindWithTag("Player").transform.position - transform.position,  Vector3.up);
    }
}
