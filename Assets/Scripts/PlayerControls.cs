using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public bool m_isPlayer = false;
    public int m_id;
    public TMPro.TextMeshPro m_nameTextMesh;
    public GameObject m_playerSprite;
    public Animator m_animator;
    public Animator m_animatorStatusEffect;
    public string m_walkCycleName;
    public string m_idleCycleName;
    public Vector2 m_position;
    public Color m_color;
    public float m_speed;
    public string dataCurrent = "Player, ID, position_X, position_Y, localScale_X, animationImage";
    public List<float> m_prevTransformData;
    public int m_prevState;
    public bool m_isMoving = false;
    public char m_testingMovement = ' ';
    public CircleCollider2D m_circleCollider;

    public enum PLAYER_STATE {IDLE, MOVING, DAZED}
    public PLAYER_STATE m_state = PLAYER_STATE.IDLE;

    public PlayerSupplyItem m_playerSupplyItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_playerSprite == null)
        {
            Debug.LogError($"PlayerController has no assigned spriteRenderer!");
        }
        if(m_animator == null) m_animator = m_playerSprite.GetComponent<Animator>();
        m_prevTransformData = new List<float>();
        m_prevTransformData.Add(transform.localPosition.x);
        m_prevTransformData.Add(transform.localPosition.y);
        m_prevTransformData.Add(m_playerSprite.transform.localScale.x);
        m_prevState = (int)m_state;
        Debug.Log(m_state);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isPlayer)
        {
            if (m_state != PLAYER_STATE.DAZED)
            {
                m_isMoving = false;
                if (Input.GetKey(KeyCode.A) || m_testingMovement == 'A')
                {
                    MoveLeftRight(-1);
                }
                else if (Input.GetKey(KeyCode.D) || m_testingMovement == 'D')
                {
                    MoveLeftRight(1);
                }
                if (Input.GetKey(KeyCode.W) || m_testingMovement == 'W')
                {
                    MoveUpDown(1);
                }
                else if (Input.GetKey(KeyCode.S) || m_testingMovement == 'S')
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


    public void MoveLeftRight(int direction)
    {
        if(m_playerSprite != null) flipSpriteIfOppositeToDirection(direction, m_playerSprite);

        if (m_animator != null && !m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_walkCycleName))
            m_animator.Play(m_walkCycleName);

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
        if (m_animator != null && !m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_walkCycleName))
            m_animator.Play(m_walkCycleName);

        Vector3 position = transform.localPosition;
        float movementDelta = Time.deltaTime * m_speed * direction;
        position.y += movementDelta;

        transform.localPosition = position;
        m_state = PLAYER_STATE.MOVING;
        m_isMoving = true;
    }

    public void MoveStop()
    {
        m_animator.Play(m_idleCycleName);
        m_state = PLAYER_STATE.IDLE;
    }

    public string GetChangedData()
    {
        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state"
        //      0,  1,          2,          3,                           4,     5
        string changedData = $",{m_id},";
        changedData += m_prevTransformData[0] != transform.localPosition.x ? $"{transform.localPosition.x}," : ",";
        changedData += m_prevTransformData[1] != transform.localPosition.y ? $"{transform.localPosition.y}," : ",";
        changedData += m_prevTransformData[2] != m_playerSprite.transform.localScale.x ? $"{m_playerSprite.transform.localScale.x}," : ",";
        changedData += (int)m_state != m_prevState ? $"{(int)m_state}" : "";

        if(changedData == $",{m_id},,,,")
        {
            return "Unchanged";
        }
        return changedData;
    }

    public void SetChangedDataToCurrentValues()
    {
        m_prevTransformData[0] = transform.localPosition.x;
        m_prevTransformData[1] = transform.localPosition.y;
        m_prevTransformData[2] = m_playerSprite.transform.localScale.x;
        m_prevState = (int)m_state;
    }

    public string GetAllData()
    {
        string allData = $"Update,{m_id},";
        allData += $"{transform.localPosition.x},";
        allData += $"{transform.localPosition.y},";
        allData += $"{m_playerSprite.transform.localScale.x},";
        allData += $"{(int)m_state}";

        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state"
        //      0,  1,          2,          3,                           4,     5
        return allData;
    }

    public void PutChangedData(string[] changedDataList)
    {
        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state"
        //      0,  1,          2,          3,                           4,     5

        Vector3 position = transform.localPosition;
        if (changedDataList[2] != "") position.x = float.Parse(changedDataList[2]);
        if (changedDataList[3] != "") position.y = float.Parse(changedDataList[3]);
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
                        m_animator.Play(m_idleCycleName);
                        break;
                    case PLAYER_STATE.MOVING:
                        m_animator.Play(m_walkCycleName);
                        break;
                }
            }
        }
    }

    public void PutAllData(string[] allDataList)
    {
        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state, name, color";
        //      0,  1,          2,          3,                           4,     5,    6      7

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
                    m_animator.Play(m_idleCycleName);
                    break;
                case PLAYER_STATE.MOVING:
                    m_animator.Play(m_walkCycleName);
                    break;
            }
        }

        m_nameTextMesh.text = allDataList[6];
        string[] color = allDataList[7].Split("|");
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
        m_animator.speed = 0;
        while (timeDazed > 0)
        {
            timeDazed -= Time.deltaTime;
            yield return null;
        }

        m_state = PLAYER_STATE.IDLE;
        m_animatorStatusEffect.StopPlayback();
        m_animatorStatusEffect.gameObject.SetActive(false);
        m_animator.speed = 1;
    }

    public void Victory()
    {

    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
