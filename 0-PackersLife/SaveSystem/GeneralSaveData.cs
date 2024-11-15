using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneralSaveData
{
    public string SaveName;
    public int Money;

    public List<PalletSaveData> PalletDatas;
}

[System.Serializable]
public class PalletSaveData
{
    public Vector2 Slot;
    public Vector3 Rotation;
    public int StackedPallets;

    public List<CaseSaveData> CaseDatas;
}

[System.Serializable]
public class CaseSaveData
{
    public int ProductID;
    public Vector3 Location;
    public RotateState RotateState;
}