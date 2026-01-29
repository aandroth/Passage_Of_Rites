using System.Collections.Generic;
using UnityEngine;
using static ItemObjectiveData;

public class NetworkDataObject_Item
{
    public struct PreviousData_Item
    {
        public List<float> m_prevTransformData;
        public int m_state;
    }

    public PreviousData_Item m_prevData = new PreviousData_Item();

    public delegate int GetIdDelegate();
    public GetIdDelegate m_getId;
    public delegate int GetOwnerIdDelegate();
    public GetOwnerIdDelegate m_getOwnerId;
    public delegate void SetIdSpawnerIdOwnerTypeAndItemTypeDelegate(int i, int s, int o, int t);
    public SetIdSpawnerIdOwnerTypeAndItemTypeDelegate m_setIdSpawnerIdOwnerTypeAndNpcType;
    public delegate void UpdateTransformDelegate(List<float?> t);
    public UpdateTransformDelegate m_updateTransform;
    public delegate void UpdateStateDelegate(int s);
    public UpdateStateDelegate m_updateState;
    public delegate List<float> GetCurrentValuesDelegate();
    public GetCurrentValuesDelegate m_getCurrentValues;
    public delegate List<float> GetAllCurrentValuesDelegate();
    public GetAllCurrentValuesDelegate m_getAllCurrentValues;
    public delegate ITEM_STATE GetCurrentStateDelegate();
    public GetCurrentStateDelegate m_getCurrentState;
    public delegate void PlayerBecameGameOwnerDelegate();
    public PlayerBecameGameOwnerDelegate m_playerBecameGameOwner;

    public float positionChangeThreshhold = 10f;
    public void Start()
    {
        m_prevData.m_prevTransformData = new List<float>();
        m_prevData.m_state = 0;
    }

    public void PlayerBecameGameOwner()
    {
        m_playerBecameGameOwner();
    }

    public void SetChangedDataToCurrentValues()
    {
        //"position_X, position_Y, transform.localScale_X, state";
        //          0,          1,                      2,     3

        List<float> currValues = m_getCurrentValues();
        /*2*/ m_prevData.m_prevTransformData[0] = currValues[0];
        /*3*/ m_prevData.m_prevTransformData[1] = currValues[1];
        /*4*/ m_prevData.m_prevTransformData[2] = currValues[2];
        /*5*/ m_prevData.m_state = (int)currValues[3];
    }
    public string GetChangedData()
    {
        //"position_X, position_Y, transform.localScale_X, state";
        //          0,          1,                      2,     3
        List<float> currValues = m_getCurrentValues();
        int id = m_getId();

        //"Action, id, ownerId, ItemType, OwnerType, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,       2,        3,         4,          5,          6,                      7,     8
        /*0,1,2,3,*/ string changedData = $",{id},,,,";
        /*4,  */ changedData += m_prevData.m_prevTransformData[0] != currValues[0] ? $"{currValues[0]}," : ",";
        /*5,  */ changedData += m_prevData.m_prevTransformData[1] != currValues[1] ? $"{currValues[1]}," : ",";
        /*6,  */ changedData += m_prevData.m_prevTransformData[2] != currValues[2] ? $"{currValues[2]}," : ",";
        /*7,  */ changedData += m_prevData.m_state != (int)currValues[3] ? $"{currValues[3]}" : "";
        if (changedData == $",{id},,,,,,,")
        {
            return "Unchanged";
        }
        return changedData;
    }
    public string GetAllData()
    {
        // currValues
        //"id, ownerId, ItemType, OwnerType, position_X, position_Y, transform.localScale_X, state";
        //  0,       1,        2,         3,          4,          5,                      6,     7
        Debug.Log("Using m_getCurrentValues");
        List<float> currValues = m_getAllCurrentValues();

        /*0,1,2,3,4*/ string allData = $",{currValues[0]},{currValues[1]},{currValues[2]},{currValues[3]},";
        /*5,  */ allData += $"{currValues[4]},";
        /*6,  */ allData += $"{currValues[5]},";
        /*7,  */ allData += $"{currValues[6]},";
        /*8,  */ allData += $"{currValues[7]}";
        return allData;
    }

    public void PutChangedData(string[] changedDataList)
    {
        // changedDataList
        //"Action, id, ownerId, ItemType, OwnerType, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,       2,        3,         4,          5,          6,                      7,     8

        // possibleTransformChanges
        //"position_X, position_Y, transform.localScale_X";
        //          0,          1,                      2

        if ((changedDataList[4] != "") || (changedDataList[5] != "") || (changedDataList[6] != ""))
        {
            List<float?> possibleTransformChanges = new List<float?>() { null, null, null };
            if (changedDataList[4] != "") possibleTransformChanges[0] = float.Parse(changedDataList[4]);
            if (changedDataList[5] != "") possibleTransformChanges[1] = float.Parse(changedDataList[5]);
            if (changedDataList[6] != "") possibleTransformChanges[2] = float.Parse(changedDataList[6]);
            m_updateTransform(possibleTransformChanges);
        }

        if (changedDataList[7] != "") m_updateState(int.Parse(changedDataList[7]));
    }

    public void PutAllData(string[] fullDataList)
    {
        // fullDataList
        //"Action, id, ownerId, ItemType, OwnerType, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,       2,        3,         4,          5,          6,                      7,     8

        m_setIdSpawnerIdOwnerTypeAndNpcType(int.Parse(fullDataList[1]), int.Parse(fullDataList[2]), int.Parse(fullDataList[3]), int.Parse(fullDataList[4]));

        List<float?> transformValues = new List<float?>() { null, null, null };
        transformValues[0] = float.Parse(fullDataList[5]);
        transformValues[1] = float.Parse(fullDataList[6]);
        transformValues[2] = float.Parse(fullDataList[7]);
        m_updateTransform(transformValues);

        m_updateState(int.Parse(fullDataList[8]));
    }

    public void MarkAsDestroyed()
    {
        SetChangedDataToCurrentValues();
        m_updateState((int)ITEM_STATE.DESTROYED);
    }
}
