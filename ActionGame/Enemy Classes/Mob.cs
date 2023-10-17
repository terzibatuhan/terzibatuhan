using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : Enemy, ISetValues<MobSO>
{
    public void SetValues(MobSO mobSO)
    {
        _name = mobSO.Name;
        _health = mobSO.Health;
        _agent.speed = mobSO.Speed;
        _damage = mobSO.Damage;
        _armor = mobSO.Armor;
        _expQuantity = mobSO.ExpQuantity;
        _expDropRate = mobSO.ExpDropRate;
        _animator.runtimeAnimatorController = mobSO.animator;
        _boxCollider.size = mobSO.BoxColliderSize;
        _boxCollider.offset = mobSO.BoxColliderOffset;
        transform.GetChild(0).localPosition = mobSO.ShadowOffset;
    }
}
