using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Reusable touch surface: use as a virtual joystick or a swipe-to-look area.
/// Attach to a UI Image with Raycast Target enabled.
/// </summary>
[RequireComponent(typeof(Image))]
public class TouchControlPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private MousePlayerController player;
    [SerializeField] private RectTransform handle;
    [SerializeField] private bool lookPad;
    [SerializeField] private float maxRadius = 58f;

    private int activePointer = int.MinValue;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = transform as RectTransform;
        if (player == null) player = FindObjectOfType<MousePlayerController>();
    }

    public void Configure(MousePlayerController target, bool useForLook, RectTransform joystickHandle = null)
    {
        player = target;
        lookPad = useForLook;
        handle = joystickHandle;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activePointer != int.MinValue) return;
        activePointer = eventData.pointerId;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (player == null || eventData.pointerId != activePointer) return;

        if (lookPad)
        {
            player.LookFromUI(eventData.delta);
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            return;

        Vector2 normalized = Vector2.ClampMagnitude(localPoint / maxRadius, 1f);
        player.MoveFromUI(normalized);
        if (handle != null) handle.anchoredPosition = normalized * maxRadius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointer) return;
        activePointer = int.MinValue;
        if (player != null)
        {
            if (lookPad) player.StopLookFromUI();
            else player.StopMoveFromUI();
        }
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }
}
