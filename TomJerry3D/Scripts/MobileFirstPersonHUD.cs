using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Runtime Android HUD: left movement joystick, right swipe-look area, objective
/// counter and USE button. Add to an active scene object or let the demo add it.
/// </summary>
public class MobileFirstPersonHUD : MonoBehaviour
{
    [SerializeField] private MousePlayerController player;
    [SerializeField] private CharacterHandPickup pickup;
    [SerializeField] private bool buildOnStart = true;

    private Text objectiveLabel;
    private MouseCheeseCounter cheeseCounter;

    public void Configure(MousePlayerController target, CharacterHandPickup handPickup)
    {
        player = target;
        pickup = handPickup;
    }

    private void Start()
    {
        if (!buildOnStart) return;
        if (player == null) player = FindObjectOfType<MousePlayerController>();
        if (pickup == null) pickup = FindObjectOfType<CharacterHandPickup>();
        Build();
    }

    private void Update()
    {
        if (objectiveLabel == null) return;

        int cheese = cheeseCounter != null ? cheeseCounter.TotalCheese : 0;
        string task = pickup != null && pickup.HasItem
            ? "Bait in hand! Aim at the mousetrap and tap USE."
            : "Find cheese, pick it up, then bait the mousetrap.";
        objectiveLabel.text = "CHEESE: " + cheese + "    |    " + task;
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

        cheeseCounter = player.GetComponent<MouseCheeseCounter>();

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

        objectiveLabel = MakeText("Objective", root.transform,
            new Vector2(0.5f, 0.94f), new Vector2(1120, 76), 26);
        objectiveLabel.alignment = TextAnchor.MiddleCenter;
        objectiveLabel.color = Color.white;
        objectiveLabel.fontStyle = FontStyle.Bold;
        objectiveLabel.text = "Find cheese and bait the mousetrap.";

        Image joystickBase = MakeImage("Move Joystick", root.transform,
            new Vector2(0.14f, 0.16f), new Vector2(150, 150),
            new Color(0.08f, 0.10f, 0.14f, 0.42f));
        Image joystickHandle = MakeImage("Joystick Handle", joystickBase.transform,
            new Vector2(0.5f, 0.5f), new Vector2(64, 64),
            new Color(0.88f, 0.91f, 0.96f, 0.78f));
        joystickHandle.raycastTarget = false;
        joystickBase.gameObject.AddComponent<TouchControlPad>().Configure(player, false, joystickHandle.rectTransform);

        Image lookArea = MakeImage("Look Area", root.transform,
            new Vector2(0.77f, 0.55f), new Vector2(600, 650),
            new Color(1f, 1f, 1f, 0.015f));
        lookArea.gameObject.AddComponent<TouchControlPad>().Configure(player, true);

        if (pickup != null)
        {
            Image buttonImage = MakeImage("Interact Button", root.transform,
                new Vector2(0.88f, 0.14f), new Vector2(132, 104),
                new Color(0.96f, 0.68f, 0.20f, 0.88f));
            Button button = buttonImage.gameObject.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(pickup.Interact);

            Text label = MakeText("Label", buttonImage.transform,
                new Vector2(0.5f, 0.5f), new Vector2(132, 104), 28);
            label.text = "USE";
            label.color = new Color(0.12f, 0.10f, 0.08f);
            label.fontStyle = FontStyle.Bold;
        }

        Debug.Log("Tom & Jerry 3D mobile HUD created.");
    }

    private static Image MakeImage(string objectName, Transform parent, Vector2 anchor, Vector2 size, Color color)
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

    private static Text MakeText(string objectName, Transform parent, Vector2 anchor, Vector2 size, int fontSize)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        Text label = obj.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        label.fontSize = fontSize;
        label.alignment = TextAnchor.MiddleCenter;
        label.raycastTarget = false;
        return label;
    }

    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }
}
