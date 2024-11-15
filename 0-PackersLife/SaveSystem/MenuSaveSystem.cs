using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSaveSystem : MonoBehaviour
{
    [SerializeField] private SaveSlot[] _slots;

    void Start()
    {
        foreach (var slot in _slots) 
        {
            InitializeSlot(slot);
        }
    }

    void InitializeSlot(SaveSlot slot)
    {
        if (!PlayerPrefs.HasKey(slot.Slot.ToString() + "GeneralData"))
            return;

        GeneralSaveData generalData = JsonUtility.FromJson<GeneralSaveData>(PlayerPrefs.GetString(slot.Slot.ToString() + "GeneralData"));

        slot.Initialize(generalData);
    }
}