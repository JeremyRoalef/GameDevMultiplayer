using System.Collections;
using UnityEngine;

public class DestroyAfterLifetime : MonoBehaviour
{
    [SerializeField, Min(0)]
    float lifetime;

    private void Awake()
    {
        StartCoroutine(BeginLifetime());
    }

    IEnumerator BeginLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
