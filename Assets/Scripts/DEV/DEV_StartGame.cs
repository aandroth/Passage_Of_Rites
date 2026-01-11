using UnityEngine;
using Unity.Collections;
using System.Collections;

public class DEV_StartGame : MonoBehaviour
{
    [SerializeField] GameController m_gameController = null;
    //[SerializeField] Backend m_backend = null;
    //[SerializeField] Game m_game = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StatGameAfterDelay(2.0f));
    }

    public IEnumerator StatGameAfterDelay(float delay)
    {
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        m_gameController.ReceivedMessage("Init,0", "Init", new string[] {"Init","0"});
        m_gameController.ReceivedMessage("Make_Owner,0,t", "Make_Owner", new string[] { "Make_Owner", "0","t"});
        m_gameController.ReceivedMessage("New_Player,0,t", "New_Player", new string[] { "New_Player", "0", "0", "0", "1", "0", "0", "", "0", "0", "FAKER", "0.25|0.5|1"});
        m_gameController.ReceivedMessage("Start_Intro,0", "Start_Intro", new string[] { "Start_Intro", "0"});

        float introDelay = 15.0f;
        while (introDelay > 0)
        {
            introDelay -= Time.deltaTime;
            yield return null;
        }
        m_gameController.ReceivedMessage("Start_Countdown,0", "Start_Countdown", new string[] { "Start_Countdown", "0" });

        // Create characters
        m_gameController.m_playersDict = new System.Collections.Generic.Dictionary<int, PlayerControls>();

        // Start spawns

        // Set GameController
        // Set Backend
    }
}
