using MicroDefense.Core;
using MicroDefense.UI;
using UnityEngine;

public sealed class CompleteUIManager : MonoBehaviour
{
    private static CompleteUIManager instance;

    public static CompleteUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<CompleteUIManager>();
                if (instance == null)
                {
                    var obj = new GameObject("CompleteUIManager");
                    instance = obj.AddComponent<CompleteUIManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        // TIDAK pakai DontDestroyOnLoad agar retry bisa clean restart
        // DontDestroyOnLoad(gameObject); ← DIHAPUS!
    }

    public void ShowWaveAnnouncement(string text)
    {
        SimpleUI.Instance.ShowAnnouncement(text);
    }

    public void ShowVictory()
    {
        // Try to use GameOverUI first, fallback to SimpleUI
        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ShowWinPanel("All waves completed!");
        }
        else if (SimpleUI.Instance != null)
        {
            SimpleUI.Instance.ShowResult("VICTORY!", "All waves completed", true);
        }
    }

    public void ShowDefeat()
    {
        // Try to use GameOverUI first, fallback to SimpleUI
        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ShowLosePanel("Base destroyed!");
        }
        else if (SimpleUI.Instance != null)
        {
            SimpleUI.Instance.ShowResult("DEFEAT", "Base destroyed", false);
        }
    }
}
