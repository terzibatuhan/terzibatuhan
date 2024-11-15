using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : Worker
{
    private readonly List<Action> _unloadTruck = new();

    public void InitializeUnloadingEvents()
    {
        IsAvailable = false;

        int palletCount = Truck.Instance.Pallets.Count;

        _unloadTruck.Add(() => FindNearestCar());
        _unloadTruck.Add(() => GoToCar());
        _unloadTruck.Add(() => EnterCar());

        for (int i = 0; i < palletCount; i++)
        {
            _unloadTruck.Add(() => DriveToTruck());
            _unloadTruck.Add(() => TurnToTarget(Vector3.down * 180));
            _unloadTruck.Add(() => EnterToTruck());
            _unloadTruck.Add(() => TakeThePallet());
            _unloadTruck.Add(() => WaitForNewAction());
            _unloadTruck.Add(() => LeaveTheTruck());
            _unloadTruck.Add(() => DriveToStorage());
            _unloadTruck.Add(() => TurnToTarget(Vector3.up * 270));
            _unloadTruck.Add(() => EnterToStorage());
            _unloadTruck.Add(() => LeaveThePallet());
            _unloadTruck.Add(() => WaitForNewAction());
        }
        
        _unloadTruck.Add(() => DriveToParkingArea());

        StartCoroutine(StartUnloadingTruck());
    }

    private Transform CheckWhichSideToGo()
    {
        return (Truck.Instance.Pallets.Count % 2 == 1) ? Truck.Instance.LeftEntrance : Truck.Instance.RightEntrance;
    }
    private void DriveToTruck()
    {
        StartCoroutine(_driver.MoveToTarget(CheckWhichSideToGo().position));
    }

    protected void TurnToTarget(Vector3 target)
    {
        _driver.IK.lookObj = _driver.CurrentCar.LookObjBackwards;

        StartCoroutine(_driver.TurnToTarget(target));
    }

    protected void EnterToTruck()
    {
        List<Pallet> pallets = Truck.Instance.Pallets;

        StartCoroutine(_driver.LerpToTarget(pallets[^1].transform.position + Vector3.forward));
    }

    protected void TakeThePallet()
    {
        List<Pallet> pallets = Truck.Instance.Pallets;

        pallets[^1].AttachToCar(_driver.CurrentCar);
        _driver.CurrentCar.AttachedPallet = pallets[^1];
        Truck.Instance.Pallets.Remove(pallets[^1]);

        FinishAction();
    }

    protected void LeaveTheTruck()
    {
        _driver.IK.lookObj = _driver.CurrentCar.LookObjForward;

        StartCoroutine(_driver.LerpToTarget(new Vector3(_driver.CurrentCar.transform.position.x, _driver.CurrentCar.transform.position.y, 12)));
    }

    protected void DriveToStorage()
    {
        foreach (GroundGrid grid in GroundGridManager.Instance.GroundGrids)
        {
            if (grid.GridType == GroundGridType.Storage && !grid.IsFull)
            {
                _targetGrid = grid;
                break;
            }
        }

        if (!_targetGrid)
        {
            Debug.Log("Uygun yer yok.");
            return;
        }

        _targetGrid.Fill();

        StartCoroutine(_driver.MoveToTarget(_targetGrid.transform.position + _targetGrid.transform.forward * 3));
    }

    protected void EnterToStorage()
    {
        StartCoroutine(_driver.LerpToTarget(_targetGrid.transform.position + -_targetGrid.transform.forward));
    }

    protected void LeaveThePallet()
    {
        _driver.CurrentCar.AttachedPallet.ItsSlot = _targetGrid.SlotID;
        _driver.CurrentCar.AttachedPallet.IsSlotted = true;
        _driver.CurrentCar.AttachedPallet.DeAttach(_targetGrid);
        _driver.CurrentCar.AttachedPallet = null;

        _driver.IK.lookObj = _driver.CurrentCar.LookObjForward;

        FinishAction();
    }

    protected void DriveToParkingArea()
    {
        foreach (GroundGrid grid in GroundGridManager.Instance.GroundGrids)
        {
            if (grid.GridType == GroundGridType.Parking && !grid.IsFull)
            {
                _targetGrid = grid;
                break;
            }
        }

        if (!_targetGrid)
        {
            Debug.Log("Uygun yer yok.");
            return;
        }

        _targetGrid.Fill();

        StartCoroutine(_driver.MoveToTarget(_targetGrid.transform.position));
    }

    private IEnumerator StartUnloadingTruck() 
    {
        int index = 0;

        while (index < _unloadTruck.Count)
        {
            _unloadTruck[index].Invoke();

            yield return new WaitUntil(() => _isCurrentActionCompleted);

            _isCurrentActionCompleted = false;

            index++;
        }

        Debug.Log("Unloading Bitti");
        _unloadTruck.Clear();
    }
}