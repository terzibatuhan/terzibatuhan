using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SaveSlotEnum
{
    S1, S2, S3
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSlotEnum Slot;

    public bool AutoSave;

    [SerializeField] private float _autoSaveInterval = 5f;
    [SerializeField] private float _remainingTime;

    private SettingsSaveHandler settingsSaveHandler;

    private void Start()
    {
        AutoSave = false;

        LoadGame();

        settingsSaveHandler = GetComponent<SettingsSaveHandler>();

        LoadSettings();

        _remainingTime = _autoSaveInterval;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            AutoSave = !AutoSave;
            Debug.Log("Autosaving is " + (AutoSave ? "Activated" : "Turned Off"));
        }

        if (!AutoSave) return;

        _remainingTime -= Time.deltaTime;

        if (_remainingTime < 0f)
        {
            _remainingTime = _autoSaveInterval;
            SaveGame();
            SaveSettings();
            Debug.Log("Savelendi");
        }
    }

    public void SaveGame()
    {
        var PalletsInScene = new List<PalletSaveData>();

        foreach (var pallet in PalletHolder.Instance.Pallets)
        {
            if (pallet.IsSlotted)
            {
                PalletSaveData data = new()
                {
                    Slot = pallet.ItsSlot,
                    Rotation = pallet.transform.eulerAngles,
                    StackedPallets = pallet.FindStackedPallets(0),
                    CaseDatas = new()
                };

                foreach (var product in pallet.Products)
                {
                    CaseSaveData caseData = new()
                    {
                        ProductID = ProductHolder.Instance.Products.IndexOf(product.ProductData),
                        Location = product.transform.localPosition,
                        RotateState = product.RotateState
                    };

                    data.CaseDatas.Add(caseData);
                }

                PalletsInScene.Add(data);
            }
        }

        GeneralSaveData generalSaveData = new()
        {
            Money = Currency.Instance.Money,

            PalletDatas = PalletsInScene
        };

        string generalSaveDataJson = JsonUtility.ToJson(generalSaveData);

        PlayerPrefs.SetString(Slot.ToString() + "GeneralData", generalSaveDataJson);
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(Slot.ToString() + "GeneralData"))
            return;

        GeneralSaveData generalSaveData = JsonUtility.FromJson<GeneralSaveData>(PlayerPrefs.GetString(Slot.ToString() + "GeneralData"));

        Currency.Instance.SetMoney(generalSaveData.Money);

        if (generalSaveData.PalletDatas == null) return;

        if (generalSaveData.PalletDatas.Count == 0) return;

        foreach (PalletSaveData palletData in generalSaveData.PalletDatas)
        {
            Pallet pallet = PalletCreator.Instance.CreateEmptyPallet();

            GroundGrid grid = GroundGridManager.Instance.GroundGrids.FirstOrDefault(x => x.SlotID == palletData.Slot);

            pallet.Initialize();
            pallet.transform.position = grid.transform.position;
            pallet.transform.eulerAngles = palletData.Rotation;
            pallet.IsSlotted = true;
            pallet.ItsSlot = grid.SlotID;
            grid.IsFull = true;

            for (int i = 0; i < palletData.StackedPallets; i++)
            {
                Pallet childPallet = PalletCreator.Instance.CreateEmptyPallet();
                childPallet.Initialize();
                pallet.StackPallet(childPallet);
            }

            foreach (CaseSaveData caseData in palletData.CaseDatas)
            {
                Product product = CaseCreator.Instance.CreateCase(caseData.ProductID);

                pallet.PlaceBoxIntoPallet(product, caseData.Location, caseData.RotateState);
            }
        }
    }

    public void SaveSettings()
    {
        SettingsSaveHandler.SettingsData data = settingsSaveHandler.CreateData();

        string settingsJson = JsonUtility.ToJson(data);

        PlayerPrefs.SetString("Settings", settingsJson);
    }

    public void LoadSettings()
    {
        if (!PlayerPrefs.HasKey("Settings")) return;

        SettingsSaveHandler.SettingsData data = JsonUtility.FromJson<SettingsSaveHandler.SettingsData>(PlayerPrefs.GetString("Settings"));

        settingsSaveHandler.LoadData(data);
    }
}