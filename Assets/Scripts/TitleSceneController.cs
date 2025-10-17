using TMPro;
using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
    [SerializeField]
    TMP_Text m_ipAddressText;
    [SerializeField]
    Backend m_backend = null;

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
}
