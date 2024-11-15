using UnityEngine;
using TMPro;

public class Expiration : MonoBehaviour, IInteractable
{
    [SerializeField] string itemName;
    [field:SerializeField] public float ExpirationTime { get; set; }
    // Expiration date UI
    [SerializeField] CanvasGroup _expirationCanvasGroup;
    [SerializeField] TextMeshProUGUI _expirationText;

    private void Update()
    {
        ExpirationTime -= Time.deltaTime;
    }

    public void Interact(Transform transform = null)
    {
        
    }

    public void Enter()
    {
        ShowExpiryDateUI();
    }

    public void Exit()
    {
        HideExpiryDateUI();
    }

    void ShowExpiryDateUI()
    {
        _expirationText.SetText("Expire : " + string.Format("{0:00}:{1:00}", (int)(ExpirationTime / 60), (int)(ExpirationTime % 60)));
        _expirationCanvasGroup.alpha = 1;
    }

    void HideExpiryDateUI()
    {
        _expirationCanvasGroup.alpha = 0;
    }
}
