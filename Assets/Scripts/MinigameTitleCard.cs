using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MinigameTitleCard : MonoBehaviour
{
    [SerializeField] private float m_moveLeftTime = 1f;
    [SerializeField] private float m_moveLeftSpeed = 1f;
    [SerializeField] private float m_moveLeftTarget = -100f;
    [SerializeField] private float m_moveRightTime = 2f;
    [SerializeField] private float m_moveRightSpeed = 5f;
    [SerializeField] private float m_moveRightTarget = 1000f;

    public void OutroAnimation()
    {
        StartCoroutine(RemoveMyselfCoroutine());
    }

    private IEnumerator RemoveMyselfCoroutine()
    {
        float moveLeftTimeChanging = 0f;
        Vector3 posVec = gameObject.transform.position;
        float posX = gameObject.transform.position.x;

        // Move a bit left
        while (moveLeftTimeChanging < m_moveLeftTime)
        {
            moveLeftTimeChanging += Time.deltaTime;
            posVec.x = Mathf.Lerp(posX, m_moveLeftTarget, moveLeftTimeChanging/m_moveLeftTime);
            transform.position = posVec;
            yield return null;
        }

        // Move right till off-screen
        float moveRightTimeChanging = 0f;
        posVec = gameObject.transform.position;
        posX = gameObject.transform.position.x;

        while (moveRightTimeChanging < m_moveRightTime)
        {
            moveRightTimeChanging += Time.deltaTime;
            posVec.x = Mathf.Lerp(posX, m_moveRightTarget, moveRightTimeChanging/m_moveRightTime);
            transform.position = posVec;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
