using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField]
    TMP_Text m_ipAddressText;
    [SerializeField]
    Backend m_backend = null;
    public GameObject m_buttonPrefab;
    public GameObject m_buttonParent;
    public float      m_buttonOffset;
    //public Transform  m_buttonStartPos;
    [SerializeField]
    List<GameObject> m_ipButtonsList = new List<GameObject>();

    public void CallBackendForNewServer()
    {
        Debug.Log($"Button clicked");
        if (m_backend == null)
        {
            Debug.Log($"No backend found");
            return;
        }

        m_backend.RequestNewServer(WriteTextInIpAddressText);
    }
    public void WriteTextInIpAddressText(string str)
    {
        m_ipAddressText.text = str;
    }

    public void CallBackendForServerConnect()
    {
        Debug.Log($"Connect called");
        if (m_backend == null)
        {
            Debug.Log($"No backend found");
            return;
        }

        m_backend.StartWebSocketConnection();
    }
    public void CallBackendForServerDisconnect()
    {
        Debug.Log($"Disconnect called");
        m_backend.CancelConnection();
    }

    public void CallBackendForServerOptions()
    {
        Debug.Log($"Connect called");
        if (m_backend == null)
        {
            Debug.Log($"No backend found");
            return;
        }

        m_backend.RequestListOfServers(ParseServerList);
    }

    public void ParseServerList(string[] serverListResult)
    {
        if (serverListResult.Length == 0)
            WriteTextInIpAddressText("No active servers found.");
        else
        {
            DestroyButtonsInIpAddressPanel();
            CreateButtonsInIpAddressPanel(serverListResult);
        }
    }

    public void CreateButtonsInIpAddressPanel(string[] servers)
    {
        for(int i = 0; i < servers.Length; ++i)
        {
            GameObject newButton = Instantiate(m_buttonPrefab, m_buttonParent.transform);
            newButton.GetComponent<IpButton>().AssignButtonParameters(servers[i], m_ipAddressText, m_backend.SetServerUrl, m_backend.StartWebSocketConnection);
        }
    }

    public void DestroyButtonsInIpAddressPanel()
    {
        foreach (var item in m_ipButtonsList)
            Destroy(item);
    }


    //public void CallBackendForServerDisconnect()
    //{
    //    Debug.Log($"Disconnect called");
    //    m_backend.CancelConnection();
    //}
}
