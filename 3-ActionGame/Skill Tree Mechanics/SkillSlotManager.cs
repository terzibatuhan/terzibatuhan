using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillSlotManager : MonoBehaviour
{
    readonly List<SkillSlot> skillSlots = new();

    public void RegisterSlot(SkillSlot slot)
    {
        if (!skillSlots.Contains(slot))
        {
            skillSlots.Add(slot);
        }
    }

    public void UnregisterSlot(SkillSlot slot)
    {
        if (skillSlots.Contains(slot))
        {
            skillSlots.Remove(slot);
        }
    }

    public SkillSlot GetSlotWithSkill(SkillSO skill)
    {
        return skillSlots.FirstOrDefault(slot => slot.AssignedSkill == skill);
    }
}