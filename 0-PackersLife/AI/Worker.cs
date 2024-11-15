using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public enum WorkerState
{
    Idle, Walking, GoesToCar, IsDriving
}

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Worker : MonoBehaviour
{
    protected PalletJack _currentCar;
    protected GroundGrid _targetGrid;

    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected AIDriver _driver;
    protected WorkerState _state;
    protected bool _isCurrentActionCompleted;
    protected WaitForSeconds _wait = new(1);

    public bool IsAvailable;

    protected void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _driver = GetComponent<AIDriver>();
        IsAvailable = true;
    }

    protected void FindNearestCar()
    {
        PalletJack nearestCar = CarHolder.Instance.PalletJacks.OrderBy(car => Vector3.Distance(car.transform.position, transform.position)).FirstOrDefault();

        _currentCar = nearestCar;

        FinishAction();
    }

    protected void GoToCar()
    {
        StartCoroutine(WalkToTarget(_currentCar.SteppingArea.position));

        _state = WorkerState.GoesToCar;
    }

    protected void EnterCar()
    {
        _driver.EnterCar(_currentCar);

        _state = WorkerState.IsDriving;

        FinishAction();
    }

    protected void DriveToTarget(Vector3 position)
    {
        StartCoroutine(_driver.MoveToTarget(position));
    }

    protected void WaitForNewAction()
    {
        StartCoroutine(WaitForOneSec());
    }

    protected IEnumerator WalkToTarget(Vector3 position)
    {
        _agent.SetDestination(position);
        _animator.SetBool("IsWalking", true);

        while (_agent.pathPending) yield return null;

        while (_agent.remainingDistance > 0.25f) yield return null;

        _agent.SetDestination(transform.position);
        _animator.SetBool("IsWalking", false);

        FinishAction();
    }

    protected IEnumerator WaitForOneSec()
    {
        yield return _wait;

        FinishAction();
    }

    public void FinishAction()
    {
        _isCurrentActionCompleted = true;
    }
}