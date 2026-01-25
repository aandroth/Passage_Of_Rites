using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RatCatchGame : Game
{
    [SerializeField] TMP_Text m_mainPlayerScoreText;
    [SerializeField] float m_gameInitDelay = 0;
    [SerializeField] float m_holdOnWinnersDelay = 0;
    [SerializeField] MinigameTitleCard m_gameTitleCard;
    [SerializeField] BlackoutPanel m_blackoutCard;
    [SerializeField] TimeDisplayed m_timeDisplayed;
    [SerializeField] ThreeTwoOneGo_Countdown m_threeTwoOneGoCountdown;


    [SerializeField] int m_mainPlayerRatCageIdx = 0;
    [SerializeField] ItemObjective m_mainPlayerRatCage;
    [SerializeField] List<GameObject> m_otherPlayersRatCages;
    [SerializeField] List<Transform> m_playerSpawnLocations;
    [SerializeField] List<Transform> m_ratCageLocations;
    [SerializeField] List<TMP_Text> m_playerNameTexts;
    [SerializeField] List<TMP_Text> m_playerScoreTexts;

    private IEnumerator m_countdownCoroutine;

    [SerializeField] float m_endGameHornHoldTime;
    [SerializeField] float m_endGameScoresHoldTime;
    [SerializeField] int m_pointsPerRat = 10;

    [SerializeField] GameObject m_announcementTextPanel;
    [SerializeField] List<GameObject> m_winnerTextPanels;
    [SerializeField] List<TMP_Text> m_winnerTexts;

    [SerializeField] Camera_FollowPlayer m_mainPlayerCameraFollow;

    private void Start()
    {
        m_mainPlayerRatCage.m_reportItemCompleted = UpdateMainPlayerPoints;
    }
    
    public override void StartGamePlaying(SignalReadinessDelegate signalGameControllerReady = null)
    {
        m_playerControls.SetPlayerAsControllable();
        m_countdownCoroutine = Countdown(signalGameControllerReady);
        StartCoroutine(m_countdownCoroutine);
    }

    public override void StartGameIntro(SignalReadinessDelegate signalGameControllerReady = null)
    {
        StartCoroutine(GameIntro(signalGameControllerReady));
    }
    public IEnumerator GameIntro(SignalReadinessDelegate signalGameControllerReady = null)
    {
        if (!m_skipIntro)
        {
            float initDelayTime = m_gameInitDelay;

            while (initDelayTime > 0)
            {
                initDelayTime -= Time.deltaTime;
                yield return null;
            }
            m_gameTitleCard.OutroAnimation();

            float timeCardDelayTime = m_gameInitDelay * 2f;

            while (timeCardDelayTime > 0)
            {
                timeCardDelayTime -= Time.deltaTime;
                yield return null;
            }
            m_blackoutCard.StartFadeOut();

            float countdownDelayTime = m_gameInitDelay * 2f;

            while (timeCardDelayTime > 0)
            {
                timeCardDelayTime -= Time.deltaTime;
                yield return null;
            }

            float threeTwoOneGoDelayTime = 10f + 1f;
            m_threeTwoOneGoCountdown?.StartCountdown(10f);
            while (threeTwoOneGoDelayTime > 0)
            {
                threeTwoOneGoDelayTime -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            m_gameTitleCard.gameObject.SetActive(false);
            m_blackoutCard.gameObject.SetActive(false);
        }
        if (signalGameControllerReady != null) signalGameControllerReady();

        if (m_isGameOwner())
        {
            foreach (var npcSpawner in m_npcSpawners)
            {
                npcSpawner.gameObject.SetActive(true);
            }
        }
    }
    public override void StartGameOutro(SignalReadinessDelegate func = null)
    {
        StartCoroutine(GameOutro(func));
    }
    public IEnumerator GameOutro(SignalReadinessDelegate func = null)
    {
        // Show winner text
        m_announcementTextPanel.SetActive(true);

        // Hold on winners
        float holdOnWinnersTime = m_holdOnWinnersDelay;
        while (holdOnWinnersTime > 0)
        {
            holdOnWinnersTime -= Time.deltaTime;
            yield return null;
        }

        // Fade to black
        float fadeToBlackTime = 3.0f;
        m_blackoutCard.StartFadeIn(3);
        while (fadeToBlackTime > 0)
        {
            fadeToBlackTime -= Time.deltaTime;
            yield return null;
        }

        // Fade to black hold
        fadeToBlackTime = 3.0f;
        while (fadeToBlackTime > 0)
        {
            fadeToBlackTime -= Time.deltaTime;
            yield return null;
        }

        func?.Invoke();
    }

    public override int SetPlayerPointsAndGetBackTopPlayer(List<PlayerInfo> playerInfoList)
    {
        int topPlayerIndex = -1;
        playerInfoList?.Sort((a, b) => b.points.CompareTo(a.points));

        m_announcementTextPanel.SetActive(true);
        foreach (var playerInfoEntry in playerInfoList)
        {
            m_winnerTextPanels[playerInfoEntry.index].SetActive(true);
            m_winnerTexts[playerInfoEntry.index].text = $"{playerInfoEntry.name}:\n\r{playerInfoEntry.points} points!";
        }

        bool isTie = false;
        if (playerInfoList.Count > 2 &&
           playerInfoList[0].points == playerInfoList[1].points &&
           playerInfoList[0].points > 0)
            isTie = true;

        m_announcementTextPanel.GetComponentInChildren<TMP_Text>().text =
            (isTie) ? "It's a Tie!" : "The Winner is\n\r";

        if (!isTie && playerInfoList[0].points > 0)
        {
            m_playerControls.m_titles += m_playerControls.m_titles.Length > 0 ? $" {m_gameTitle}" : m_gameTitle;
            m_announcementTextPanel.GetComponentInChildren<TMP_Text>().text += $"{playerInfoList[0].name}\n\rthe {m_playerControls.m_titles}!";
            if (m_playerControls.m_id == topPlayerIndex)
                m_playerControls.m_titles += m_playerControls.m_titles.Length > 0 ? $" {m_gameTitle}" : m_gameTitle;
        }
        else if (playerInfoList[0].points <= 0)
        {
            m_announcementTextPanel.GetComponentInChildren<TMP_Text>().text += $"...nobody...";
        }
        return topPlayerIndex;
    }

    public override void AssignPlayer(PlayerControls playerControls, int id, bool isMainPlayer = false)
    {
        playerControls.SetPlayerAtSpawnPoint(m_playerSpawnLocations[id]);
        if (isMainPlayer)
        {
            m_mainPlayerCameraFollow.SetTarget(playerControls.transform);
            m_playerControls = playerControls;
            m_mainPlayerRatCageIdx = id;
            m_mainPlayerRatCage.transform.position = m_ratCageLocations[id].position;
        }
        else
        {
            m_playerNameTexts[id].gameObject.SetActive(true);
            m_playerNameTexts[id].text = playerControls.m_nameTextMesh.text;
            m_playerScoreTexts[id].gameObject.SetActive(true);
            m_otherPlayersRatCages[id].SetActive(true);
        }
    }

    private IEnumerator Countdown(SignalReadinessDelegate signalGameControllerReady = null)
    {
        Debug.Log("Rat-Catch Countdown started");
        float timeForGame = m_gameCountdownTime;
        int prevTime = Mathf.FloorToInt(m_gameCountdownTime);
        while (timeForGame > 0)
        {
            timeForGame -= Time.deltaTime;
            if (prevTime != Mathf.FloorToInt(timeForGame))
            {
                prevTime = Mathf.FloorToInt(timeForGame);
                prevTime = Mathf.Max(0, prevTime);
                m_timeDisplayed.SetTime(prevTime);
            }
            yield return null;
        }
        StartCoroutine(EndGame(signalGameControllerReady));
    }

    public override IEnumerator EndGame(SignalReadinessDelegate signalGameControllerReady = null)
    {
        StopCoroutine(m_countdownCoroutine);
        m_timeDisplayed.SetTime(0);
        m_playerControls.SetPlayerAsNotControllable();
        m_gameState = GAME_STATE.GAME_OVER;

        // Blow endgame horn sound here

        // Hold on endgame sound for a second
        float endGameHornHoldTime = m_endGameHornHoldTime;
        while (endGameHornHoldTime > 0)
        {
            endGameHornHoldTime -= Time.deltaTime;
            yield return null;
        }
        signalGameControllerReady?.Invoke(); // Signals GameController that the game is over

        float endGameScoresHoldTime = m_endGameScoresHoldTime;
        while (endGameScoresHoldTime > 0)
        {
            endGameScoresHoldTime -= Time.deltaTime;
            yield return null;
        }

        signalGameControllerReady?.Invoke(); // Signals GameController that the scene has finished, and the next can be loaded
    }


    public void RatPutIntoCage()
    {
        UpdateMainPlayerPoints();
    }

    public void UpdateMainPlayerPoints()
    {
        m_playerControls.m_points += m_pointsPerRat;
        m_mainPlayerScoreText.text = $"{m_playerControls.m_points} Points";
    }


    public void AssignRatsToPlayerAndStation()
    {
        //m_playerControls.AssignTrapToPlayerSupplyItem(t);
        //m_mainPlayerRatCage.AssignTrapToComplete(t);
    }

    public override void UpdateOtherPlayerPoints(int playerIndex, int points)
    {
        m_playerScoreTexts[playerIndex].text = $"{points} Points";
    }
}
