using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DEV_StartGame : MonoBehaviour
{
    [SerializeField] GameController m_gameController = null;
    [SerializeField] Backend m_backend = null;
    [SerializeField] string m_playerName = "FAKER";
    [SerializeField] int m_id = 0;
    [SerializeField] int m_numberOfOtherPlayers = 0;
    [SerializeField] FakeServer m_fakeServer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_backend = FindAnyObjectByType<Backend>();
        m_fakeServer.StartServer();
        StartCoroutine(StartGameAfterDelay(14.0f));
    }

    public IEnumerator StartGameAfterDelay(float delay)
    {
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        Debug.Log($"Starting DEV game with id: {m_id} and player name: {m_playerName}");
        m_backend.RequestServerToLoadLevel(SceneManager.GetActiveScene().buildIndex);

        //m_gameController.ReceivedMessage($"Init,{m_id}", "Init", new string[] { $"Init", $"{m_id}" });
        //m_gameController.ReceivedMessage($"Make_Owner,{m_id},t", "Make_Owner", new string[] { "Make_Owner", $"{m_id}","t"});
        //m_gameController.ReceivedMessage($"New_Player,{m_id},t", "New_Player", new string[] { "New_Player", $"{m_id}", "0", "0", "1", "0", "0", "", "0", "0", $"{m_playerName}", "0.25|0.5|1"});

        //int maxNumberOfOtherPlayers = 4;
        //for (int id = 0, count = 0;  id < maxNumberOfOtherPlayers && count < m_numberOfOtherPlayers; ++id)
        //{
        //    if (id == m_id) continue;

        //    int otherPlayerId = id;
        //    m_gameController.ReceivedMessage($"New_Player,{otherPlayerId},f", "New_Player", new string[] { "New_Player", $"{otherPlayerId}", "0", "0", "1", "0", "0", "", "0", "0", $"Kobold_{otherPlayerId}", $"{1/(Mathf.Pow(2, id))}|{1 / (Mathf.Pow(2, id))}|{1 / (Mathf.Pow(2, id))}" });
        //    ++count;
        //} 
        //m_gameController.ReceivedMessage($"Start_Intro,{m_id}", "Start_Intro", new string[] { "Start_Intro", $"{m_id}" });

        //float introDelay = 15.0f;
        //while (introDelay > 0)
        //{
        //    introDelay -= Time.deltaTime;
        //    yield return null;
        //}
        //m_gameController.ReceivedMessage("Start_Countdown,0", "Start_Countdown", new string[] { "Start_Countdown", "0" });
    }
}
