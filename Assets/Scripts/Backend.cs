using NUnit.Framework;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using NativeWebSocket;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Collections;
using System;

public class Backend : MonoBehaviour
{
    public delegate void ReceivedMessageForGameControllerDelegate(string s, string t, string[] p);
    public ReceivedMessageForGameControllerDelegate ReceivedMessageForGameController;
    public delegate string GetPlayerDataDelegate();
    public GetPlayerDataDelegate GetPlayerData;
    public delegate void SetPlayerChangedDataToCurrentValuesDelegate();
    public SetPlayerChangedDataToCurrentValuesDelegate SetPlayerChangedDataToCurrentValues;
    public delegate void GetTextDataDelegate(string s);
    public GetTextDataDelegate GetTextData;

    public string m_apiGatewayUrl = "https://t2lfwpskr0.execute-api.us-west-2.amazonaws.com/dev";
    public string m_serverUrl = "18.237.4.137"; // ws://localhost
    public WebSocket m_webSocket;
    public int m_webSocketConnectionAttemptsToTry = 3;
    public float intervalTimeCurr = 0f;
    public float intervalTime = 0.3f;
    public bool m_connected = false;
    public string urlResult;
    //public string m_defaultUrl = "localhost:3000/hello";

    public async void StartWebSocketConnection()
    {
        m_webSocket = new WebSocket($"ws://{m_serverUrl}:5000");

        m_webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            m_connected = true;
        };

        m_webSocket.OnError += (e) =>
        {
            Debug.Log("Connection error!" + e.ToString());
            Debug.Log("Error! " + e.ToString());
        };

        m_webSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed! " + e);
            m_connected = false;
        };

        m_webSocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            Debug.Log(bytes);

            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage! " + message);
            ReceivedMessage(message);
        };

        Debug.Log($"Trying to connect to websocket at {m_serverUrl}");
        // waiting for messages
        await m_webSocket.Connect();
    }


    public void RequestNewServer(Action<string> callbackFn)
    {
        StartCoroutine(RequestNewServerCoroutine(callbackFn));
    }

    public IEnumerator RequestNewServerCoroutine(Action<string> callbackFn)
    {
        Debug.Log($"Requesting new server at {m_apiGatewayUrl}");
        using (UnityWebRequest serverRequest = UnityWebRequest.Get(m_apiGatewayUrl))
        {
            Debug.Log($"Request made");
            yield return serverRequest.SendWebRequest();
            string errorString = "There was an error? Of course there was an error. Why couldn't it just work!?\n- you, probably";

            switch (serverRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log(errorString);
                    Debug.Log(serverRequest.result);
                    m_serverUrl = "Bad Result";
                    break;
                case UnityWebRequest.Result.Success:
                    m_serverUrl = serverRequest.downloadHandler.text;
                    Debug.Log($"RequestNewServer SUCCESS: {(m_serverUrl)}");
                    break;
            }
        }
        Debug.Log($"Request finished");
        callbackFn(m_serverUrl);
    }

    public void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if(m_connected)
            m_webSocket.DispatchMessageQueue();
#endif
        if (Input.GetKeyUp(KeyCode.I))
        {
            Debug.Log("Calling API Gateway");
            StartCoroutine(RequestNewServerCoroutine((str) => { Debug.Log($"API called from keystroke with result: {str}"); }));
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            Debug.Log("Calling connection");
            StartWebSocketConnection();
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            Debug.Log("Cancelling connection");
            CancelConnection();
        }

        if (m_connected && GetPlayerData != null)
        {
            intervalTimeCurr += Time.deltaTime;
            if (intervalTimeCurr >= intervalTime)
            {
                intervalTimeCurr = 0;
                string changes = GetPlayerData();
                if (changes != "Unchanged")
                {
                    changes = $"Update{changes}";
                    Debug.Log($"Sending: {changes}");
                    var bytes = System.Text.Encoding.UTF8.GetBytes(changes);
                    m_webSocket.Send(bytes);
                    SetPlayerChangedDataToCurrentValues();
                }
            }
        }
    }

    public void OnDestroy()
    {
        CancelConnection();
    }

    public void ReturnUrlResult(string url = "localhost:3000/hello")
    {
        Debug.Log("ReturnUrlResult");
        StartCoroutine(ReturnUrlResultCoroutine(url));
    }

    IEnumerator ReturnUrlResultCoroutine(string url)
    {
        Debug.Log($"ReturnUrlResultCoroutine to url {url}");
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            urlResult = uwr.error;
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            urlResult = uwr.downloadHandler.text;
        }
        GetTextData.Invoke(urlResult);
    }

    public void CancelConnection()
    {
        if (m_webSocket != null && m_connected)
        {
            m_webSocket.CancelConnection();
        }
        m_connected = false;
    }

    public void SendServerDataToGameController(string data, string action, string[] playerData)
    {
        if (ReceivedMessageForGameController == null)
        {
            Debug.LogError($"ReceivedMessageForGameController is null");
            return;
        }

        ReceivedMessageForGameController(data, action, playerData);
    }

    public void ReceivedMessage(string raw_data)
    {
        string data = raw_data.Substring(1, raw_data.Length - 2);
        string[] playerData = data.Split(',');
        string action = playerData.Length > 0 ? playerData[0] : "Disconnect";
        Debug.Log($"Received: {data} with action {action}");

        switch (action)
        {
            case "Disconnect":
                CancelConnection();
                break;
            case "Player":
            case "NewPlayer":
            case "Update":
                SendServerDataToGameController(data, action, playerData);
                break;
            default:
                break;
        }
    }
}
