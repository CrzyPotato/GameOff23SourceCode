using System.Collections;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    [SerializeField] private float timeBeforeDisabling = 2f;
    
    private void OnEnable()
    {
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(timeBeforeDisabling);

        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
