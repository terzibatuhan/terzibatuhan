using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
[System.Serializable]
public class SkillSO : ScriptableObject
{
    public enum SkillType {DarkMagic, Fire, Holy}
    public Sprite Sprite;
    public SkillType skillType;
    public string Name;
    public string Description;
    public int RequiredLevel;
    public SkillSO[] Prerequisites;
    public bool IsPassive;
    public bool IsUnlocked;

    [Header("Skill")]
    public UpgradePool UpgradePool;
}