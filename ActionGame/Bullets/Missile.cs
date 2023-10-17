using System.Collections;
using UnityEngine;

public class Missile : Bullet
{
    protected override void Awake()
    {
        base.Awake();
        destroyOnCollision = true;
        trailTime = 0.05f;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        rb.gravityScale = 2f;
        Enemy Target = Helpers.FindClosestEnemy();
        gameObject.SetActive(Target);
        if (gameObject.activeInHierarchy) StartCoroutine(FollowClosestEnemy(Target));
    }
    void Update()
    {
        trailTime -= Time.deltaTime;

        if (trailTime > 0f) return;

        GameObject Trail = ObjectPooler.Instance.SpawnFromPool("Smoke" + smokeTrailCount.ToString(), transform.position, Quaternion.identity);
        Trail.SetActive(false);
        Trail.SetActive(true);
        trailTime = 0.02f;

        if (smokeTrailCount < 4) smokeTrailCount++;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Collision(collision.gameObject);
    }

    IEnumerator FollowClosestEnemy(Enemy Target) 
    {
        yield return new WaitForSeconds(0.5f);

        rb.gravityScale = 0f;
        Vector3 aimDirection = rb.velocity;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
        float _Speed = speed * 2;

        while (enabled)
        {
            if (!EnemyManager.Instance.Enemies.Contains(Target)) Target = Helpers.FindClosestEnemy();

            gameObject.SetActive(Target);
            if (!gameObject.activeInHierarchy) yield break;

            Vector2 Direction = (Vector2)Target.transform.position - rb.position;
            Direction.Normalize();
            float RotateAmount = Vector3.Cross(Direction, rb.velocity.normalized).z;
            rb.angularVelocity = -RotateAmount * 360f;
            _Speed += 3 * Time.deltaTime;
            rb.velocity = transform.right * _Speed;
            yield return null;
        }
    }
}
