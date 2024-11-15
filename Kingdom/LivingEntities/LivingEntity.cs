using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public enum EntityBehaviour
{
    Idle,
    Walk,
    Run,
    Attack
}

public class LivingEntity : NetworkBehaviour
{
    protected NavMeshAgent _agent;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected EntityBehaviour _defaultBehaviour;

    protected EntityBehaviour _currentBehaviour;

    [Header("Patrolling")]
    protected Vector3 _spawnPosition;
    private Vector3 _targetPoint;
    [SerializeField] private Transform[] _patrollingPoints;

    protected Dictionary<EntityBehaviour, float> _speedPerBehaviour;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        _speedPerBehaviour = new()
        {
            { EntityBehaviour.Idle, 0f },
            { EntityBehaviour.Walk, 3.5f},
            { EntityBehaviour.Run, 7f},
            { EntityBehaviour.Attack, 0f}
        };

        ChangeBehaviour(_defaultBehaviour);

        if (_currentBehaviour == EntityBehaviour.Walk)
            Patrol(); // Start patrolling if the entity is supposed to walk

        else if (_currentBehaviour == EntityBehaviour.Idle)
            _spawnPosition = transform.position; // Save the initial spawn position if idle
    }

    protected virtual void Update()
    {
        
    }

    [ServerCallback]
    protected void Patrol()
    {
        // Select a random point from the patrolling points
        _targetPoint = _patrollingPoints[Random.Range(0, _patrollingPoints.Length)].position;

        // Set the agent to move towards the target
        _agent.isStopped = false; // Ensure the agent is not stopped

        // Start coroutine to move to the target and continue patrolling
        StartCoroutine(MoveToTarget(_targetPoint, 1f, Patrol));
    }

    [ServerCallback]
    protected IEnumerator MoveToTarget(Vector3 target, float stoppingDistance, System.Action onReachTarget = null)
    {
        _agent.SetDestination(target);
        _agent.isStopped = false;
        ChangeBehaviour(EntityBehaviour.Walk);

        while (true)
        {
            // Wait until the path calculation is complete
            if (_agent.pathPending)
            {
                yield return null;
                continue;
            }

            // Handle cases where the path is invalid or incomplete
            if (_agent.path.status == NavMeshPathStatus.PathInvalid || _agent.path.status == NavMeshPathStatus.PathPartial)
            {
                yield return null;
                continue;
            }

            // If the agent is close enough to the target, execute the callback
            if (_agent.remainingDistance <= stoppingDistance)
            {
                onReachTarget?.Invoke();
                yield break;
            }

            yield return null;
        }
    }

    [ServerCallback]
    protected void ChangeBehaviour(EntityBehaviour behaviour)
    {
        _currentBehaviour = behaviour;
        _animator.SetFloat("Speed", _speedPerBehaviour[behaviour]);
        _agent.speed = _speedPerBehaviour[behaviour];
    }
}