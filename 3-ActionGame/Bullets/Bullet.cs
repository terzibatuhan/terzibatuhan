using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public Vector3 Direction { get; set; }

    protected float speed, damage, criticalChance, lifeSteal, triggerTime, destroyTime;
    protected float trailTime = 0.1f;
    protected float hiddenTriggerTime;
    protected int smokeTrailCount = 0;
    protected bool destroyOnCollision;
    protected Rigidbody2D rb;

    readonly WaitForSeconds wait = new(3f);

    public void SetValues(float speed, float damage, float criticalChance, float lifeSteal, float triggerTime, float destroyTime)
    {
        this.speed = speed;
        this.damage = damage;
        this.criticalChance = criticalChance;
        this.lifeSteal = lifeSteal;
        this.triggerTime = triggerTime;
        this.destroyTime = destroyTime;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable()
    {
        hiddenTriggerTime = triggerTime;
        rb.velocity = Direction * speed;
        StartCoroutine(TurnOffGameObject());
    }

    protected void Collision(GameObject Collider)
    {
        if (Collider.TryGetComponent(out ITakeDamage Damagable))
        {
            if (CalculateCriticalChance())
                Damagable.TakeDamage(damage * 3);

            else Damagable.TakeDamage(damage);

            gameObject.SetActive(!destroyOnCollision);
        }
    }

    protected bool CalculateCriticalChance()
    {
        float Chance = Random.Range(0f, 100f);

        if (Chance < criticalChance) return true;
        else return false;
    }

    IEnumerator TurnOffGameObject()
    {
        yield return wait;
        gameObject.SetActive(false);
    }
}