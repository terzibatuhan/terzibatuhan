using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(Transform grabber = null);

    void Enter();

    void Exit();
}
