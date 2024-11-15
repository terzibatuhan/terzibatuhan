using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingEntity
{
    [Header("Detection")]
    private float _detectionInterval;
    private float _nextDetectionTime;

    [SerializeField] private float _viewAngle = 45f;
    [SerializeField] private float _viewDistance = 10f;
    [SerializeField] private float _maxChaseDistance = 15f;
    [SerializeField] private LayerMask _layerToDetect;

    private Transform _currentTarget;

    private Coroutine _turnCoroutine;

    [Header("Attacking")]
    [SerializeField] private float _attackInterval = 1.5f; // Time between attacks
    [SerializeField] private int _damageAmount = 10; // Damage amount per attack

    protected override void Start()
    {
        base.Start();

        _detectionInterval = 0.25f;
        _nextDetectionTime = 0f;
    }

    protected override void Update()
    {
        base.Update();

        if (isServer)
        {
            DetectEntities();
        }

        else
        {

        }
    }

    [ServerCallback]
    private void DetectEntities()
    {
        // Don't detect anymore if have any target
        if (_currentTarget != null)
            return;

        // Check if it's time to perform the next detection
        if (Time.time < _nextDetectionTime)
            return;

        // Update the time for the next detection
        _nextDetectionTime = Time.time + _detectionInterval;

        // Get all colliders within the detection radius
        Collider[] entitiesInView = Physics.OverlapSphere(transform.position, _viewDistance, _layerToDetect);

        foreach (Collider entity in entitiesInView)
        {
            Vector3 directionToEntity = (entity.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToEntity);

            // Check if the entity is within the field of view
            if (angleToPlayer < _viewAngle / 2)
            {
                // Perform a raycast to check if the entity is visible (not obstructed)
                if (!Physics.Linecast(transform.position + (Vector3.up * 0.5f), entity.transform.position + (Vector3.up * 0.5f)))
                {
                    DetectEntity(entity);
                    return; // Detect the entity and exit the loop
                }
            }
            else
            {
                if (!entity.TryGetComponent(out FPSController controller))
                    return;

                if (!controller.IsGrounded || controller.IsSprinting)
                {
                    // If a previous turn coroutine is running, stop it
                    if (_turnCoroutine != null)
                    {
                        //StopCoroutine(_turnCoroutine);
                    }

                    // Start the coroutine to turn towards the entity

                    StopAllCoroutines();
                    _turnCoroutine = StartCoroutine(TurnTowardsEntity(directionToEntity));
                }
            }
        }
    }

    [ServerCallback]
    private void DetectEntity(Collider entity)
    {
        Debug.Log(name + " detected: " + entity.name);

        _currentTarget = entity.transform;

        // Stop patrolling and start chasing
        StopAllCoroutines();

        StartCoroutine(ChaseEntity());
    }

    [ServerCallback]
    private IEnumerator ChaseEntity()
    {
        ChangeBehaviour(EntityBehaviour.Run);
        _agent.isStopped = false;

        while (true) // Run indefinitely until manually stopped
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _currentTarget.position);

            // Check the distance from the player
            if (distanceToPlayer > _maxChaseDistance)
            {
                _agent.isStopped = true; // Immediately stop the agent
                _currentTarget = null;
                ChangeBehaviour(EntityBehaviour.Idle);

                if (_defaultBehaviour == EntityBehaviour.Idle)
                {
                    // Return to spawn point if default behavior is Idle
                    StartCoroutine(MoveToTarget(_spawnPosition, 1f, () => ChangeBehaviour(EntityBehaviour.Idle)));
                }
                else
                {
                    // Continue patrolling if the default behavior is 'Walk'
                    Patrol();
                }

                // Exit the coroutine immediately
                yield break;
            }

            // Continue chasing the player
            _agent.SetDestination(_currentTarget.position);

            // Check the distance to the target player
            if (distanceToPlayer <= 3f)
            {
                _agent.isStopped = true; // Stop chasing
                _agent.velocity = Vector3.zero; // Stop any residual velocity
                ChangeBehaviour(EntityBehaviour.Attack);
                StartCoroutine(Attack());
                yield break; // Stop chasing when close enough
            }

            yield return null; // Wait until the next frame before repeating the loop
        }
    }

    private IEnumerator Attack()
    {
        // Check if the target has a health component
        if (_currentTarget.TryGetComponent(out HealthSystem targetHealth))
        {
            float attackTimer = 1f; // Timer to keep track of attack intervals

            while (true) // Infinite loop to keep attacking
            {
                // Check distance to target; stop if target is too far
                if (Vector3.Distance(transform.position, _currentTarget.transform.position) > 5f)
                {
                    // If the target moves away, chase again
                    StartCoroutine(ChaseEntity());
                    yield break; // End the attack coroutine
                }

                // Update the attack timer
                attackTimer += Time.deltaTime;

                // If enough time has passed since the last attack, attack
                if (attackTimer >= _attackInterval)
                {
                    // Deal damage to the player
                    //targetHealth.TakeDamage(_damageAmount);
                    Debug.Log($"{name} attacked {_currentTarget.name} for {_damageAmount} damage.");

                    // Reset the timer
                    attackTimer = 0f;
                }

                yield return null; // Wait until the next frame before repeating the loop
            }
        }

        else
        {
            Debug.LogWarning($"{_currentTarget.name} has no health component.");
        }
    }

    [ServerCallback]
    private IEnumerator TurnTowardsEntity(Vector3 directionToEntity)
    {
        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToEntity);

        // Continue rotating until the enemy faces the entity
        while (Vector3.Angle(transform.forward, directionToEntity) > 0.5f)
        {
            // Smoothly rotate towards the target direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            yield return null; // Wait for the next frame
        }

        Debug.Log("Enemy detected something behind and turned to look!");
    }

    private void OnDrawGizmos()
    {
        // Gizmo color based on entity behavior
        Gizmos.color = (_currentBehaviour == EntityBehaviour.Run) ? Color.red : Color.green;

        Gizmos.DrawWireSphere(transform.position, _viewDistance);

        // Field of view lines
        Vector3 leftViewAngle = Quaternion.Euler(0, -_viewAngle / 2, 0) * transform.forward;
        Vector3 rightViewAngle = Quaternion.Euler(0, _viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftViewAngle * _viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightViewAngle * _viewDistance);
    }
}