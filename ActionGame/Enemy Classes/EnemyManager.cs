using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> Enemies = new();

    public static EnemyManager Instance;

    [SerializeField] Transform _player;

    readonly WaitForSeconds WaitSmall = new(0.2f), WaitBig = new(1.5f);

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(SetDirection());
        StartCoroutine(DistanceCheck());
    }

    IEnumerator SetDirection()
    {
        while (true)
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Move();
            }
            yield return WaitSmall;
        }
    }

    IEnumerator DistanceCheck()
    {
        while (true)
        {
            foreach (Enemy enemy in Enemies)
            {
                if (Vector3.Distance(_player.position, enemy.transform.position) > 20) EnemySpawner.Instance.SetPosition(enemy.transform);
            }
            yield return WaitBig;
        }
    }
}
