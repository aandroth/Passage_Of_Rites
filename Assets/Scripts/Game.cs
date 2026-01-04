using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    public int m_nextLevelIndex = 0;
    public float m_gameCountdownTime = 120.0f;
    public delegate void SignalReadinessDelegate(bool b = false);
    [SerializeField] protected string m_gameTitle = "";
    public enum GAME_STATE { INIT, PLAYING, GAME_OVER };
    protected GAME_STATE m_gameState = GAME_STATE.INIT;

    public delegate void GetPlayerNameAndPointsFromGameController(int idx);
    public GetPlayerNameAndPointsFromGameController m_getPlayerNameAndPointsFromGameController;

    public delegate void UpdatMainPlayerPoints(int idx);
    public UpdatMainPlayerPoints m_updateMainPlayerPoints;

    public struct PlayerInfo 
    {
        public int index;
        public string name;
        public string titles;
        public int points;
    }
    public virtual void StartGameIntro(SignalReadinessDelegate signalGameControllerReady = null) {  }
    public virtual void StartGameOutro(SignalReadinessDelegate signalGameControllerReady = null) { }
    public virtual int SetPlayerPointsAndGetBackTopPlayer(List<PlayerInfo> playInfoList) { return -1; }
    public virtual void AssignPlayer(PlayerControls playerControls, int id, bool isMainPlayer = false) { }
    public virtual string GetTitle() { return ""; }
    public virtual bool GameIsMiniGame() { return true; }
    public virtual int GameGetNextLevelIndex() { return m_nextLevelIndex; }
    public virtual float GameGetLevelCountdownTime() { return m_gameCountdownTime; }
    public virtual void StartGamePlaying(SignalReadinessDelegate signalGameControllerReady = null) {}
    public virtual GAME_STATE GetGameState() { return m_gameState; }
    public virtual IEnumerator EndGame(SignalReadinessDelegate signalGameControllerReady = null) { yield return null; }
    public virtual void SendNameToTitleSceneController(string name) {}
    public virtual string GetGameTitle() { return m_gameTitle; }

    public virtual void UpdateOtherPlayerPoints(int playerIndex, int points) { }

}
