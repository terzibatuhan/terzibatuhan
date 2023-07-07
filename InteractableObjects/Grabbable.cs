using System.Collections;
using UnityEngine;

public class Grabbable : MonoBehaviour, IInteractable
{
    // Private fields
    Rigidbody _grabbableRb;

    void Start()
    {
        _grabbableRb = GetComponent<Rigidbody>();
    }

    // Public methods
    public void Interact(Transform grabber = null)
    {
        grabber.position = transform.position;

        // Continuously move the grabbed object to the grabber's position and update its velocity
        StartCoroutine(MoveObject(grabber));
    }

    public void Enter()
    {
        // Do nothing
    }

    public void Exit()
    {
        // Do nothing
    }

    // Private methods
    IEnumerator MoveObject(Transform grabber) 
    {
        while (!Input.GetMouseButtonUp(0))
        {
            // Move the grabbed object with the mouse and update its velocity
            grabber.position += new Vector3(0f, Input.GetAxis("Mouse Y") * Time.deltaTime * 3f, 0f);
            _grabbableRb.velocity = (grabber.position - _grabbableRb.position) * 10f;

            // Apply rotation to the grabbed object if Q or E is held down
            if (Input.GetKey(KeyCode.E)) _grabbableRb.AddRelativeTorque(5f * Time.deltaTime * Vector3.up);
            if (Input.GetKey(KeyCode.Q)) _grabbableRb.AddRelativeTorque(5f * Time.deltaTime * Vector3.down);

            yield return null;
        }
    }
}
