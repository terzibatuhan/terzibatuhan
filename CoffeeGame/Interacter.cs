using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacter : MonoBehaviour
{
    // Camera
    Camera _camera;

    // Interactable objects
    [SerializeField] LayerMask _interactableLayerMask;
    IInteractable[] _interactables = new IInteractable[0];

    // Interacting
    Ray _ray;
    RaycastHit _hit;
    [SerializeField] float _interactingDistance;

    // Grabbing objects
    [SerializeField] Transform _grabber;

    // Highlight mouse cursor
    Mouse _mouse;

    void Start()
    {
        _camera = Camera.main;
        _mouse = Mouse.Instance;
    }

    void Update()
    {
        // Cast a ray from the mouse position and check for any interactable objects within range
        _ray = _camera.ScreenPointToRay(Input.mousePosition);

        foreach (var interactable in _interactables) interactable?.Exit();

        _mouse.SetBright(false);

        if (!Physics.Raycast(_ray, out _hit, _interactingDistance, _interactableLayerMask)) return;

        // Update the list of interactable objects
        _interactables = _hit.collider.GetComponents<IInteractable>();

        // Call Enter() on each interactable object and check for input to trigger Interact()
        foreach (var interactable in _interactables) 
        {
            interactable?.Enter();

            if (Input.GetMouseButtonDown(0))
            {
                interactable?.Interact(_grabber);
            }
        }

        // Highlight mouse cursor
        _mouse.SetBright(true);
    }
}
