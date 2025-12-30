using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class StartOutro : MonoBehaviour
{
    public float m_timeTillOutro = 5.0f;
    public Game m_game;

    public string[] m_names;
    public int[] m_points;
    public int m_nextLevel;

    public bool startOutro = false;

    [SerializeField] List<Game.PlayerInfo> m_playerInfos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startOutro)
        {
            while (m_timeTillOutro > 0)
            {
                m_timeTillOutro -= Time.deltaTime;
                if (m_timeTillOutro <= 0)
                {
                    m_playerInfos = new List<Game.PlayerInfo>();
                    for (int i = 0; i < m_names.Length; i++)
                    {
                        Game.PlayerInfo playerInfo = new Game.PlayerInfo
                        {
                            index = i,
                            name = m_names[i],
                            points = m_points[i]
                        };
                        m_playerInfos.Add(playerInfo);
                    }

                    m_game.SetPlayerPoints(m_playerInfos);
                    m_game.StartGameOutro();
                }
            }
        }
    }
}
