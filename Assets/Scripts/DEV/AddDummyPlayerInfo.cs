using UnityEngine;

public class AddDummyPlayerInfo : MonoBehaviour
{
    [SerializeField] EndingSceneController m_endingSceneController = null;
    [SerializeField] GameObject m_playerPrefab = null;
    private void Awake()
    {
        if (m_endingSceneController == null)
        {
            m_endingSceneController = GetComponent<EndingSceneController>();
        }
        GameObject p1 = GameObject.Instantiate(m_playerPrefab, Vector3.zero, Quaternion.identity);
        p1.GetComponent<PlayerControls>().m_id = 1;
        p1.GetComponent<PlayerControls>().m_isMainPlayer = false;
        p1.GetComponent<PlayerControls>().m_nameTextMesh.text = "Bobberino";
        p1.GetComponent<PlayerControls>().m_titles = "Wise Mage";
        p1.GetComponent<PlayerControls>().m_points = 200;
        m_endingSceneController.AssignPlayer(p1.GetComponent<PlayerControls>(), 1);

        GameObject p2 = GameObject.Instantiate(m_playerPrefab, Vector3.zero, Quaternion.identity);
        p2.GetComponent<PlayerControls>().m_id = 2;
        p2.GetComponent<PlayerControls>().m_isMainPlayer = false;
        p2.GetComponent<PlayerControls>().m_nameTextMesh.text = "Charlie";
        p2.GetComponent<PlayerControls>().m_titles = "Stealthy Rogue";
        p2.GetComponent<PlayerControls>().m_points = 180;
        m_endingSceneController.AssignPlayer(p2.GetComponent<PlayerControls>(), 2);

        GameObject p3 = GameObject.Instantiate(m_playerPrefab, Vector3.zero, Quaternion.identity);
        p3.GetComponent<PlayerControls>().m_id = 3;
        p3.GetComponent<PlayerControls>().m_isMainPlayer = false;
        p3.GetComponent<PlayerControls>().m_nameTextMesh.text = "Alice";
        p3.GetComponent<PlayerControls>().m_titles = "Brave Warrior";
        p3.GetComponent<PlayerControls>().m_points = 153;
        m_endingSceneController.AssignPlayer(p3.GetComponent<PlayerControls>(), 3);


        m_endingSceneController.StartGameIntro();
    }
}
