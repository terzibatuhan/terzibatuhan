using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public SaveSlotEnum Slot;

    [SerializeField] private Text _slotText;
    [SerializeField] private Text _moneyText;
    [SerializeField] private Button _deleteButton;

    public void Initialize(GeneralSaveData data)
    {
        _slotText.text = "Save Slot " + Slot.ToString()[1..];

        _moneyText.gameObject.SetActive(true);
        _moneyText.text = data.Money.ToString() + " $";

        _deleteButton.gameObject.SetActive(true);
        _deleteButton.onClick.RemoveAllListeners();
        _deleteButton.onClick.AddListener(() => DeleteSave());
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(Slot.ToString() + "GeneralData");

        _deleteButton.gameObject.SetActive(false);

        _slotText.text = "EMPTY SLOT";
        _moneyText.gameObject.SetActive(false);
    }
}
