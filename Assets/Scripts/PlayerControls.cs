using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemObjective;

public class PlayerControls : MonoBehaviour
{
    public bool m_isMainPlayer = false;
    public int m_id;
    public TMPro.TextMeshPro m_nameTextMesh;
    public string m_titles;
    public int m_points;
    public int m_totalPoints;
    public GameObject m_playerSprite;
    public Animator m_animatorBody;
    public Animator m_animatorStatusEffect;
    public string m_walkCycleName;
    public string m_walkCycleEyesName;
    public string m_idleCycleName;
    public string m_idleCycleEyesName;
    public Vector2 m_position;
    public Color m_color;
    public float m_speed;
    public string dataCurrent = "Player, ID, position_X, position_Y, localScale_X, animationImage";
    public List<float> m_prevTransformData;
    public int m_prevState;
    public string m_prevTitles;
    public int m_prevPoints;
    public int m_prevCarriedItem;
    public bool m_isMoving = false;
    public char m_nonPlayerMovement = ' ';
    public CircleCollider2D m_circleCollider;
    [SerializeField]
    float m_positionDeltaThreshold = 10f;

    public enum PLAYER_STATE {IDLE, MOVING, DAZED}
    public PLAYER_STATE m_state = PLAYER_STATE.IDLE;
    public bool m_playerIsControllable = false;


    [SerializeField] PlayerSupplyItem m_playerSupplyItem;
    [SerializeField] OtherPlayerSupplyItem m_otherPlayerSupplyItem;
    [SerializeField] IAccessibleSupplyItem m_accessibleSupplyItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_playerSprite == null)
        {
            Debug.LogError($"PlayerController has no assigned spriteRenderer!");
        }
        if(m_animatorBody == null) m_animatorBody = m_playerSprite.GetComponent<Animator>();
        m_prevTransformData = new List<float>();
        m_prevTransformData.Add(transform.localPosition.x);
        m_prevTransformData.Add(transform.localPosition.y);
        m_prevTransformData.Add(m_playerSprite.transform.localScale.x);
        m_prevState = (int)m_state;
        m_prevTitles = m_titles;
        m_prevPoints = m_points;
        Debug.Log(m_state);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isMainPlayer)
        {
            if (m_state != PLAYER_STATE.DAZED)
            {
                m_isMoving = false;
                if ((Input.GetKey(KeyCode.A) && m_playerIsControllable) || m_nonPlayerMovement == 'A')
                {
                    MoveLeftRight(-1);
                }
                else if ((Input.GetKey(KeyCode.D) && m_playerIsControllable) || m_nonPlayerMovement == 'D')
                {
                    MoveLeftRight(1);
                }
                if ((Input.GetKey(KeyCode.W) && m_playerIsControllable) || m_nonPlayerMovement == 'W')
                {
                    MoveUpDown(1);
                }
                else if ((Input.GetKey(KeyCode.S) && m_playerIsControllable) || m_nonPlayerMovement == 'S')
                {
                    MoveUpDown(-1);
                }

                if (!m_isMoving && m_state == PLAYER_STATE.MOVING)
                {
                    MoveStop();
                }
            }
        }
    }

    public void SetPlayerAsControllable()
    {
        m_playerIsControllable = true;
        m_playerSupplyItem.SetMouseIsControllable(true);
        Debug.Log("Player is now controllable");
    }

    public void SetPlayerAsNotControllable()
    {
        m_playerIsControllable = false;
        m_playerSupplyItem.SetMouseIsControllable(false);
    }

    public void SetPlayerAtSpawnPoint(Transform spawnPoint)
    {
        transform.position = spawnPoint.transform.position;
        m_playerSprite.transform.localScale = spawnPoint.transform.localScale;
    }

    public void SetPlayerAsMainOrOther(bool isMainPlayer)
    {
        if (isMainPlayer)
        {
            m_accessibleSupplyItem = m_playerSupplyItem;
            m_otherPlayerSupplyItem.gameObject.SetActive(false);
        }
        else
        {
            m_accessibleSupplyItem = m_otherPlayerSupplyItem;
            m_playerSupplyItem.gameObject.SetActive(false);
        }
    }

    public void MoveLeftRight(int direction)
    {
        if(m_playerSprite != null) flipSpriteIfOppositeToDirection(direction, m_playerSprite);

        PlayWalkCycle();

        Vector3 position = transform.localPosition;
        float movementDelta = Time.deltaTime * m_speed * direction;
        position.x += movementDelta;

        transform.localPosition = position;
        m_state = PLAYER_STATE.MOVING;
        m_isMoving = true;
    }

    private void flipSpriteIfOppositeToDirection(int direction, GameObject playerSprite)
    {
        if (direction < 0 && m_playerSprite.transform.localScale.x > 0 ||
            direction > 0 && m_playerSprite.transform.localScale.x < 0)
        {
            Vector3 localScale = m_playerSprite.transform.localScale;
            localScale.x = -m_playerSprite.transform.localScale.x;
            m_playerSprite.transform.localScale = localScale;
        }
    }


    public void MoveUpDown(int direction)
    {
        PlayWalkCycle();

        Vector3 position = transform.localPosition;
        float movementDelta = Time.deltaTime * m_speed * direction;
        position.y += movementDelta;

        transform.localPosition = position;
        m_state = PLAYER_STATE.MOVING;
        m_isMoving = true;
    }

    public void MoveStop()
    {
        PlayIdleCycle();
        m_state = PLAYER_STATE.IDLE;
    }

    public string GetChangedData()
    {
        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state, carriedItem, titles, points";
        //      0,  1,          2,          3,                           4,     5,           6,      7,      8
        /*0,1,*/string changedData = $",{m_id},";
        /*2,  */changedData += m_prevTransformData[0] != transform.localPosition.x ? $"{transform.localPosition.x}," : ",";
        /*3,  */changedData += m_prevTransformData[1] != transform.localPosition.y ? $"{transform.localPosition.y}," : ",";
        /*4,  */changedData += m_prevTransformData[2] != m_playerSprite.transform.localScale.x ? $"{m_playerSprite.transform.localScale.x}," : ",";
        /*5,  */changedData += (int)m_state != m_prevState ? $"{(int)m_state}," : ",";
        /*6,  */changedData += (int)m_playerSupplyItem.GetSupplyItemName() != m_prevCarriedItem ? $"{(int)m_playerSupplyItem.GetSupplyItemName()}," : ",";
        /*7,  */changedData += m_titles != m_prevTitles ? $"{m_titles}," : ",";
        /*8,  */changedData += m_points != m_prevPoints ? $"{m_points}" : "";

        if(changedData == $",{m_id},,,,,,,")
        {
            return "Unchanged";
        }
        return changedData;
    }

    public void SetChangedDataToCurrentValues()
    {
        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state, carriedItem, titles, points";
        //      0,  1,          2,          3,                           4,     5,           6,      7,      8
        /*2*/m_prevTransformData[0] = transform.localPosition.x;
        /*3*/m_prevTransformData[1] = transform.localPosition.y;
        /*4*/m_prevTransformData[2] = m_playerSprite.transform.localScale.x;
        /*5*/m_prevState = (int)m_state;
        /*6*/m_prevCarriedItem = (int)m_accessibleSupplyItem.GetSupplyItemName();
        /*7*/m_prevTitles = m_titles;
        /*8*/m_prevPoints = m_points;
    }

    public string GetChangableData()
    {
        string changableData = $"Update,{m_id},";
        changableData += $"{transform.localPosition.x},";
        changableData += $"{transform.localPosition.y},";
        changableData += $"{m_playerSprite.transform.localScale.x},";
        changableData += $"{(int)m_state},";
        changableData += $"{(int)m_accessibleSupplyItem.GetSupplyItemName()},";
        changableData += $"{m_titles},";
        changableData += $"{m_points}";

        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state, carriedItem, titles, points";
        //      0,  1,          2,          3,                           4,     5,           6,      7,      8
        return changableData;
    }

    public void PutChangedData(string[] changedDataList)
    {
        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state, carriedItem, titles, points";
        //      0,  1,          2,          3,                           4,     5,           6,      7,      8

        Vector3 position = transform.localPosition;
        if (changedDataList[2] != "") position.x = float.Parse(changedDataList[2]);
        if (changedDataList[3] != "") position.y = float.Parse(changedDataList[3]);
        float positionDelta = Vector2.Distance(position, transform.localPosition);
        if (m_isMainPlayer && positionDelta >= m_positionDeltaThreshold)
            transform.localPosition = position;

        Vector3 scale = m_playerSprite.transform.localScale;
        if (changedDataList[4] != "") scale.x = float.Parse(changedDataList[4]);
        m_playerSprite.transform.localScale = scale;

        if (changedDataList[5] != "")
        {
            int state = int.Parse(changedDataList[5]);
            if ((int)m_state != state)
            {
                m_state = (PLAYER_STATE)state;
                switch (m_state)
                {
                    case PLAYER_STATE.IDLE:
                        PlayIdleCycle();
                        break;
                    case PLAYER_STATE.MOVING:
                        PlayWalkCycle();
                        break;
                    case PLAYER_STATE.DAZED:
                        Dazed();
                        break;
                }
            }
        }

        if (changedDataList[6] != "")
        {
            int carriedItem = int.Parse(changedDataList[6]);
            if ((int)m_accessibleSupplyItem.GetSupplyItemName() != carriedItem)
            {
                m_accessibleSupplyItem.SetSupplyItem((SupplyItemName)carriedItem);
            }
        }
        if (changedDataList[7] != "") m_titles = changedDataList[7];
        if (changedDataList[8] != "") m_points = int.Parse(changedDataList[8]);
    }

    public void PutAllData(string[] allDataList)
    {
        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state, carriedItem, titles, points, totalPoints, name, color";
        //      0,  1,          2,          3,                           4,     5,           6,      7,      8,           9,   10,    11

        Vector3 position = transform.localPosition;
        position.x = float.Parse(allDataList[2]);
        position.y = float.Parse(allDataList[3]);
        transform.localPosition = position;

        Vector3 scale = m_playerSprite.transform.localScale;
        scale.x = float.Parse(allDataList[4]);
        m_playerSprite.transform.localScale = scale;

        int state = int.Parse(allDataList[5]);
        if ((int)m_state != state)
        {
            m_state = (PLAYER_STATE)state;
            switch (m_state)
            {
                case PLAYER_STATE.IDLE:
                    PlayIdleCycle();
                    break;
                case PLAYER_STATE.MOVING:
                    PlayWalkCycle();
                    break;
                case PLAYER_STATE.DAZED:
                    Dazed();
                    break;
            }
        }

        int carriedItem = int.Parse(allDataList[6]);
        if ((int)m_accessibleSupplyItem.GetSupplyItemName() != carriedItem)
        {
            m_accessibleSupplyItem.SetSupplyItem((SupplyItemName)carriedItem);
        }

        m_titles = allDataList[7];

        m_points = int.Parse(allDataList[8]);
        m_totalPoints = int.Parse(allDataList[9]);
        m_nameTextMesh.text = allDataList[10];
        string[] color = allDataList[11].Split("|");
        m_color = new Color(float.Parse(color[0]),
                            float.Parse(color[1]),
                            float.Parse(color[2]));
        m_playerSprite.GetComponent<SpriteRenderer>().color = m_color;
    }

    public void Dazed(float timeDazed = 1f)
    {
        StartCoroutine(DazedCoroutine(timeDazed));
    }

    public IEnumerator DazedCoroutine(float timeDazed = 1f)
    {
        m_animatorStatusEffect.gameObject.SetActive(true);
        m_animatorStatusEffect.Play("Dazed");
        m_animatorBody.speed = 0;
        while (timeDazed > 0)
        {
            timeDazed -= Time.deltaTime;
            yield return null;
        }

        m_state = PLAYER_STATE.IDLE;
        m_animatorStatusEffect.StopPlayback();
        m_animatorStatusEffect.gameObject.SetActive(false);
        m_animatorBody.speed = 1;
    }

    public void AssignNeededSuppliesToPlayerSupplyItem(List<SupplyItemName> neededSupplyItems)
    {
        m_playerSupplyItem.AssignNeededSupplyItems(neededSupplyItems);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }


    private void PlayWalkCycle()
    {
        if (m_animatorBody != null && !m_animatorBody.GetCurrentAnimatorStateInfo(0).IsName(m_walkCycleName))
            m_animatorBody.Play(m_walkCycleName);
    }

    private void PlayIdleCycle()
    {
        if (m_animatorBody != null && !m_animatorBody.GetCurrentAnimatorStateInfo(0).IsName(m_walkCycleName))
            m_animatorBody.Play(m_idleCycleName);
    }
}
