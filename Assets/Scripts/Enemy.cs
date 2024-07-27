using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
/// Base Enemy Class.
/// </summary>
public class Enemy : MonoBehaviour
{
    public EnemyManager em;
    private Material mat;
    
    public float health = 100.0f;
    public bool inLight = false;

    [SerializeField] private float attackDamage = 10.0f;
    
    [Header("AI Variables")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask groundLayer, playerLayer;

    public Vector3 walkPoint;
    
    [SerializeField] private float walkPointRange;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float sightRange, attackRange;

    private Player _player;
    private bool playerInSightRange, playerInAttackRange, walkPointSet, alreadyAttacked;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        mat = GetComponent<Renderer>().material;
    }
    
    void Update()
    {

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        if (playerInSightRange && !playerInAttackRange) Chase();
        if (playerInAttackRange && playerInSightRange) Attack();
        
        if (inLight)
        {
            mat.color = Color.green;
            
            // Damage Testing
            health -= 100.0f * Time.deltaTime;
            if (health <= 0.0f) em.EnemyDied(this);
        }
        else mat.color = Color.red;
    }

    private void Patrol()
    {
        if (!walkPointSet) GetWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void GetWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            walkPointSet = true;
    }

    private void Chase()
    {
        agent.SetDestination(_player.transform.position);
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(_player.transform);

        if (!alreadyAttacked)
        {
            _player.PlayerHealth.TakeDamage(attackDamage);
            
            alreadyAttacked = true;
            StartCoroutine(ResetAttack());
        }
    }
    
    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        alreadyAttacked = false;
    }
    
    private void OnBecameVisible()
    {
        em.EnemyVisible(this);
    }
    
    private void OnBecameInvisible()
    {
        em.EnemyLeftVisibility(this);
    }
    
    // Draws sight / attack ranges
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
