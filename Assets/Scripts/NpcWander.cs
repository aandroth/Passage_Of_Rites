using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ItemObjective;
using static PlayerControls;

public class NpcWander : MonoBehaviour
{

    [SerializeField] public int m_id { get; private set; }
    [SerializeField] public int m_spawnerId { get; private set; }
    [SerializeField] public bool m_hasItem { get; private set; }
    [SerializeField] public NpcTypeData.NpcTypes m_npcType { get; private set; }
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

    [SerializeField] public NetworkDataObject_Npc m_networkDataObjectNpc { get; private set; } = new NetworkDataObject_Npc();


    public delegate void OnDestroyDelegate(int id);
    public OnDestroyDelegate m_onDestroyDelegate;

    public void FillNetworkDataObjectDelegates()
    {
        m_networkDataObjectNpc.m_getId = () => { return m_id; };
        m_networkDataObjectNpc.m_getSpawnerId = () => { return m_spawnerId; };
        m_networkDataObjectNpc.m_setIdSpawnerIdAndNpcType = SetIdSpawnerIdAndNpcType;
        m_networkDataObjectNpc.m_getCurrentValues = GetCurrentValues;
        m_networkDataObjectNpc.m_getAllCurrentValues = GetAllCurrentValues;
        m_networkDataObjectNpc.m_updateTransform = UpdateTransformValues;
        m_networkDataObjectNpc.m_updateState = UpdateState;
        m_networkDataObjectNpc.m_playerBecameGameOwner = StartWandering;

        m_networkDataObjectNpc.m_prevData.m_prevTransformData = new List<float>() {0f,0f,0f};
        m_networkDataObjectNpc.m_prevData.m_state = 0;

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
            ChangeToRandomDirection();
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

    private void ChangeToRandomDirection()
    {
        m_wanderDirection = Random.insideUnitCircle.normalized;
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
        ChangeToRandomDirection();
        CorrectFacing();
    }

    public void SetIdSpawnerIdAndNpcType(int id, int spawnerId, int type)
    {
        m_id = id;
        m_spawnerId = spawnerId;
        m_npcType = (NpcTypeData.NpcTypes)(type);
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
        NPC_STATE newState = (NPC_STATE)state;
        if (newState == NPC_STATE.DESTROYED)
            Destroy(gameObject);
        else if(newState == NPC_STATE.MOVING && m_state != NPC_STATE.MOVING)
            PlayMovingCycle();
        else if(newState == NPC_STATE.IDLE && m_state != NPC_STATE.IDLE)
            PlayIdleCycle();
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

    public List<float> GetAllCurrentValues()
    {
        return new List<float>() {
            m_id,
            m_spawnerId,
            (int)m_npcType,
            transform.position.x,
            transform.position.y,
            transform.localScale.x,
            (float)m_state
        };
    }
    public void OnDestroy()
    {
        m_onDestroyDelegate?.Invoke(m_id);
    }
}
