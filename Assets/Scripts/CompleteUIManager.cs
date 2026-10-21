using MicroDefense.Core;
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
        DontDestroyOnLoad(gameObject);
    }

    public void ShowWaveAnnouncement(string text)
    {
        SimpleUI.Instance.ShowAnnouncement(text);
    }

    public void ShowVictory()
    {
        SimpleUI.Instance.ShowResult("VICTORY!", "All waves completed", true);
    }

    public void ShowDefeat()
    {
        SimpleUI.Instance.ShowResult("DEFEAT", "Base destroyed", false);
    }
}
