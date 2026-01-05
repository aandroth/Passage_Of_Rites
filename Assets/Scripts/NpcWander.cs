using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ItemObjective;
using static PlayerControls;

public class NpcWander : MonoBehaviour
{
    [SerializeField] float m_runTimeMin = 2f, m_runTimeMax = 4f;
    [SerializeField] float m_waitTimeMin = 1f, m_waitTimeMax = 3f;
    [SerializeField] int m_facingDirection = 1;
    [SerializeField] float m_speed = 2f;
    [SerializeField] Vector2 m_wanderDirection;
    [SerializeField] string m_runAnimationName;
    [SerializeField] string m_idleAnimationName;
    [SerializeField] Animator m_animator;
    public enum NPC_STATE { IDLE, MOVING, DESTROYED }
    [SerializeField] NPC_STATE m_state = NPC_STATE.IDLE;
    private IEnumerator m_wanderCoroutine;

    [SerializeField] NetworkDataObject_Npc m_networkDataObjectNpc = new NetworkDataObject_Npc();

    public void Start()
    {
        

        m_wanderCoroutine = WanderCoroutine();
        if(GameController.Instance.m_isGameOwner)
            StartWandering();
        m_networkDataObjectNpc.SetChangedDataToCurrentValues();
    }

    public void OnEnable()
    {
        if (m_runTimeMin > m_runTimeMax)
            Debug.LogError("NpcWander: Run Min time greater than max time!");
        if (m_waitTimeMin > m_waitTimeMax)
            Debug.LogError("NpcWander: Wait Min time greater than max time!");
        if(m_wanderCoroutine != null && GameController.Instance.m_isGameOwner) 
            StartWandering();
    }

    public void StartWandering()
    {
        StartCoroutine(m_wanderCoroutine);
    }

    public void StopWandering()
    {
        StopCoroutine(m_wanderCoroutine);
        PlayIdleCycle();
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
            PlayMovingCycle();
            while (runTime > 0f)
            {
                transform.Translate(m_wanderDirection * Time.deltaTime * m_speed);
                runTime -= Time.deltaTime;
                yield return null;
            }
            PlayIdleCycle();
            yield return new WaitForSeconds(Random.Range(m_waitTimeMin, m_waitTimeMax)); // Adjust spawn interval as needed
        }
    }

    private void PlayIdleCycle()
    {
        m_state = NPC_STATE.IDLE;
        m_animator.Play(m_idleAnimationName);
    }

    private void PlayMovingCycle()
    {
        m_state = NPC_STATE.MOVING;
        m_animator.Play(m_runAnimationName);
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

    public void DestroyedWasCalled()
    {

    }

    public void UpdateTransformValues(List<float?> possibleValues)
    {
        if (possibleValues[0] != null || possibleValues[1] != null)
        {
            Vector3 newPosition = transform.localPosition;
            if (possibleValues[0] != null) newPosition.x = (float)possibleValues[0];
            if (possibleValues[1] != null) newPosition.y = (float)possibleValues[1];
            transform.localPosition = newPosition;
        }

        if (possibleValues[2] != null)
        {
            Vector3 newScale = transform.localScale;
            newScale.x = (float)possibleValues[2];
            transform.localScale = newScale;
        }
    }

    public void UpdateState(int state)
    {
        m_state = (NPC_STATE)state;
    }

    public List<float> GetCurrentValues()
    {
        return new List<float>() {
            transform.position.x,
            transform.position.y,
            transform.localScale.x,
            (float)m_state
        };
    }
}
