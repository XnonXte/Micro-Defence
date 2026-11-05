using UnityEngine;
using UnityEngine.SceneManagement;

namespace MicroDefense.Core
{
    /// <summary>
    /// Handles level unlock progression (Level 1-5).
    /// Level 1 is always unlocked. Winning a level unlocks the next level.
    /// </summary>
    public static class LevelProgress
    {
        private const string PREF_KEY = "UnlockedLevel";
        public const int MaxLevel = 5;

        public static int GetUnlockedLevel()
        {
            // Default: only level 1 unlocked
            return Mathf.Clamp(PlayerPrefs.GetInt(PREF_KEY, 1), 1, MaxLevel);
        }

        public static bool IsLevelUnlocked(int level)
        {
            if (level <= 1) return true;
            if (level > MaxLevel) return false;
            return GetUnlockedLevel() >= level;
        }

        public static void UnlockLevel(int level)
        {
            int clamped = Mathf.Clamp(level, 1, MaxLevel);
            int current = GetUnlockedLevel();
            if (clamped <= current) return;

            PlayerPrefs.SetInt(PREF_KEY, clamped);
            PlayerPrefs.Save();
            Debug.Log($"[LevelProgress] Unlocked level {clamped}");
        }

        /// <summary>
        /// Parses level number from scene name. Supports "1"-"5", "Level1"-"Level5", "level2" etc.
        /// </summary>
        public static bool TryGetLevelNumberFromScene(string sceneName, out int levelNumber)
        {
            levelNumber = 0;
            if (string.IsNullOrWhiteSpace(sceneName)) return false;

            string s = sceneName.Trim().ToLowerInvariant();
            if (s.StartsWith("level")) s = s.Substring("level".Length);
            s = s.TrimStart('_', ' ');

            if (int.TryParse(s, out levelNumber))
            {
                return levelNumber >= 1 && levelNumber <= MaxLevel;
            }

            return false;
        }

        public static bool TryGetCurrentLevelNumber(out int levelNumber)
        {
            return TryGetLevelNumberFromScene(SceneManager.GetActiveScene().name, out levelNumber);
        }

        public static void UnlockNextFromCurrentScene()
        {
            if (!TryGetCurrentLevelNumber(out int current)) return;
            if (current < MaxLevel)
            {
                UnlockLevel(current + 1);
            }
        }

        public static void ResetProgressForDebug()
        {
            PlayerPrefs.SetInt(PREF_KEY, 1);
            PlayerPrefs.Save();
        }
    }
}
