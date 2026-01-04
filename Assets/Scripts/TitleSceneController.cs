using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

public class TitleSceneController : Game
{
    [SerializeField] TMP_Text m_ipAddressText;
    [SerializeField] Backend m_backend = null;
    public GameObject m_buttonPrefab;
    public GameObject m_buttonParent;
    public GameObject m_buttonStartConnection;
    public GameObject m_buttonStopConnection;
    public float      m_buttonOffset;
    //public Transform  m_buttonStartPos;
    [SerializeField] List<GameObject> m_ipButtonsList = new List<GameObject>();
    [SerializeField] UnityEngine.UI.Button m_startGameButton = null;
    [SerializeField] TMP_InputField m_playerNameInputField = null;
    [SerializeField] int m_firstLevel = 2;

    public void Start()
    {
        if (m_backend == null)
        {
            m_backend = Backend.Instance;

            if (m_backend == null)
            {
                Debug.Log($"No backend found");
            }
        }
    }

    public void CallBackendForNewServer()
    {
        m_backend?.RequestNewServer(WriteTextInIpAddressText);
    }
    public void WriteTextInIpAddressText(string str)
    {
        m_ipAddressText.text = str;
    }

    public async void CallBackendForServerConnect()
    {
        await m_backend?.StartWebSocketConnection();
    }
    public void CallBackendForServerDisconnect()
    {
        Debug.Log($"Disconnect called");
        m_backend?.CancelConnection();
    }
    public void CallBackendForServerKill()
    {
        Debug.Log($"Kill called");
        m_backend?.RequestKillServer();
        if(!m_buttonStartConnection.activeSelf) m_buttonStartConnection.SetActive(true);
        if(m_buttonStopConnection.activeSelf) m_buttonStopConnection.SetActive(false);
        if(m_startGameButton.gameObject.activeSelf) m_startGameButton.gameObject.SetActive(false);
        CallBackendForServerOptions();
    }

    public void CallBackendForServerOptions()
    {
        Debug.Log($"Connect called");
        m_backend?.RequestListOfServers(ParseServerList);
    }

    public async void CallBackendToLoadLevel()
    {
        Debug.Log($"load level called");


        if (m_backend == null)
        {
            Debug.Log($"No backend found");
            return;
        }

        Debug.Log($"Name is currently: {m_playerNameInputField.text}");
        if (m_backend.m_connected && !string.IsNullOrEmpty(m_playerNameInputField.text))
        {
            await m_backend.RequestServerToChangeName(m_playerNameInputField.text);
        }

        m_backend.RequestServerToLoadLevel(m_firstLevel);
    }

    public void ParseServerList(string[] serverListResult)
    {
        DestroyButtonsInIpAddressPanel();
        if (serverListResult.Length == 0)
        {
            WriteTextInIpAddressText("No active servers found.");
        }
        else
        {
            CreateButtonsInIpAddressPanel(serverListResult);
        }
    }

    public void CreateButtonsInIpAddressPanel(string[] servers)
    {
        if (m_backend == null)
        {
            Debug.Log($"No backend found");
            return;
        }
        for (int i = 0; i < servers.Length; ++i)
        {
            GameObject newButton = Instantiate(m_buttonPrefab, m_buttonParent.transform);
            newButton.GetComponent<IpButton>().AssignButtonParameters(servers[i], m_ipAddressText, m_backend.SetServerUrl);
            m_ipButtonsList.Add(newButton);
        }
    }

    public void DestroyButtonsInIpAddressPanel()
    {
        for (int i = 0; i < m_ipButtonsList.Count; ++i)
        {
            Destroy(m_ipButtonsList[i]);
            m_ipButtonsList.RemoveAt(i);
        }
    }

    public void BecomeGameOwner(bool becameOwner = true)
    {
        m_startGameButton?.gameObject.SetActive(becameOwner);
    }

    public override bool GameIsMiniGame()
    {
        return false;
    }

    public override void SendNameToTitleSceneController(string name)
    {
        m_playerNameInputField.text = name;
    }

    public void SwapStartAndStopConnectionButtons()
    {
        m_buttonStartConnection.SetActive(!m_buttonStartConnection.activeSelf);
        m_buttonStopConnection.SetActive(!m_buttonStopConnection.activeSelf);
    }
}
