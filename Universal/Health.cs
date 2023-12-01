using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Variables")]
    public float maxHealth;
    [SerializeField] private float currentHealth;
    public bool isDead;
    public bool isLandmark;

    [Header("References")]
    [SerializeField] private Image healthBarSprite;
    [SerializeField] private GameObject deathFX;
    private WaveManager waveManager;

    public UnityEvent OnDeath;

    private void Start()
    {
        waveManager = WaveManager.Instance;
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        UpdateHealthBar(maxHealth, currentHealth);
        isDead = false;
    }

    private void OnDisable()
    {
        
    }

    public void DealDamage(float damageToDeal)
    {
        currentHealth -= damageToDeal;

        if (currentHealth <= 0 && !isDead)
        {
            if (deathFX != null)
                ObjectPoolManager.SpawnObject(deathFX, transform.position, Quaternion.identity, ObjectPoolManager.PoolType.GameObject);

            waveManager.enemiesAlive--;
            currentHealth = 0;
            isDead = true;
            OnDeath.Invoke();
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }

        UpdateHealthBar(maxHealth, currentHealth);
    }

    private void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        healthBarSprite.fillAmount = currentHealth / maxHealth;
    }
}
