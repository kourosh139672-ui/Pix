using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Creates a simple Android HUD at runtime: left movement joystick,
/// right swipe-look area and an Interact button.
/// Add this component to any active scene object.
/// </summary>
public class MobileFirstPersonHUD : MonoBehaviour
{
    [SerializeField] private MousePlayerController player;
    [SerializeField] private CharacterHandPickup pickup;
    [SerializeField] private bool buildOnStart = true;

    private void Start()
    {
        if (!buildOnStart) return;
        if (player == null) player = FindObjectOfType<MousePlayerController>();
        if (pickup == null) pickup = FindObjectOfType<CharacterHandPickup>();
        Build();
    }

    [ContextMenu("Build Mobile HUD")]
    public void Build()
    {
        if (player == null) player = FindObjectOfType<MousePlayerController>();
        if (pickup == null) pickup = FindObjectOfType<CharacterHandPickup>();
        if (player == null)
        {
            Debug.LogWarning("MobileFirstPersonHUD: assign a MousePlayerController.");
            return;
        }

        Transform oldHud = transform.Find("Generated Mobile HUD");
        if (oldHud != null)
        {
            if (Application.isPlaying) Destroy(oldHud.gameObject);
            else DestroyImmediate(oldHud.gameObject);
        }

        EnsureEventSystem();

        GameObject root = new GameObject("Generated Mobile HUD",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.transform.SetParent(transform, false);

        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Left movement joystick.
        Image joystickBase = MakeImage("Move Joystick", root.transform,
            new Vector2(0.14f, 0.16f), new Vector2(150, 150),
            new Color(0.08f, 0.10f, 0.14f, 0.42f));
        Image joystickHandle = MakeImage("Joystick Handle", joystickBase.transform,
            new Vector2(0.5f, 0.5f), new Vector2(64, 64),
            new Color(0.88f, 0.91f, 0.96f, 0.78f));
        joystickHandle.raycastTarget = false;
        TouchControlPad movePad = joystickBase.gameObject.AddComponent<TouchControlPad>();
        movePad.Configure(player, false, joystickHandle.rectTransform);

        // Right-side swipe surface for first-person camera look.
        Image lookArea = MakeImage("Look Area", root.transform,
            new Vector2(0.77f, 0.55f), new Vector2(600, 650),
            new Color(1f, 1f, 1f, 0.015f));
        TouchControlPad lookPad = lookArea.gameObject.AddComponent<TouchControlPad>();
        lookPad.Configure(player, true);

        // Interact button: pick up/drop objects and operate the fridge.
        if (pickup != null)
        {
            Image buttonImage = MakeImage("Interact Button", root.transform,
                new Vector2(0.88f, 0.14f), new Vector2(132, 104),
                new Color(0.96f, 0.68f, 0.20f, 0.88f));
            Button button = buttonImage.gameObject.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(pickup.Interact);

            GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(buttonImage.transform, false);
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            Text label = labelObject.GetComponent<Text>();
            label.text = "USE";
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = 28;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.color = new Color(0.12f, 0.10f, 0.08f);
        }

        Debug.Log("Tom & Jerry 3D mobile HUD created.");
    }

    private static Image MakeImage(string objectName, Transform parent, Vector2 anchor,
        Vector2 size, Color color)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
        Image image = obj.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = true;
        return image;
    }

    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }
}
