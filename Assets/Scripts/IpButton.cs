using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IpButton : MonoBehaviour
{
    [SerializeField]
    private string m_text = "";
    public TMP_Text m_ipAddressText;
    public delegate void PutIpIntoBackend(string t);
    public PutIpIntoBackend m_putIpIntoBackendDelegate;
    public delegate void ConnectToServer();
    public ConnectToServer m_connectToServer;

    public void AssignButtonParameters(string ipText, TMP_Text textMeshPro, PutIpIntoBackend putIp, ConnectToServer connectToServer)
    {
        m_text = ipText;
        m_ipAddressText = textMeshPro;
        m_putIpIntoBackendDelegate = putIp;
        m_connectToServer = connectToServer;
        gameObject.GetComponentInChildren<TMP_Text>().text = m_text;
        gameObject.GetComponent<Button>().onClick.AddListener(() => m_putIpIntoBackendDelegate(m_text));
        gameObject.GetComponent<Button>().onClick.AddListener(() => m_connectToServer());
        gameObject.GetComponent<Button>().onClick.AddListener(() => PutIpIntoIpTextPanel());
    }

    public void PutIpIntoIpTextPanel()
    {
        m_ipAddressText.text = m_text;
    }
}
