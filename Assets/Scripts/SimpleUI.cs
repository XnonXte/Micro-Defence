using System.Collections;
using MicroDefense.Core;
using UnityEngine;
using UnityEngine.UI;

public sealed class SimpleUI : MonoBehaviour
{
    public static SimpleUI Instance { get; private set; }

    [SerializeField] private int totalWavesFallback = 5;
    [SerializeField] private int fontSize = 36;
    [SerializeField] private bool buildRuntimeHud = true;

    private Text waveText;
    private Text plasmaText;
    private Text healthText;
    private Image plasmaIcon;
    private Image healthBarFill;
    private Text waveStatusText;
    private GameObject announcementPanel;
    private Text announcementText;
    private GameObject resultPanel;
    private Text resultTitle;
    private Text resultSubtitle;
    private Font runtimeFont;
    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // TIDAK pakai DontDestroyOnLoad agar retry bisa clean restart
        // DontDestroyOnLoad(gameObject); ← DIHAPUS!
        runtimeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Only build UI on game scenes, not on MainMenu
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu")
        {
            enabled = false;
            return;
        }
        
        if (buildRuntimeHud)
            BuildUI();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private IEnumerator Start()
    {
        yield return null;
        Subscribe();
        RefreshValues();
    }

    private void OnDisable()
    {
        if (WaveManager.Instance != null) WaveManager.Instance.OnWaveStatusChanged -= UpdateWave;
        if (LeakManager.Instance != null) LeakManager.Instance.OnHealthChanged -= UpdateHealth;
        if (PlasmaManager.Instance != null) PlasmaManager.Instance.OnPlasmaChanged -= UpdatePlasma;
        if (GameManager.Instance != null) GameManager.Instance.OnGameStateChanged -= UpdateState;
    }

    private void Subscribe()
    {
        if (WaveManager.Instance != null) WaveManager.Instance.OnWaveStatusChanged -= UpdateWave;
        if (WaveManager.Instance != null) WaveManager.Instance.OnWaveStatusChanged += UpdateWave;
        if (LeakManager.Instance != null) LeakManager.Instance.OnHealthChanged -= UpdateHealth;
        if (LeakManager.Instance != null) LeakManager.Instance.OnHealthChanged += UpdateHealth;
        if (PlasmaManager.Instance != null) PlasmaManager.Instance.OnPlasmaChanged -= UpdatePlasma;
        if (PlasmaManager.Instance != null) PlasmaManager.Instance.OnPlasmaChanged += UpdatePlasma;
        if (GameManager.Instance != null) GameManager.Instance.OnGameStateChanged -= UpdateState;
        if (GameManager.Instance != null) GameManager.Instance.OnGameStateChanged += UpdateState;
    }

    private void BuildUI()
    {
        canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            var canvasObject = new GameObject("GameCanvas");
            canvasObject.transform.SetParent(transform, false);
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        canvas.transform.localScale = Vector3.one;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;
        var hud = CreateObject("HUD", canvas.transform);
        var hudRect = hud.GetComponent<RectTransform>() ?? hud.AddComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(0, 1);
        hudRect.anchorMax = new Vector2(1, 1);
        hudRect.pivot = new Vector2(0.5f, 1);
        hudRect.sizeDelta = new Vector2(0, 90);
        hudRect.anchoredPosition = Vector2.zero;
        waveText = GetOrCreateText(hud.transform, "WaveText", "WAVE 1 / 5", 42, Color.yellow, new Vector2(0.5f, 1f), new Vector2(0, -10), TextAnchor.UpperCenter);
        waveStatusText = GetOrCreateText(hud.transform, "WaveStatusText", "READY", 20, Color.white, new Vector2(0.5f, 1f), new Vector2(0, -54), TextAnchor.UpperCenter);
        plasmaText = GetOrCreateText(hud.transform, "PlasmaText", "PLASMA 0", fontSize, Color.cyan, new Vector2(0, 1f), new Vector2(28, -18), TextAnchor.UpperLeft);
        healthText = GetOrCreateText(hud.transform, "HealthText", "HEALTH 20/20", fontSize, Color.green, new Vector2(1f, 1f), new Vector2(-28, -18), TextAnchor.UpperRight);
        healthBarFill = CreateBar(hud.transform, "HealthBar", new Vector2(1f, 1f), new Vector2(-28, -58), new Vector2(250, 14), new Color(0.1f, 0.85f, 0.25f, 1f));

        announcementPanel = CreatePanel("WaveAnnouncement", canvas.transform, new Color(0, 0, 0, 0.85f));
        announcementText = CreateText("AnnouncementText", announcementPanel.transform, "WAVE 1", 64, Color.white, new Vector2(0.5f, 0.5f), Vector2.zero, TextAnchor.MiddleCenter);
        announcementPanel.SetActive(false);

        resultPanel = CreatePanel("GameResult", canvas.transform, new Color(0, 0, 0, 0.92f));
        resultTitle = CreateText("ResultTitle", resultPanel.transform, "", 64, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0, 80), TextAnchor.MiddleCenter);
        resultSubtitle = CreateText("ResultSubtitle", resultPanel.transform, "", 28, Color.white, new Vector2(0.5f, 0.5f), new Vector2(0, 10), TextAnchor.MiddleCenter);
        CreateButton("RestartButton", resultPanel.transform, "RESTART", new Vector2(0, -80), Restart);
        resultPanel.SetActive(false);
    }

    private GameObject CreateObject(string name, Transform parent)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        return obj;
    }

    private GameObject CreatePanel(string name, Transform parent, Color color)
    {
        var panel = CreateObject(name, parent);
        var rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        panel.AddComponent<Image>().color = color;
        return panel;
    }

    private Text GetOrCreateText(Transform parent, string name, string value, int size, Color color, Vector2 anchor, Vector2 position, TextAnchor alignment)
    {
        var existing = parent.Find(name);
        if (existing != null)
        {
            var existingText = existing.GetComponent<Text>();
            if (existingText != null) return existingText;
        }
        return CreateText(name, parent, value, size, color, anchor, position, alignment);
    }

    private Image CreateBar(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, Color color)
    {
        var backgroundObject = CreateObject(name, parent);
        var backgroundRect = backgroundObject.AddComponent<RectTransform>();
        backgroundRect.anchorMin = anchor;
        backgroundRect.anchorMax = anchor;
        backgroundRect.pivot = new Vector2(1f, 1f);
        backgroundRect.sizeDelta = size;
        backgroundRect.anchoredPosition = position;
        backgroundObject.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.85f);

        var fillObject = CreateObject("Fill", backgroundObject.transform);
        var fillRect = fillObject.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        var fill = fillObject.AddComponent<Image>();
        fill.color = color;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.fillAmount = 1f;
        return fill;
    }

    private Text CreateText(string name, Transform parent, string value, int size, Color color, Vector2 anchor, Vector2 position, TextAnchor alignment)
    {
        var obj = CreateObject(name, parent);
        var rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.sizeDelta = new Vector2(560, 90);
        rect.anchoredPosition = position;
        rect.localScale = Vector3.one;
        var text = obj.AddComponent<Text>();
        text.font = runtimeFont;
        text.text = value;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.alignment = alignment;
        return text;
    }

    private void CreateButton(string name, Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        var buttonObject = CreateObject(name, parent);
        var rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(260, 60);
        rect.anchoredPosition = position;
        buttonObject.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 1f);
        var button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(action);
        CreateText("Label", buttonObject.transform, label, 24, Color.white, new Vector2(0.5f, 0.5f), Vector2.zero, TextAnchor.MiddleCenter);
    }

    private void RefreshValues()
    {
        if (WaveManager.Instance != null) UpdateWave(WaveManager.Instance.currentWaveIndex + 1, WaveManager.Instance.TotalWaves > 0 ? WaveManager.Instance.TotalWaves : totalWavesFallback);
        if (LeakManager.Instance != null) UpdateHealth(LeakManager.Instance.CurrentLeakCount, LeakManager.Instance.MaxLeakCapacity);
        if (PlasmaManager.Instance != null) UpdatePlasma(PlasmaManager.Instance.CurrentPlasma);
    }

    private void UpdateWave(int current, int total)
    {
        int safeTotal = Mathf.Max(1, total);
        if (waveText != null) waveText.text = $"WAVE {Mathf.Clamp(current, 1, safeTotal)} / {safeTotal}";
        if (waveStatusText != null)
            waveStatusText.text = WaveManager.Instance != null && WaveManager.Instance.isWaveSpawning ? "INCOMING ENEMIES" : "READY TO DEPLOY";
    }

    private void UpdateHealth(int current, int max)
    {
        if (healthText != null) healthText.text = $"HEALTH {current}/{max}";
        if (healthBarFill != null) healthBarFill.fillAmount = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
        if (healthText != null) healthText.color = current <= max * 0.25f ? Color.red : current <= max * 0.5f ? Color.yellow : Color.green;
    }

    private void UpdatePlasma(int amount)
    {
        if (plasmaText != null) plasmaText.text = $"PLASMA {amount}";
    }

    private int GetWaveCount()
    {
        return WaveManager.Instance != null && WaveManager.Instance.TotalWaves > 0
            ? WaveManager.Instance.TotalWaves
            : totalWavesFallback;
    }

    private void UpdateState(GameState state)
    {
        if (state == GameState.Victory) ShowResult("VICTORY!", "All waves completed", true);
        if (state == GameState.Defeat) ShowResult("DEFEAT", "Base destroyed", false);
        if (state == GameState.Preparation || state == GameState.MainMenu)
            HideResult();
    }

    public void ShowAnnouncement(string value)
    {
        if (announcementPanel == null) return;
        announcementText.text = value;
        announcementPanel.SetActive(true);
        CancelInvoke(nameof(HideAnnouncement));
        Invoke(nameof(HideAnnouncement), 2.5f);
    }

    private void HideAnnouncement()
    {
        if (announcementPanel != null) announcementPanel.SetActive(false);
    }

    public void ShowResult(string title, string subtitle, bool victory)
    {
        if (resultPanel == null) return;
        resultTitle.text = title;
        resultTitle.color = victory ? Color.green : Color.red;
        resultSubtitle.text = subtitle;
        resultPanel.SetActive(true);
    }

    private void HideResult()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        HideResult();

        if (LeakManager.Instance != null)
            LeakManager.Instance.ResetLeaks();

        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.ResetRun();
        }

        if (GameManager.Instance != null)
            GameManager.Instance.SetGameState(GameState.Preparation);

        var enemies = FindObjectsByType<MicroDefense.Enemies.EnemyBase>();
        foreach (var enemy in enemies)
            Destroy(enemy.gameObject);

        RefreshValues();
    }
}
