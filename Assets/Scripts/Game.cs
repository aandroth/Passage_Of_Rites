using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Game : MonoBehaviour
{
    [SerializeField] public bool m_skipIntro = false;
    public int m_nextLevelIndex = 0;
    public float m_gameCountdownTime = 120.0f;
    public delegate void SignalReadinessDelegate(bool b = false);
    [SerializeField] protected string m_gameTitle = "";
    public List<Spawner_Npc> m_npcSpawners;
    public List<Spawner_ItemObjective> m_itemObjectiveSpawners;
    public enum GAME_STATE { INIT, PLAYING, GAME_OVER };
    protected GAME_STATE m_gameState = GAME_STATE.INIT;

    public delegate void GetPlayerNameAndPointsFromGameController(int idx);
    public GetPlayerNameAndPointsFromGameController m_getPlayerNameAndPointsFromGameController;

    public delegate void UpdatMainPlayerPoints(int idx);
    public UpdatMainPlayerPoints m_updateMainPlayerPoints;
    public delegate void RequestServerSpawnNpcDelegate(NetworkDataObject_Npc n);
    public RequestServerSpawnNpcDelegate m_requestServerSpawnNpc;

    public delegate void RequestServerGetAllDataNpcDelegate();
    public RequestServerGetChangedDataNpcDelegate m_requestServerGetChangedDataNpc;
    public delegate void RequestServerGetChangedDataNpcDelegate(NetworkDataObject_Npc n);
    public RequestServerGetChangedDataNpcDelegate m_requestServerGetAllDataNpc;

    public delegate void RequestServerPutAllDataNpcDelegate(NetworkDataObject_Npc n);
    public RequestServerPutChangedDataNpcDelegate m_requestServerPutChangedDataNpc;
    public delegate void RequestServerPutChangedDataNpcDelegate(NetworkDataObject_Npc n);
    public RequestServerPutChangedDataNpcDelegate m_requestServerPutAllDataNpc;

    //[SerializeField] List<>

    public struct PlayerInfo 
    {
        public int index;
        public string name;
        public string titles;
        public int points;
    }
    public virtual void SetSpawnerRequestDelegatesAndIndexes(RequestServerSpawnNpcDelegate Spawn, Action<int> Despawn, Func<bool> IsOwner) { }
    public virtual void NpcSpawn(RequestServerSpawnNpcDelegate n) { }
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

    public virtual NetworkDataObject_Npc SpawnNpcFromServer(string[] data)
    {
        //"Action, id, spawnerId, NpcType, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,         2,       3,          4,          5,                      6,     7

        int spawnerId = int.Parse(data[2]);
        if (m_npcSpawners.Count > 0 && m_npcSpawners.Count > spawnerId)
        {
            NetworkDataObject_Npc newNpcNetworkData = m_npcSpawners[spawnerId].SpawnNpc().m_networkDataObjectNpc;
            newNpcNetworkData?.PutAllData(data); // If the spawner is already at max count, it will return null
            return newNpcNetworkData;
        }
        return null;
    }

}
