using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float m_pullChangedDataInterval;
    public Dictionary<int, PlayerControls> m_playersDict = new Dictionary<int, PlayerControls>();
    public GameObject m_playerPrefab;
    public int m_mainPlayerId = -1;
    public bool m_isGameOwner;

    public Backend m_backend;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Start()
    {
        m_backend.GetPlayerData = GetPlayerChangedData;
        m_backend.SetPlayerChangedDataToCurrentValues = SetPlayerChangedDataToCurrentValues;
        m_backend.ReceivedMessageForGameController = ReceivedMessage;
    }

    public void UpdateCharacter(int id, string[] playerData)
    {
        if (m_playersDict.ContainsKey(id))
            m_playersDict[int.Parse(playerData[1])].PutChangedData(playerData);
        //else
        //    CreateCharacter(false, id, playerData);
    }

    public void SetPlayerLocation(Vector3 position)
    {
        m_playersDict[m_mainPlayerId].transform.position = position;
    }

    public GameObject CreateCharacter(bool isMainPlayer, int id, string[] data)
    {
        Vector3 spawnPosition = WorkshopGame.GetSpawnLocationForId(id);

        GameObject go = GameObject.Instantiate(m_playerPrefab, spawnPosition, Quaternion.identity);
        go.GetComponent<PlayerControls>().m_id = id;
        go.GetComponent<PlayerControls>().m_isMainPlayer = isMainPlayer;
        go.GetComponent<PlayerControls>().SetPlayerAsMainOrOther(isMainPlayer);
        go.GetComponent<PlayerControls>().PutAllData(data);
        m_playersDict[id] = go.GetComponent<PlayerControls>();
        m_mainPlayerId = isMainPlayer ? id : m_mainPlayerId;
        Debug.Log($"{go.GetComponent<PlayerControls>().m_nameTextMesh.text} created");
        return go;
    }

    public void BecomeGameOwner(bool becameOwner = true)
    {
        Debug.Log($"Ownership is now marked as {becameOwner}");

        m_isGameOwner = becameOwner;

        TitleSceneController t = (TitleSceneController)GameObject.FindAnyObjectByType(typeof(TitleSceneController));
        if (t != null)
        {
            Debug.Log($"Found TitleSceneController");
            t.BecomeGameOwner(becameOwner);
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

    public void DestroyPlayer()
    {
        if(m_mainPlayerId != -1)
        {
            m_playersDict[m_mainPlayerId].DestroySelf();
            m_playersDict.Remove(m_mainPlayerId);
            m_mainPlayerId = -1;
        }
    }

    public void ReceivedMessage(string data, string action, string[] playerData)
    {
        int id = int.Parse(playerData[1]);
        Debug.Log($"Received: {data} with action {action} and id {id}");

        switch (action)
        {
            case "Init":
                m_mainPlayerId = id;
                break;
            case "Make_Owner":
                if(playerData.Length >= 3)
                    BecomeGameOwner(playerData[2] == "t" ? true : false);
                break;
            case "Start_Game":
                if(playerData.Length >= 3)
                    BecomeGameOwner(playerData[2] == "t" ? true : false);
                break;
            case "Player":
                CreateCharacter(true, m_mainPlayerId, playerData);
                break;
            case "NewPlayer":
                if(m_mainPlayerId != id)
                    CreateCharacter(false, id, playerData);
                break;
            case "Update":
                UpdateCharacter(id, playerData);
                break;
        }
    }
}
