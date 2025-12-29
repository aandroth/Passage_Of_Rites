using UnityEngine;

public class StartOutro : MonoBehaviour
{
    public float m_timeTillOutro = 5.0f;
    public Game m_game;

    public string[] names;
    public int[] points;
    public int nextLevel;

    public bool startOutro = false;

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
                    m_game.SetPlayerPoints(names, points);
                    m_game.StartGameOutro();
                }
            }
        }
    }
}
