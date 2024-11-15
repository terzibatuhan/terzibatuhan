using UnityEngine;

public class Interacter : MonoBehaviour
{
    public IInteractable CurrentInteractable;

    [Header("Raycast Settings")]
    [SerializeField] private float _interactionDistance = 5f;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private float _raycastTimer = 0f;
    [SerializeField] private float _raycastInterval = 0.2f;

    [Header("Interaction")]
    [SerializeField] private Transform _cursor;

    [Header("Components")]
    public Hand Hand;
    public Driver Driver;

    private Camera _camera;
    private Ray _ray;
    private RaycastHit _hit;

    public static Interacter Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _camera = Camera.main;

        Hand = new Hand
        {
            Interacter = this
        };

        Driver = GetComponent<Driver>();
    }

    private void Update()
    {
        PlayerState state = Player.Instance.GetPlayerState();

        Hand.HandleRotation(state);

        _raycastTimer += Time.deltaTime;

        if (_raycastTimer >= _raycastInterval)
        {
            _raycastTimer = 0f;

            bool hit = CheckForInteractableObject(out IInteractable highlightable);

            _cursor.localScale = hit ? Vector3.one * 1.2f : Vector3.one;

            if (hit)
                highlightable?.Highlight(this);

            else
            {
                if (state == PlayerState.Placing)
                    CurrentInteractable.ResetState(this);
            }
        }

        if (!Input.GetMouseButtonDown(0)) return;

        if (state == PlayerState.Riding)
        {
            Driver.CurrentCar.DropDriver();
            return;
        }

        else if (state == PlayerState.Carrying)
        {
            CurrentInteractable.Paint(Colors.Grey);
            Player.Instance.ChangeState(PlayerState.Placing);
            return;
        }

        else if (state == PlayerState.Placing)
        {
            Hand.ReleaseTheHandedItem();
            return;
        }

        else if (state != PlayerState.Idle)
            return;

        if (!CheckForInteractableObject(out IInteractable interactable))
            return;

        interactable.Interact(this);
    }

    public void ResetState()
    {
        CurrentInteractable = null;
        Hand.CurrentPallet = null;
        BoxPlacingHandler.Instance.BoxParent.SetParent(null);
        Player.Instance.ChangeState(PlayerState.Idle);
    }

    private bool CheckForInteractableObject(out IInteractable interactable)
    {
        interactable = null;
        _ray = _camera.ScreenPointToRay(Input.mousePosition);

        bool hit = Physics.Raycast(_ray, out _hit, _interactionDistance, _interactLayer);

        if (hit)
            if (_hit.collider.TryGetComponent(out IInteractable foundItem))
                interactable = foundItem;

        return hit;
    }
}