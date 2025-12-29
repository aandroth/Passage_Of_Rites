using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndingSceneController : Game
{
    [SerializeField] Backend m_backend = null;
    [SerializeField] TMP_Text m_weGatherToHonorText;
    [SerializeField] float m_showHonorTime;
    [SerializeField] float m_showPlayerInfoTime;
    [SerializeField] float m_showBlankTime = 2f;
    [SerializeField] float m_fadeBlackoutOutTime = 2f;
    [SerializeField] float m_fadeBlackoutInTime = 2f;
    [SerializeField] float m_showThanksTime;
    [SerializeField] List<PlayerInfo> m_playerInfoList;
    [SerializeField] int m_titleSceneLevel = 0;
    [SerializeField] GameObject[] m_spawnLocations = null;
    [SerializeField] BlackoutPanel m_blackoutPanel = null;

    private struct PlayerInfo
    {
        public string name;
        public string titles;
        public int points;
    }

    public void Awake()
    {
        if(m_backend == null)
            m_backend = Backend.Instance;
        m_playerInfoList = new List<PlayerInfo>();

        StartBlackoutFadeOut();
    }

    private void StartBlackoutFadeOut()
    {
        StartCoroutine(StartBlackoutFadeOutCoroutine());
    }

    private IEnumerator StartBlackoutFadeOutCoroutine()
    {
        float showHonorTime = m_fadeBlackoutOutTime;

        while (showHonorTime > 0)
        {
            showHonorTime -= Time.deltaTime;
            yield return null;
        }
    }

    public override void StartGameIntro(SignalReadinessDelegate signalGameControllerReady = null)
    {
        Debug.Log($"Starting Ending Scene Outro");

        StartCoroutine(StartGameOutroCoroutine(signalGameControllerReady));
    }
    public IEnumerator StartGameOutroCoroutine(SignalReadinessDelegate signalGameControllerReady = null)
    {
        float blackoutFadeOutTime = m_fadeBlackoutOutTime;
        m_blackoutPanel.StartFadeOut(blackoutFadeOutTime);

        while (blackoutFadeOutTime > 0){
            blackoutFadeOutTime -= Time.deltaTime;
            yield return null;
        }


        float showHonorTime = m_showHonorTime;

        while(showHonorTime > 0){
            showHonorTime -= Time.deltaTime;
            yield return null;
        }

        foreach (var playerInfo in m_playerInfoList)
        {
            string fullTitle = string.IsNullOrEmpty(playerInfo.titles) ? "" : $"\n\rthe {playerInfo.titles}";
            m_weGatherToHonorText.text = $"{playerInfo.name}{fullTitle}\n\rwith {playerInfo.points} points!";
            float showPlayerInfoTime = m_showPlayerInfoTime;
            while (showPlayerInfoTime > 0)
            {
                showPlayerInfoTime -= Time.deltaTime;
                yield return null;
            }
            m_weGatherToHonorText.text = $"";
            while (showPlayerInfoTime > 0)
            {
                showPlayerInfoTime -= Time.deltaTime;
                yield return null;
            }
        }

        m_weGatherToHonorText.text = $"Congratulations to the players!";
        float showThanksTime = m_showThanksTime;
        while (showThanksTime > 0)
        {
            showThanksTime -= Time.deltaTime;
            yield return null;
        }

        m_weGatherToHonorText.text = $"And thanks. Thanks for playing my game.";
        showThanksTime = m_showThanksTime;
        while (showThanksTime > 0)
        {
            showThanksTime -= Time.deltaTime;
            yield return null;
        }

        float blackoutFadeInTime = m_fadeBlackoutInTime;
        m_blackoutPanel.StartFadeIn(blackoutFadeInTime);

        while (blackoutFadeOutTime > 0)
        {
            blackoutFadeOutTime -= Time.deltaTime;
            yield return null;
        }
        // Send players back to Title screen
        if(signalGameControllerReady != null)
            signalGameControllerReady(true);

        yield return null;
    }

    public override void AssignPlayer(PlayerControls playerControls, int id, bool isMainPlayer = false) 
    {
        m_playerInfoList.Add(new PlayerInfo
        {
            name = playerControls.m_nameTextMesh.text,
            titles = playerControls.m_titles,
            points = playerControls.m_points
        });
        playerControls.transform.position = m_spawnLocations[id].transform.position;
        playerControls.m_playerSprite.transform.localScale = m_spawnLocations[id].transform.localScale;
    }
}
