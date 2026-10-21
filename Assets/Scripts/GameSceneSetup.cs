using MicroDefense.Core;
using UnityEngine;

public sealed class GameSceneSetup : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        EnsureManager<GameManager>("GameManager");
        EnsureManager<PlasmaManager>("PlasmaManager");
        EnsureManager<LeakManager>("PlayerHealth");
        EnsureManager<WaveManager>("WaveManager");

        var ui = FindAnyObjectByType<SimpleUI>();
        if (ui == null)
        {
            var uiObject = new GameObject("SimpleUI");
            ui = uiObject.AddComponent<SimpleUI>();
        }
    }

    private static void EnsureManager<T>(string objectName) where T : Component
    {
        if (FindAnyObjectByType<T>() != null) return;
        var manager = new GameObject(objectName);
        manager.AddComponent<T>();
    }
}
