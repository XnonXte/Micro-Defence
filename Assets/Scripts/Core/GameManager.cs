using System;
using UnityEngine;

namespace MicroDefense.Core
{
    public enum GameState
    {
        MainMenu,
        Preparation,
        WaveInProgress,
        Paused,
        Defeat,
        Victory
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; } = GameState.Preparation;

        public event Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            SetGameState(GameState.Preparation);
        }

        public void SetGameState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            Time.timeScale = (newState == GameState.Paused) ? 0f : 1f;

            OnGameStateChanged?.Invoke(newState);
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.WaveInProgress || CurrentState == GameState.Preparation)
            {
                SetGameState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                SetGameState(GameState.Preparation);
            }
        }
    }
}
