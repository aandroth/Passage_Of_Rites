using System;
using System.Collections;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    public int m_nextLevelIndex = 0;
    public float m_gameCountdownTime = 120.0f;
    public delegate void SignalReadinessDelegate(bool b = false);
    public enum GAME_STATE { INIT, PLAYING, GAME_OVER };
    protected GAME_STATE m_gameState = GAME_STATE.INIT;

    public virtual void StartGameIntro(SignalReadinessDelegate signalGameControllerReady = null) {  }
    public virtual void StartGameOutro(SignalReadinessDelegate signalGameControllerReady = null) { }
    public virtual void SetPlayerPoints(string[] names, int[] points) { }
    public virtual void AssignPlayer(PlayerControls playerControls, int id, bool isMainPlayer = false) { }
    public virtual string GetTitle() { return ""; }
    public virtual bool GameIsMiniGame() { return true; }
    public virtual int GameGetNextLevelIndex() { return m_nextLevelIndex; }
    public virtual float GameGetLevelCountdownTime() { return m_gameCountdownTime; }
    public virtual void StartGamePlaying(SignalReadinessDelegate signalGameControllerReady = null) {}
    public virtual GAME_STATE GetGameState() { return m_gameState; }
    public virtual void EndGame(SignalReadinessDelegate signalGameControllerReady = null) {}
    public virtual void SendNameToTitleSceneController(string name) {}
}
