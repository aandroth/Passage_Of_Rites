using System.Collections;
using UnityEngine;

public class NpcWander : MonoBehaviour
{
    [SerializeField] float m_runTimeMin = 2f, m_runTimeMax = 4f;
    [SerializeField] float m_waitTimeMin = 1f, m_waitTimeMax = 3f;
    [SerializeField] int m_facingDirection = 1;
    [SerializeField] float m_speed = 2f;
    [SerializeField] Vector2 m_wanderDirection;
    private IEnumerator m_wanderCoroutine;

    public void Start()
    {
        m_wanderCoroutine = WanderCoroutine();
        StartWandering();
    }

    public void OnEnable()
    {
        if (m_runTimeMin > m_runTimeMax)
            Debug.LogError("NpcWander: Run Min time greater than max time!");
        if (m_waitTimeMin > m_waitTimeMax)
            Debug.LogError("NpcWander: Wait Min time greater than max time!");
        if(m_wanderCoroutine != null) StartWandering();
    }

    public void StartWandering()
    {
        StartCoroutine(m_wanderCoroutine);
    }

    public void StopWandering()
    {
        StopCoroutine(m_wanderCoroutine);
    }

    private IEnumerator WanderCoroutine()
    {
        while (true)
        {
            // pick randirection
            m_wanderDirection = Random.insideUnitCircle.normalized;
            CorrectFacing();

            // run in that direction for some time
            float runTime = Random.Range(m_runTimeMin, m_runTimeMax);
            while (runTime > 0f)
            {
                transform.Translate(m_wanderDirection * Time.deltaTime * m_speed);
                runTime -= Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(m_waitTimeMin, m_waitTimeMax)); // Adjust spawn interval as needed
        }
    }

    public void CorrectFacing()
    {
        if ((m_wanderDirection.x > 0 && m_facingDirection < 0) ||
           (m_wanderDirection.x < 0 && m_facingDirection > 0))
        {
            // flip facing direction
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
            m_facingDirection *= -1;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("NpcWander: Collision detected, changing direction.");
        m_wanderDirection *= -1;
        CorrectFacing();
    }
}
