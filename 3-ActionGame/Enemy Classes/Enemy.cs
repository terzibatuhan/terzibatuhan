using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public abstract class Enemy : MonoBehaviour, ITakeDamage
{
    // Enemy Stats
    protected string _name;
    protected float _health;
    protected float _armor;
    protected float _damage;
    protected float _speed;

    // Information of exp its dropping
    protected float _expQuantity;
    protected float _expDropRate;

    public float Damage { get { return _damage; } }

    bool _isDead;

    Transform _UIs;
    static int UICount = 0;

    protected Transform _player;

    Coroutine _takeHitRoutine;

    Material _shaderGUI, _URPdefault;
    SpriteRenderer _spriteRenderer;

    protected Rigidbody2D _rb;
    protected BoxCollider2D _boxCollider;
    protected Animator _animator;

    protected Vector3 _normal, _flipped;

    protected NavMeshAgent _agent;

    void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    protected virtual void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _UIs = GameObject.FindGameObjectWithTag("UIs").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _shaderGUI = MaterialHolder.Instance.ShaderGUI;
        _URPdefault = MaterialHolder.Instance.Matte;

        _normal = Vector3.one;
        _flipped = new Vector3(-1, 1, 1);
    }

    void OnEnable()
    {
        _isDead = false;
        _agent.isStopped = false;
        _boxCollider.enabled = true;
        _animator.Play(0, 0, Random.Range(0f, 1f));
        //transform.GetChild(0).GetComponent<Animator>().Play(0, 0, Random.Range(0f, 1f));
        EnemyManager.Instance.Enemies.Add(this);
    }
    public void TakeDamage(float PlayersDamage)
    {
        if (_isDead) return;

        // Damage Calculations With Armor
        float HitPower;
        HitPower = PlayersDamage * (100 - _armor) / 100;
        _health -= HitPower;
        //Players Life Steal Section
        _player.GetComponent<Player>().LifeSteal(HitPower / 100);

        //Damage Amount UI
        GameObject nextUI = _UIs.GetChild(UICount).gameObject;
        nextUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(HitPower.ToString("0"));
        nextUI.transform.position = transform.position + Vector3.up * Random.Range(0.25f, 0.5f) + Vector3.right * Random.Range(-0.5f, 0.5f);
        nextUI.SetActive(false);
        nextUI.SetActive(true);
        UICount++;
        if (UICount == 10) UICount = 0;

        //Take Hit Anim
        if (_takeHitRoutine != null) 
        {
            StopCoroutine(_takeHitRoutine);
        }
        _takeHitRoutine = StartCoroutine(HitAnim());

        if (_health <= 0) Die();
    }

    public void Move()
    {
        _agent.SetDestination(_player.position);

        if (_agent.velocity.x < 0f) transform.localScale = _flipped;

        else transform.localScale = _normal;
    }

    protected virtual void Die() 
    {
        _isDead = true;
        _agent.isStopped = true;
        EnemyManager.Instance.Enemies.Remove(this);
        Player.GetKill();

        float Chance = Random.Range(0f, 100f);
        if (Chance < _expDropRate)
        {
            GameObject newExpOrb = ObjectPooler.Instance.SpawnFromPool("Exp", transform.position, Quaternion.identity);
            newExpOrb.GetComponent<Experience>().ExpQuantity = _expQuantity;
            newExpOrb.SetActive(true);
            ListHolder.Instance.ExpOrbs.Add(newExpOrb.transform);
        }
        _boxCollider.enabled = false;
        _animator.SetTrigger("Death");
    }

    IEnumerator HitAnim() 
    {
        _spriteRenderer.material = _shaderGUI;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.material = _URPdefault;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        EnemyManager.Instance.Enemies.Remove(this);
        StopAllCoroutines();
    }
}