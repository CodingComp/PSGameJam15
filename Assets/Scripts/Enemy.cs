using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/// <summary>
///     Base Enemy Class.
/// </summary>
public class Enemy : MonoBehaviour
{
    public EnemyManager em;

    public float health = 100.0f;
    public bool inLight;

    [SerializeField] private float attackDamage = 10.0f;

    [Header("AI Variables")] 
    public Vector3 walkPoint;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask groundLayer, playerLayer;
    [SerializeField] private float walkPointRange;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float sightRange, attackRange;

    public Renderer renderer;
    private Player _player;
    private Material _mat;
    private bool _playerInSightRange, _playerInAttackRange, _walkPointSet, _alreadyAttacked;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        agent = GetComponent<NavMeshAgent>();
        _mat =renderer.material;
    }

    private void Update()
    {
        _playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        _playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!_playerInSightRange && !_playerInAttackRange) Patrol();
        if (_playerInSightRange && !_playerInAttackRange) Chase();
        if (_playerInAttackRange && _playerInSightRange) Attack();

        if (inLight)
        {
            _mat.color = Color.green;

            // Damage Testing
            health -= 100.0f * Time.deltaTime;
            if (health <= 0.0f) em.EnemyDied(this);
        }
        else _mat.color = Color.red;
    }

    private void OnBecameInvisible()
    {
        em.EnemyLeftVisibility(this);
    }

    private void OnBecameVisible()
    {
        em.EnemyVisible(this);
    }

    // Draws sight / attack ranges
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void Patrol()
    {
        if (!_walkPointSet) GetWalkPoint();

        if (_walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) _walkPointSet = false;
    }

    private void GetWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
            _walkPointSet = true;
    }

    private void Chase()
    {
        agent.SetDestination(_player.transform.position);
    }

    private void Attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(_player.transform);

        if (!_alreadyAttacked)
        {
            _player.playerHealth.TakeDamage(attackDamage);

            _alreadyAttacked = true;
            StartCoroutine(ResetAttack());
        }
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        _alreadyAttacked = false;
    }
}
