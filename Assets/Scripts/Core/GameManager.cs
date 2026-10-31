using System;
using MicroDefense.Enemies;
using MicroDefense.Grid;
using MicroDefense.Towers;
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

        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        private GameState previousState = GameState.Preparation;

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

            if (CurrentState != GameState.Paused && newState == GameState.Paused)
            {
                previousState = CurrentState;
            }

            CurrentState = newState;
            
            // Standar Unity Pause: Cukup atur timeScale = 0 saat Paused/Defeat/Victory, = 1 saat main
            if (newState == GameState.Defeat || newState == GameState.Victory || newState == GameState.Paused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }

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
                // Kembalikan ke state sebelum pause (WaveInProgress atau Preparation)
                SetGameState(previousState);
            }
        }
    }
}
