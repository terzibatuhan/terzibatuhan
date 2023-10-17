using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] private SkillSO _skill;
    [SerializeField] private Color _unlockedColor;
    [SerializeField] private Color _unlockableColor;
    [SerializeField] private Color _notUnlockableColor;
    [SerializeField] private Slider _fillSlider;
    [SerializeField] private float _fillDuration;
    private float _buttonPressedTime;

    private Image _buttonImage;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _originalPosition;

    private SkillSlot _targetSlot; // The slot to capture the skill button

    private void Awake()
    {
        _buttonImage = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _originalPosition = _rectTransform.anchoredPosition;
    }

    private void Start()
    {
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        if (_skill.IsUnlocked)
        {
            _buttonImage.color = _unlockedColor;
        }
        else if (IsUnlockable())
        {
            _buttonImage.color = _unlockableColor;
        }
        else
        {
            _buttonImage.color = _notUnlockableColor;
        }
    }

    private bool IsUnlockable()
    {
        foreach (var skill in _skill.Prerequisites)
        {
            if (!skill.IsUnlocked)
            {
                return false;
            }
        }
        return true;
    }

    public static void UpdateAllButtonColors()
    {
        foreach (var button in FindObjectsOfType<SkillButton>())
        {
            button.UpdateButtonColor();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Handle logic
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Handle logic
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsUnlockable() && !_skill.IsUnlocked)
            StartCoroutine(OnPress());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_skill.IsUnlocked) return;

        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator OnPress()
    {
        while (Input.GetMouseButton(0))
        {
            _buttonPressedTime += Time.deltaTime;
            _fillSlider.value = Mathf.Lerp(0, 100, _buttonPressedTime / _fillDuration);

            if (_buttonPressedTime > _fillDuration)
            {
                _skill.IsUnlocked = true;
                UpdateAllButtonColors();
                Debug.Log(_skill.Name + " Açýldý!");
            }

            yield return null;
        }

        _buttonPressedTime = 0;
        _fillSlider.value = 0;
        UpdateButtonColor();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_skill.IsUnlocked) _rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        SkillSlot[] allSlots = FindObjectsOfType<SkillSlot>();
        float closestDistance = float.MaxValue;
        _targetSlot = null;

        foreach (SkillSlot slot in allSlots)
        {
            float distance = Vector2.Distance(_rectTransform.position, slot.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                _targetSlot = slot;
            }
        }

        if (_targetSlot != null && closestDistance <= 100f)
        {
            // Snap the button to the target slot
            _rectTransform.anchoredPosition = _originalPosition;
            _targetSlot.AssignSkill(_skill);
        }
        else
        {
            // Return the button to its original position
            _rectTransform.anchoredPosition = _originalPosition;
            _targetSlot = null;
        }
    }
}