using NativeWebSocket;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Backend : MonoBehaviour
{
    public delegate void ReceivedMessageForGameControllerDelegate(string s, string t, string[] p);
    public ReceivedMessageForGameControllerDelegate ReceivedMessageForGameController;
    public delegate string GetPlayerDataDelegate();
    public GetPlayerDataDelegate GetPlayerData;
    public delegate string GetItemObjectiveDataDelegate();
    public GetItemObjectiveDataDelegate GetItemObjectiveData;
    public delegate void SetPlayerChangedDataToCurrentValuesDelegate();
    public SetPlayerChangedDataToCurrentValuesDelegate SetPlayerChangedDataToCurrentValues;
    public delegate void SetNpcChangedDataToCurrentValuesDelegate();
    public SetNpcChangedDataToCurrentValuesDelegate SetNpcChangedDataToCurrentValues;
    public delegate void SetItemObjectiveChangedDataToCurrentValuesDelegate();
    public SetItemObjectiveChangedDataToCurrentValuesDelegate SetItemObjectiveChangedDataToCurrentValues;
    public delegate void GetTextDataDelegate(string s);
    public GetTextDataDelegate GetTextData;
    public delegate int GetIdFromGameControllerDelegate();
    public GetIdFromGameControllerDelegate GetIdFromGameController;
    public delegate void GetAllNpcChangedDataFromGameControllerDelegate();
    public GetAllNpcChangedDataFromGameControllerDelegate GetAllNpcChangedDataFromGameController;

    public string m_apiGatewayUrl = "https://t2lfwpskr0.execute-api.us-west-2.amazonaws.com/dev";
    public string m_serverUrl = "localhost"; //"18.237.4.137";
    public string[] m_serverUrlList = new string[0];
    public WebSocket m_webSocket;
    public int m_webSocketConnectionAttemptsToTry = 3;
    public float m_intervalTimeCurr = 0f;
    public float m_intervalTime = 0.3f;
    public bool m_gameInProgress = false;
    public bool m_connected = false;
    public string urlResult;
    private int pings;
    private float pingTiming = 0, pingTimePrev = 0;
    [SerializeField] TMP_Text pingText;
    //public string m_defaultUrl = "localhost:3000/hello";

    public static Backend Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public class JsonClassList
    {
        public string statusCode;
        public string[] body;
    }
    public class JsonClassSingle
    {
        public string statusCode;
        public string body;
    }



    public async System.Threading.Tasks.Task StartWebSocketConnection(string playerName = "")
    {
        if(m_connected) await m_webSocket.Close();

        m_webSocket = new WebSocket($"ws://{m_serverUrl}:5000");

        m_webSocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            m_connected = true;
            if(playerName != "")
            {
                // Send name request to server
                int id = GetIdFromGameController();
            }
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
            CancelConnection();
        };

        m_webSocket.OnMessage += (bytes) =>
        {
            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            if(!message.Contains("Ping"))
                Debug.Log("OnMessage! " + message);
            ReceivedMessage(message);
        };

        Debug.Log($"Trying to connect to websocket at {m_serverUrl}");
        // waiting for messages
        await m_webSocket.Connect();
    }
    public void StartGettingChangedData()
    {
        m_gameInProgress = true;
    }
    public void StopGettingChangedData()
    {
        m_gameInProgress = false;
    }

    public void RequestNewServer(Action<string> callbackFn)
    {
        StartCoroutine(RequestNewServerCoroutine(callbackFn));
    }

    public IEnumerator RequestNewServerCoroutine(Action<string> callbackFn)
    {
        Debug.Log($"Requesting new server at {m_apiGatewayUrl}/CreateGame");
        using (UnityWebRequest serverRequest = UnityWebRequest.Get(m_apiGatewayUrl + "/CreateGame"))
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
                    var data = JsonUtility.FromJson<JsonClassSingle>(serverRequest.downloadHandler.text);
                    m_serverUrl = data.body;
                    //m_serverUrl = serverRequest.downloadHandler.text;
                    Debug.Log($"RequestNewServer SUCCESS: {(m_serverUrl)}");
                    break;
            }
        }
        Debug.Log($"Request finished");
        callbackFn(m_serverUrl);
    }
    

    public void RequestListOfServers(Action<string[]> callbackFn)
    {
        StartCoroutine(RequestListOfServersCoroutine(callbackFn));
    }

    public IEnumerator RequestListOfServersCoroutine(Action<string[]> callbackFn)
    {
        Debug.Log($"Requesting all existing servers at {m_apiGatewayUrl}/ListGames");
        using (UnityWebRequest serverRequest = UnityWebRequest.Get(m_apiGatewayUrl + "/ListGames"))
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
                    callbackFn(new string[] { "localhost" });
                    break;
                case UnityWebRequest.Result.Success:
                    var data = JsonUtility.FromJson<JsonClassList>(serverRequest.downloadHandler.text);
                    Debug.Log($"RequestListOfServers SUCCESS: {(m_serverUrl)}, {data.body.Length}");
                    callbackFn(data.body);
                    break;
            }
        }
        Debug.Log($"Request finished");
    }
    public void ServerPing()
    {
        pingTiming += (float)Time.timeAsDouble - pingTimePrev;
        pingTimePrev = (float)Time.timeAsDouble;
        ++pings;
        if(pingTiming > 1)
        {
            pingText.text = $"Ping: ({pings})";
            pingTiming = 0;
            pings = 0;
        }
    }
    public void PingToServer()
    {
        string pingRequest = $"Ping";
        var bytes = System.Text.Encoding.UTF8.GetBytes(pingRequest);
        m_webSocket?.Send(bytes);
    }
    public System.Threading.Tasks.Task RequestServerToChangeName(string name)
    {
        if (this == Instance)
        {
            //"Action, id, name
            //      0,  1,    2
            string nameChangeRequest = $"Change_Name,{GetIdFromGameController()},{name}";

            var bytes = System.Text.Encoding.UTF8.GetBytes(nameChangeRequest);
            m_webSocket.Send(bytes);
            Debug.Log($"Name change request finished");
        }

        return System.Threading.Tasks.Task.CompletedTask;
    }
    public System.Threading.Tasks.Task RequestServerToSetInterval()
    {
        if (this == Instance)
        {
            //"Action, id, name
            //      0,  1,    2
            string setIntervalRequest = $"Set_Interval,{GetIdFromGameController()},{this.m_intervalTime}";

            var bytes = System.Text.Encoding.UTF8.GetBytes(setIntervalRequest);
            m_webSocket?.Send(bytes);
            Debug.Log($"setIntervalRequest finished");
        }

        return System.Threading.Tasks.Task.CompletedTask;
    }
    public void RequestServerToLoadLevel(int levelIdx)
    {
        if (this == Instance && m_connected)
        {
            string loadLevelRequest = $"Load_Level,{levelIdx}";

            var bytes = System.Text.Encoding.UTF8.GetBytes(loadLevelRequest);
            m_webSocket?.Send(bytes);
            Debug.Log($"Request finished");
        }
    }
    public void RequestServerToStartCountdown(float levelCountdownTime)
    {
        if (this == Instance && m_connected)
        {
            string startCountdownRequest = $"Start_Countdown,{levelCountdownTime}";

            var bytes = System.Text.Encoding.UTF8.GetBytes(startCountdownRequest);
            m_webSocket?.Send(bytes);
            Debug.Log($"Request finished");
        }
    }
    public void SignalReadinessToServer(int id)
    {
        if (this == Instance && m_connected)
        {
            //"Action, id
            //      0,  1
            string readyToServer = $"Player_Ready,";

            var bytes = System.Text.Encoding.UTF8.GetBytes(readyToServer);
            m_webSocket?.Send(bytes);
            Debug.Log($"Request finished");
        }
    }
    public void RequestServerToStartGame()
    {
        if (this == Instance && m_connected)
        {
            //"Action, id
            //      0,  1
            string startGameRequest = $"Start_Game,";

            var bytes = System.Text.Encoding.UTF8.GetBytes(startGameRequest);
            m_webSocket?.Send(bytes);
            Debug.Log($"Request finished");
        }
    }


    public void RequestKillServer()
    {
        if (this == Instance && m_connected)
        {
            StartCoroutine(RequestKillServerCoroutine());
        }
    }

    public IEnumerator RequestKillServerCoroutine()
    {
        if (this == Instance && m_connected)
        {
            string startGameRequest = $"Kill_Game,";

            var bytes = System.Text.Encoding.UTF8.GetBytes(startGameRequest);
            m_webSocket?.Send(bytes);
            Debug.Log($"Request finished");
        }


        Debug.Log($"Requesting kill server at {m_apiGatewayUrl}/KillGame");
        using (UnityWebRequest serverRequest = UnityWebRequest.Get(m_apiGatewayUrl + "/KillGame"))
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
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log($"RequestListOfServers SUCCESS: {(m_serverUrl)}");
                    break;
            }
        }
        Debug.Log($"Request finished");
    }

    public void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if(m_connected)
            m_webSocket.DispatchMessageQueue();
#endif
        //if (Input.GetKeyUp(KeyCode.I))
        //{
        //    Debug.Log("Calling API Gateway");
        //    StartCoroutine(RequestNewServerCoroutine((str) => { Debug.Log($"API called from keystroke with result: {str}"); }));
        //}
        //if (Input.GetKeyUp(KeyCode.O))
        //{
        //    Debug.Log("Calling connection");
        //    StartWebSocketConnection();
        //}
        //if (Input.GetKeyUp(KeyCode.P))
        //{
        //    Debug.Log("Cancelling connection");
        //    CancelConnection();
        //}

        if (m_connected && GetPlayerData != null && m_gameInProgress)
        {
            m_intervalTimeCurr += Time.deltaTime;
            if (m_intervalTimeCurr >= m_intervalTime)
            {
                m_intervalTimeCurr = 0;
                SendPlayerChangedData(); 
                if(GameController.Instance.m_isGameOwner)
                    GetAllNpcChangedDataFromGameController();
                PingToServer();
            }
        }
    }

    public void SendPlayerChangedData()
    {
        string changes = GetPlayerData();
        if (changes != "Unchanged")
        {
            changes = $"Update_Player{changes}";
            Debug.Log($"Sending: {changes}");
            var bytes = System.Text.Encoding.UTF8.GetBytes(changes);
            m_webSocket?.Send(bytes);
            SetPlayerChangedDataToCurrentValues();
        }
    }

    public void SendNpcChangedData(NetworkDataObject_Npc npcData)
    {
        string changes = npcData.GetChangedData();
        if (changes != "Unchanged")
        {
            changes = $"Update_Npc{changes}";
            Debug.Log($"Sending: {changes}");
            var bytes = System.Text.Encoding.UTF8.GetBytes(changes);
            m_webSocket.Send(bytes);
            npcData?.SetChangedDataToCurrentValues();
        }
    }

    public void SendNpcSpawnData(NetworkDataObject_Npc npcData)
    {
        var data = npcData.GetAllData();
        string dataAsString = $"Spawn_Npc{data}";
        Debug.Log($"Sending: {dataAsString}");
        var bytes = System.Text.Encoding.UTF8.GetBytes(dataAsString);
        m_webSocket?.Send(bytes);
    }

    public void SendNpcDespawnData(int id)
    {
        string dataAsString = $"Despawn_Npc{id}";
        Debug.Log($"Sending: {dataAsString}");
        var bytes = System.Text.Encoding.UTF8.GetBytes(dataAsString);
        m_webSocket?.Send(bytes);
    }

    public void SendItemObjectiveChangedData()
    {
        string changes = GetItemObjectiveData();
        if (changes != "Unchanged")
        {
            changes = $"Update_ItemObjective{changes}";
            Debug.Log($"Sending: {changes}");
            var bytes = System.Text.Encoding.UTF8.GetBytes(changes);
            m_webSocket?.Send(bytes);
            SetItemObjectiveChangedDataToCurrentValues();
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
        if (this == Instance)
        {
            if (this?.m_webSocket != null && this.m_connected)
            {
                m_webSocket?.CancelConnection();
            }
            m_connected = false;
            SendServerDataToGameController("", "Make_Owner", new string[] { "", "-1", "f" }); // Stop being a Game Owner
            SendServerDataToGameController("", "Load_Level", new string[] { "Load_Level", "0" }); // Stop being a Game Owner
        }
    }

    public void SendServerDataToGameController(string data, string action, string[] playerData)
    {
        if (Instance.ReceivedMessageForGameController == null)
        {
            Debug.LogError($"ReceivedMessageForGameController is null");
            return;
        }

        Instance.ReceivedMessageForGameController(data, action, playerData);
    }

    public void ReceivedMessage(string raw_data)
    {
        string data = raw_data.Substring(1, raw_data.Length - 2);
        string[] playerData = data.Split(',');
        string action = playerData.Length > 0 ? playerData[0] : "Disconnect";
        if (!action.Contains("Ping"))
            Debug.Log($"Received: {data} with action {action}");

        switch (action)
        {
            case "Disconnect":
                CancelConnection();
                break;
            case "Init":
            case "Make_Owner":
            case "Load_Level":
            case "Start_Intro":
            case "Call_Countdown":
            case "Start_Countdown":
            case "Game_Ready":
            case "Stop_Game":
            case "Start_Outro":
            case "New_Player":
            case "New_Npc":
            case "New_ItemObjective":
            case "Ready_For_Next_Level":
            case "Update_Player":
            case "Update_Npc":
            case "Update_Item":
            case "Set_Interval":
                SendServerDataToGameController(data, action, playerData);
                break;
            case "Ping":
                ServerPing();
                break;
            default:
                Debug.LogWarning($"Unhandled action at backend: {action}");
                break;
        }
    }

    public void SetServerUrl(string url)
    {
        m_serverUrl = url;
    }
}
