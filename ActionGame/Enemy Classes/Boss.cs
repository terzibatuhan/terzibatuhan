using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss : Enemy, ISetValues<BossSO>
{
    bool isAttacking;
    [SerializeField] GameObject flamePrefab;
    GameObject _flame;

    protected override void Start()
    {
        base.Start();
        _flame = Instantiate(flamePrefab, Vector3.zero, Quaternion.identity);
    }

    void Update()
    {
        if (_agent.remainingDistance > 4f) return;

        _agent.isStopped = true;

        if (_player.position.x < transform.position.x) transform.localScale = _flipped;

        else transform.localScale = _normal;

        if (transform.position.y - _player.position.y > 1 || transform.position.y - _player.position.y < -0.5f)
        {
            _agent.isStopped = false;
            _agent.SetDestination(new Vector3(transform.position.x, _player.position.y + 1, transform.position.z));
        }

        if (isAttacking) return;

        isAttacking = true;
        _animator.SetTrigger("Attack");
        Invoke(nameof(ResetState), 1.05f);
    }

    void ResetState()
    {
        isAttacking = false;
    }

    protected override void Die()
    {
        GameManager.Instance.GameOver();

        _flame.SetActive(false);

        foreach (Enemy enemy in EnemyManager.Instance.Enemies) enemy.GetComponent<Animator>().SetTrigger("Death");

        base.Die();
    }

    public void ConjureFlame()
    {
        _flame.transform.position = transform.position;
        _flame.SetActive(true);
    }

    public void SendFlameBack()
    {
        _flame.SetActive(false);
    }

    public void SetValues(BossSO bossSO)
    {
        _name = bossSO.Name;
        _health = bossSO.Health;
        _speed = bossSO.Speed;
        _damage = bossSO.Damage;
        _armor = bossSO.Armor;
        _animator.runtimeAnimatorController = bossSO.animator;
        _boxCollider.size = bossSO.BoxColliderSize;
        _boxCollider.offset = bossSO.BoxColliderOffset;
        transform.GetChild(0).localScale = bossSO.ShadowSize;
        transform.GetChild(0).localPosition = bossSO.ShadowOffset;
    }
}
