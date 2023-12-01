using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowableProj : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject impactVFX;
    [SerializeField] private GameObject destroyVFX;

    [Header("Stats")]
    public float damage;
    public float damageRadius = 5f;
    [Tooltip("Amount of times the projectile will bounce before being destroyed")] public int amtOfBounces;
    [Tooltip("Projectile's lifetime (DEPENDS ON THROWABLE TYPE)")]  public float lifeTime;
    private int timesBounced;

    public LayerMask damageLayers;
    public ThrowableType projType;

    private void OnEnable()
    {
        timesBounced = 0;
    }

    private void OnDisable()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        timesBounced++;

        if (projType == ThrowableType.Boulder)
        {
            ObjectPoolManager.SpawnObject(impactVFX, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObject);

            Vector3 radiusDamagePos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(radiusDamagePos, damageRadius, damageLayers);
            int i = 0;
            while (i < colliders.Length)
            {
                Collider hit = colliders[i];
                hit.GetComponent<Health>().DealDamage(damage);
                Debug.Log(hit.name);
                i++;
            }

            if (timesBounced >= amtOfBounces)
            {
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }
        }
        else if (projType == ThrowableType.Tree)
            StartCoroutine(DespawnAfterTime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (projType == ThrowableType.Tree)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<Health>().DealDamage(damage);
            }
        }
    }

    private IEnumerator DespawnAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolManager.SpawnObject(destroyVFX, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObject);
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
