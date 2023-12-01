using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Basic_Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform landmarkTarget;
    private Health landmarkHealth;
    private Health agentHealth;
    
    [Header("Stats")]
    public float attackDamage;
    public float attackRange = 1.5f;
    public float attackInterval = 2f;
    // Speed is handled by NavMeshAgent component atm

    private float distanceToTarget;
    private float timeSinceLastAttack = 0f;
    private bool celebrate; // Has the enemy won the game?

    private void OnEnable()
    {
        LandMark.OnDeath += StopAttacking;

        if (landmarkTarget != null)
            agent.SetDestination(landmarkTarget.position);

        agent.isStopped = false;
        timeSinceLastAttack = 0f;
    }

    private void OnDisable()
    {
        LandMark.OnDeath -= StopAttacking;
    }

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (landmarkTarget == null)
            landmarkTarget = LandMark.Instance.transform;

        agentHealth = GetComponent<Health>();
        landmarkHealth = landmarkTarget.GetComponent<Health>();
        agent.SetDestination(landmarkTarget.position); // TODO: Assign landmark as target when no other targets (like the player or defenses) are nearby
    }

    void Update()
    {
        // If the enemy is dead, stop the agent and return
        // TODO: Add indication that enemy is dead (could use ondeath event in health script)
        if (agentHealth.isDead || celebrate)
        {
            agent.isStopped = true;
            return;
        }

        // If the distance is less than the attack range, stop the agent and attack the landmark
        if (distanceToTarget < attackRange)
        {
            agent.isStopped = true;
            // Increment the attackTimer by the time elapsed since last frame
            if (timeSinceLastAttack < attackInterval)
            {
                timeSinceLastAttack += Time.deltaTime;
            }
            else
                Attack(landmarkHealth);
        }
        // Otherwise, resume the agent and reset the attack timer
        else
        {
            agent.isStopped = false;
            timeSinceLastAttack = 0f;
        }

        // Get the distance between the enemy and the landmark
        distanceToTarget = Vector3.Distance(transform.position, landmarkTarget.position);
    }

    private void Attack(Health targetToDamage)
    {
        // If the attack timer is less than the attack cooldown, return
        if (timeSinceLastAttack < attackInterval)
        {
            return;
        }

        timeSinceLastAttack = 0;

        targetToDamage.DealDamage(attackDamage);
    }

    private void StopAttacking()
    {
        celebrate = true;
    }
}
