using System.Collections;
using UnityEngine;

public class DespawnAfterTime : MonoBehaviour
{
    [SerializeField] private float timeBeforeDisabling = 2f;
    
    private void OnEnable()
    {
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(timeBeforeDisabling);
        gameObject.SetActive(false);
    }
}
