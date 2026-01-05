using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public float m_pullChangedDataInterval;
    public Dictionary<int, PlayerControls> m_playersDict = new Dictionary<int, PlayerControls>();
    public Dictionary<int, NetworkDataObject_Npc> m_npcsDict = new Dictionary<int, NetworkDataObject_Npc>();
    public Dictionary<int, PlayerControls> m_itemsDict = new Dictionary<int, PlayerControls>();
    public GameObject m_playerPrefab;
    public int m_mainPlayerId = -1;
    public bool m_isGameOwner;

    public Backend m_backend;
    public Game m_game = null;
    [SerializeField] string m_titleSceneName = "TitleScene";
    [SerializeField] string m_endSceneName = "EndingScene";
    [SerializeField] bool m_gameIsPlaying = false;


    public static GameController Instance { get; private set; }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);

            m_backend.GetPlayerData = GetPlayerChangedData;
            m_backend.SetPlayerChangedDataToCurrentValues = SetPlayerChangedDataToCurrentValues;
            m_backend.ReceivedMessageForGameController = ReceivedMessage;
            m_backend.GetIdFromGameController = () => { return m_mainPlayerId; };
        }
    }

    public void UpdateCharacter(int id, string[] playerData)
    {
        if (m_playersDict.ContainsKey(id))
            m_playersDict[int.Parse(playerData[1])].PutChangedData(playerData);
    }

    public void UpdateNpc(int id, string[] npcData)
    {
        if (m_playersDict.ContainsKey(id))
            m_npcsDict[int.Parse(npcData[1])].PutChangedData(npcData);
    }

    public void UpdateItem(int id, string[] playerData)
    {
        if (m_playersDict.ContainsKey(id))
            m_playersDict[int.Parse(playerData[1])].PutChangedData(playerData);
    }

    public void SetPlayerLocation(Vector3 position)
    {
        m_playersDict[m_mainPlayerId].transform.position = position;
    }

    public PlayerControls CreateCharacter(bool isMainPlayer, int id, string[] data)
    {
        GameObject go = GameObject.Instantiate(m_playerPrefab, Vector3.zero, Quaternion.identity);
        go.GetComponent<PlayerControls>().m_id = id;
        go.GetComponent<PlayerControls>().m_isMainPlayer = isMainPlayer;
        go.GetComponent<PlayerControls>().SetPlayerAsMainOrOther(isMainPlayer);
        go.GetComponent<PlayerControls>().PutAllData(data);
        m_playersDict[id] = go.GetComponent<PlayerControls>();
        m_mainPlayerId = isMainPlayer ? id : m_mainPlayerId;
        Debug.Log($"{go.GetComponent<PlayerControls>().m_nameTextMesh.text} created");
        return go.GetComponent<PlayerControls>();
    }

    public void BecomeGameOwner(bool becameOwner = true)
    {
        Debug.Log($"Ownership is now marked as {becameOwner}");

        m_isGameOwner = becameOwner;

        TitleSceneController titleSceneController = (TitleSceneController)GameObject.FindAnyObjectByType(typeof(TitleSceneController));
        if (titleSceneController != null)
        {
            Debug.Log($"Found TitleSceneController");
            titleSceneController.BecomeGameOwner(becameOwner);
        }
    }

    public string GetPlayerChangedData()
    {
        return m_mainPlayerId == -1 ? "" : m_playersDict[m_mainPlayerId].GetChangedData();
    }
    public void SetPlayerChangedDataToCurrentValues()
    {
        if(m_mainPlayerId != -1) m_playersDict[m_mainPlayerId].SetChangedDataToCurrentValues();
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (this == Instance) {
            m_game = GameObject.FindAnyObjectByType<Game>();
            if (m_game != null)
            {
                m_backend.SignalReadinessToServer(m_mainPlayerId);
                if (scene.name == m_titleSceneName && m_mainPlayerId != -1)
                {
                    m_game.SendNameToTitleSceneController(m_playersDict[m_mainPlayerId].m_nameTextMesh.text);
                    if(m_backend.m_connected)
                        m_backend.RequestKillServer();
                }
                else if(scene.name != m_endSceneName)
                {

                }
            }
        }
    }

    public void DestroyPlayer()
    {
        if(m_mainPlayerId != -1)
        {
            m_playersDict[m_mainPlayerId].DestroySelf();
            m_playersDict.Remove(m_mainPlayerId);
            m_mainPlayerId = -1;
        }
    }

    public void CallbackForGameControllerSendReadySignal(bool isEndScene = false)
    {
        if(isEndScene) 
            m_backend.RequestServerToLoadLevel(0);

        if (m_gameIsPlaying && m_game != null)
        {
            List<Game.PlayerInfo> playerInfos = new List<Game.PlayerInfo>();
            foreach (var player in m_playersDict)
            {
                playerInfos.Add(new Game.PlayerInfo
                {
                    index = player.Key,
                    name = player.Value.m_nameTextMesh.text,
                    points = player.Value.m_points
                });
            }
            m_gameIsPlaying = false;
            if (m_playersDict.Count > 0)
            {
                int topPlayerIndex = m_game.SetPlayerPointsAndGetBackTopPlayer(playerInfos);
                if (topPlayerIndex == m_mainPlayerId)
                {
                    //m_playersDict[m_mainPlayerId].m_titles += m_playersDict[m_mainPlayerId].m_titles.Length > 0 ? $" {m_game.GetTitle()}" : m_game.GetTitle();
                    m_backend?.SendPlayerChangedData();
                }
            }
        }
        else
            m_backend?.SignalReadinessToServer(m_mainPlayerId);
    }

    public void ReceivedMessage(string data, string action, string[] playerData)
    {
        int id = -1;
        if (playerData.Length >= 2)
        {
            try
            {
                id = playerData.Length >= 2 ? int.Parse(playerData[1]) : -1;
                Debug.Log($"Received: {data} with action {action} and id {id}, playerData length: {playerData.Length}");
            }
            catch
            {
                Debug.LogWarning($"Failed to parse player id from data: {data}");
            }
        }

        switch (action)
        {
            case "Init":
                m_mainPlayerId = id;
                break;
            case "Make_Owner":
                if(playerData.Length >= 3)
                    BecomeGameOwner(playerData[2] == "t" ? true : false);
                break;
            case "Load_Level":
                if (playerData.Length >= 2)
                {
                    int levelIndex = int.Parse(playerData[1]);
                    if (levelIndex == 0 && m_backend != null && m_backend.m_connected) { m_backend?.CancelConnection(); }
                    SceneManager.LoadScene(levelIndex);
                }
                break;
            case "Start_Intro":
                if (playerData.Length >= 2)
                {
                    Debug.Log("Calling GameIntro");
                    m_game?.StartGameIntro(CallbackForGameControllerSendReadySignal);
                    Debug.Log("Called GameIntro");
                }
                break;
            case "Call_Countdown":
                if (m_isGameOwner)
                {
                    m_backend?.RequestServerToStartCountdown(m_game.GameGetLevelCountdownTime());
                }
                break;
            case "Start_Countdown":
                m_game?.StartGamePlaying(CallbackForGameControllerSendReadySignal);
                m_gameIsPlaying = true;
                m_backend.StartGettingChangedData();
                break;
            case "Stop_Game":
                if (!(m_game.GetGameState() == Game.GAME_STATE.GAME_OVER))
                    m_game?.EndGame(CallbackForGameControllerSendReadySignal);
                break;
            case "Start_Outro":
                Debug.Log("Calling GameOutro");
                m_game?.StartGameOutro(CallbackForGameControllerSendReadySignal);
                Debug.Log("Called GameOutro");
                break;
            case "Ready_For_Next_Level":
                if (m_isGameOwner)
                {
                    m_backend.RequestServerToLoadLevel(m_game.GameGetNextLevelIndex());
                }
                m_backend.StopGettingChangedData();
                break;
            case "New_Player":
                PlayerControls pc = CreateCharacter(m_mainPlayerId == id, id, playerData);
                m_game?.AssignPlayer(pc, id, m_mainPlayerId == id);
                if (m_mainPlayerId == id)
                    m_backend.SignalReadinessToServer(id);
                break;
            case "Update_Player":
                UpdateCharacter(id, playerData);
                if (id != m_mainPlayerId && playerData[8] != "") m_game?.UpdateOtherPlayerPoints(id, int.Parse(playerData[8]));
                break;
            case "Update_Npc":
                UpdateCharacter(id, playerData);
                if (id != m_mainPlayerId && playerData[8] != "") m_game?.UpdateOtherPlayerPoints(id, int.Parse(playerData[8]));
                break;
            case "Update_Item":
                UpdateCharacter(id, playerData);
                if (id != m_mainPlayerId && playerData[8] != "") m_game?.UpdateOtherPlayerPoints(id, int.Parse(playerData[8]));
                break;
        }
    }
}
