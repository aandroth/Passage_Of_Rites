using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour 
{
    public float m_pullChangedDataInterval;
    public Dictionary<int, PlayerControls> m_playersDict = new Dictionary<int, PlayerControls>();
    public GameObject m_playerPrefab;
    public int m_playerId = -1;

    public Backend m_backend;

    public void Start()
    {
        m_backend.GetPlayerData = GetPlayerChangedData;
        m_backend.SetPlayerChangedDataToCurrentValues = SetPlayerChangedDataToCurrentValues;
        m_backend.ReceivedMessageForGameController = ReceivedMessage;
    }

    public void Update()
    {
        
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
        m_playersDict[m_playerId].transform.position = position;
    }

    public GameObject CreateCharacter(bool isPlayer, int id, string[] data)
    {
        GameObject go = GameObject.Instantiate(m_playerPrefab, Vector3.zero, Quaternion.identity);
        go.GetComponent<PlayerControls>().m_id = id;
        go.GetComponent<PlayerControls>().PutAllData(data);
        go.GetComponent<PlayerControls>().m_isPlayer = isPlayer;
        m_playersDict[id] = go.GetComponent<PlayerControls>();
        m_playerId = isPlayer ? id : m_playerId;
        Debug.Log($"{go.GetComponent<PlayerControls>().m_nameTextMesh.text} created");
        return go;
    }

    public string GetPlayerChangedData()
    {
        return m_playerId == -1 ? "" : m_playersDict[m_playerId].GetChangedData();
    }
    public void SetPlayerChangedDataToCurrentValues()
    {
        if(m_playerId != -1) m_playersDict[m_playerId].SetChangedDataToCurrentValues();
    }

    public void DestroyPlayer()
    {
        if(m_playerId != -1)
        {
            m_playersDict[m_playerId].DestroySelf();
            m_playersDict.Remove(m_playerId);
            m_playerId = -1;
        }
    }

    public void ReceivedMessage(string data, string action, string[] playerData)
    {
        int id = int.Parse(playerData[1]);
        Debug.Log($"Received: {data} with action {action} and id {id}");

        switch (action)
        {
            case "Player":
                m_playerId = id;
                CreateCharacter(true, m_playerId, playerData);
                break;
            case "NewPlayer":
                if(m_playerId != id)
                    CreateCharacter(false, id, playerData);
                break;
            case "Update":
                UpdateCharacter(id, playerData);
                break;
        }
    }
}
