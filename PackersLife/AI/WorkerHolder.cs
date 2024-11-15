using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerHolder : MonoBehaviour
{
    public List<Worker> Workers = new();

    public static WorkerHolder Instance;

    private void Awake()
    {
        Instance = this;
    }
}
