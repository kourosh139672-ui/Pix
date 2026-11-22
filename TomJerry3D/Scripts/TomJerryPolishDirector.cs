using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Adds mission progression, generated cartoon sound cues and a small graphics
/// preset panel. Everything is built at runtime, so no imported assets are needed.
/// </summary>
public class TomJerryPolishDirector : MonoBehaviour
{
    [SerializeField] private int cheeseGoal = 3;
    private MouseCheeseCounter counter;
    private MouseTrapBaitSocket trap;
    private Text missionText;
    private AudioSource audioSource;
    private int previousCheese;
    private bool previousArmed;
    private GameObject graphicsPanel;

    private void Start()
    {
        counter = FindObjectOfType<MouseCheeseCounter>();
        trap = FindObjectOfType<MouseTrapBaitSocket>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        BuildMissionAndGraphicsUI();
        ApplyGraphicsPreset(1);
        UpdateMission();
    }

    private void Update()
    {
        int cheese = counter != null ? counter.TotalCheese : 0;
        if (cheese > previousCheese) PlayTone(740f, 0.16f);
        previousCheese = cheese;

        bool armed = trap != null && trap.IsArmed;
        if (armed && !previousArmed) PlayTone(980f, 0.28f);
        previousArmed = armed;
        UpdateMission();
    }

    private void BuildMissionAndGraphicsUI()
    {
        GameObject root = new GameObject("Mission And Graphics HUD",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.transform.SetParent(transform, false);
        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        missionText = MakeText("Mission Progress", root.transform,
            new Vector2(0.5f, 0.875f), new Vector2(950, 58), 24);
        missionText.color = Color.white;
        missionText.fontStyle = FontStyle.Bold;

        Button graphics = MakeButton("GFX", root.transform,
            new Vector2(0.08f, 0.92f), new Vector2(112, 64));
        graphics.onClick.AddListener(() => graphicsPanel.SetActive(!graphicsPanel.activeSelf));

        graphicsPanel = new GameObject("Graphics Presets", typeof(RectTransform), typeof(Image));
        graphicsPanel.transform.SetParent(root.transform, false);
        RectTransform panelRect = graphicsPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.08f, 0.78f);
        panelRect.anchorMax = new Vector2(0.08f, 0.78f);
        panelRect.sizeDelta = new Vector2(390, 190);
        graphicsPanel.GetComponent<Image>().color = new Color(0.04f, 0.06f, 0.09f, 0.9f);

        MakeButton("LOW", graphicsPanel.transform, new Vector2(0.2f, 0.5f), new Vector2(100, 70))
            .onClick.AddListener(() => { ApplyGraphicsPreset(0); graphicsPanel.SetActive(false); });
        MakeButton("MED", graphicsPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(100, 70))
            .onClick.AddListener(() => { ApplyGraphicsPreset(1); graphicsPanel.SetActive(false); });
        MakeButton("HIGH", graphicsPanel.transform, new Vector2(0.8f, 0.5f), new Vector2(100, 70))
            .onClick.AddListener(() => { ApplyGraphicsPreset(2); graphicsPanel.SetActive(false); });
        graphicsPanel.SetActive(false);
    }

    private void UpdateMission()
    {
        if (missionText == null) return;
        int cheese = counter != null ? counter.TotalCheese : 0;
        bool armed = trap != null && trap.IsArmed;
        if (armed)
            missionText.text = "MISSION COMPLETE! Tom is in for a surprise!";
        else if (cheese >= cheeseGoal)
            missionText.text = "CHEESE READY! Carry it to the mousetrap and tap USE.";
        else
            missionText.text = "MISSION: Collect cheese (" + cheese + "/" + cheeseGoal + ") and bait the mousetrap.";
    }

    private void ApplyGraphicsPreset(int preset)
    {
        int level = Mathf.Clamp(preset, 0, Mathf.Max(0, QualitySettings.names.Length - 1));
        QualitySettings.SetQualityLevel(level, true);
        QualitySettings.shadowDistance = preset == 0 ? 18f : (preset == 1 ? 35f : 60f);
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = preset == 0 ? 30 : 60;
    }

    private void PlayTone(float frequency, float duration)
    {
        const int sampleRate = 22050;
        int samples = Mathf.Max(1, Mathf.CeilToInt(sampleRate * duration));
        AudioClip clip = AudioClip.Create("Cartoon UI Cue", samples, 1, sampleRate, false);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = Mathf.Clamp01(1f - t / duration);
            data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.16f;
        }
        clip.SetData(data, 0);
        audioSource.PlayOneShot(clip);
        Destroy(clip, duration + 0.1f);
    }

    private static Text MakeText(string name, Transform parent, Vector2 anchor, Vector2 size, int fontSize)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchor; rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero; rect.sizeDelta = size;
        Text text = obj.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        return text;
    }

    private static Button MakeButton(string label, Transform parent, Vector2 anchor, Vector2 size)
    {
        GameObject obj = new GameObject(label + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchor; rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero; rect.sizeDelta = size;
        Image image = obj.GetComponent<Image>();
        image.color = new Color(0.95f, 0.67f, 0.20f, 0.94f);
        Button button = obj.GetComponent<Button>();
        button.targetGraphic = image;
        Text text = MakeText("Label", obj.transform, new Vector2(0.5f, 0.5f), size, 24);
        text.text = label;
        text.fontStyle = FontStyle.Bold;
        text.color = new Color(0.12f, 0.10f, 0.08f);
        return button;
    }
}
