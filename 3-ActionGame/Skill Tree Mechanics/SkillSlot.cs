using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour
{
    private Image _slotImage;
    private SkillSO _assignedSkill;

    private SkillSlotManager _slotManager;
    [SerializeField] private SkillTreeData _skillTreeData;

    private void Awake()
    {
        _slotManager = FindObjectOfType<SkillSlotManager>();
        if (_slotManager) 
            _slotManager.RegisterSlot(this);
        _slotImage = GetComponent<Image>();
        _skillTreeData.ClearList();
    }

    public SkillSO AssignedSkill => _assignedSkill;

    public void AssignSkill(SkillSO skill)
    {
        if (_assignedSkill == skill)
        {
            Debug.Log("Bu skill zaten buraya atanmýþtý!");
            return;
        }

        else if (_assignedSkill) 
        {
            _skillTreeData.RemoveSkill(_assignedSkill);
        }

        SkillSlot existingSlotWithSkill = _slotManager.GetSlotWithSkill(skill);
        if (existingSlotWithSkill)
        {
            existingSlotWithSkill.DeallocateSkill();
            _skillTreeData.RemoveSkill(skill);
        }

        _assignedSkill = skill;
        _skillTreeData.AddSkill(skill);
        // Handle any logic for assigning the skill to the slot, such as visually representing the skill
        _slotImage.sprite = skill.Sprite;
        _slotImage.color = Color.white;
    }

    public void DeallocateSkill()
    {
        _slotImage.sprite = null;
        _assignedSkill = null;
    }

    private void OnDestroy()
    {
        if (_slotManager)
            _slotManager.UnregisterSlot(this);
    }
}