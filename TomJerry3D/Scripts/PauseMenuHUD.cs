using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Start screen and pause overlay for the Tom & Jerry mobile demo.
/// Time is paused until Play is pressed; Escape toggles pause in the editor.
/// </summary>
public class PauseMenuHUD : MonoBehaviour
{
    private Canvas canvas;
    private GameObject startPanel;
    private GameObject pausePanel;
    private bool started;
    private bool paused;

    private void Start()
    {
        Build();
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!started) return;
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    private void Build()
    {
        if (FindObjectOfType<EventSystem>() == null)
            new GameObject("Menu EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        GameObject root = new GameObject("Start Pause Menu",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.transform.SetParent(transform, false);
        canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 40;

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        Button pauseButton = MakeButton("PAUSE", root.transform,
            new Vector2(0.91f, 0.92f), new Vector2(170, 72));
        pauseButton.onClick.AddListener(TogglePause);

        startPanel = MakePanel("Start Panel", root.transform);
        AddTitle(startPanel.transform, "TOM & JERRY", "THE GREAT CHEESE CHASE");
        Button playButton = MakeButton("PLAY", startPanel.transform,
            new Vector2(0.5f, 0.40f), new Vector2(300, 100));
        playButton.onClick.AddListener(StartGame);

        pausePanel = MakePanel("Pause Panel", root.transform);
        AddTitle(pausePanel.transform, "PAUSED", "Take a little break!");
        Button resumeButton = MakeButton("RESUME", pausePanel.transform,
            new Vector2(0.5f, 0.40f), new Vector2(300, 90));
        resumeButton.onClick.AddListener(ResumeGame);
        Button restartButton = MakeButton("RESTART", pausePanel.transform,
            new Vector2(0.5f, 0.27f), new Vector2(300, 78));
        restartButton.onClick.AddListener(RestartScene);

        pausePanel.SetActive(false);
    }

    private void StartGame()
    {
        started = true;
        paused = false;
        startPanel.SetActive(false);
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void TogglePause()
    {
        if (!started) return;
        if (paused) ResumeGame();
        else
        {
            paused = true;
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void ResumeGame()
    {
        paused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        Scene active = SceneManager.GetActiveScene();
        if (!string.IsNullOrEmpty(active.path))
            SceneManager.LoadScene(active.path);
        else
            Debug.LogWarning("Save the demo scene before using Restart.");
    }

    private GameObject MakePanel(string panelName, Transform parent)
    {
        GameObject panel = new GameObject(panelName, typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image image = panel.GetComponent<Image>();
        image.color = new Color(0.035f, 0.055f, 0.085f, 0.88f);
        return panel;
    }

    private void AddTitle(Transform parent, string heading, string subtitle)
    {
        Text title = MakeText("Title", parent, new Vector2(0.5f, 0.64f),
            new Vector2(1000, 130), 76);
        title.text = heading;
        title.fontStyle = FontStyle.Bold;
        title.color = new Color(1f, 0.78f, 0.28f);

        Text sub = MakeText("Subtitle", parent, new Vector2(0.5f, 0.55f),
            new Vector2(1000, 70), 30);
        sub.text = subtitle;
        sub.color = Color.white;
    }

    private Button MakeButton(string label, Transform parent, Vector2 anchor, Vector2 size)
    {
        GameObject obj = new GameObject(label + " Button", typeof(RectTransform), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        Image image = obj.GetComponent<Image>();
        image.color = new Color(0.96f, 0.67f, 0.18f, 0.96f);
        Button button = obj.GetComponent<Button>();
        button.targetGraphic = image;

        Text text = MakeText("Button Label", obj.transform, new Vector2(0.5f, 0.5f), size, 32);
        text.text = label;
        text.fontStyle = FontStyle.Bold;
        text.color = new Color(0.10f, 0.09f, 0.08f);
        return button;
    }

    private Text MakeText(string objectName, Transform parent, Vector2 anchor, Vector2 size, int fontSize)
    {
        GameObject obj = new GameObject(objectName, typeof(RectTransform), typeof(Text));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;

        Text text = obj.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        return text;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
