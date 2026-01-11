using System.Collections.Generic;
using UnityEngine;
using static NpcWander;

public class NetworkDataObject_Npc
{
    public struct PreviousData_Npc
    {
        public List<float> m_prevTransformData;
        public int m_state;
    }

    public PreviousData_Npc m_prevData = new PreviousData_Npc();

    public delegate int GetIdDelegate();
    public GetIdDelegate m_getId;
    public delegate int GetSpawnerIdDelegate();
    public GetSpawnerIdDelegate m_getSpawnerId;
    public delegate void SetIdSpawnerIdAndNpcTypeDelegate(int i, int s, int t);
    public SetIdSpawnerIdAndNpcTypeDelegate m_setIdSpawnerIdAndNpcType;
    public delegate void UpdateTransformDelegate(List<float?> t);
    public UpdateTransformDelegate m_updateTransform;
    public delegate void UpdateStateDelegate(int s);
    public UpdateStateDelegate m_updateState;
    public delegate List<float> GetCurrentValuesDelegate();
    public GetCurrentValuesDelegate m_getCurrentValues;
    public delegate List<float> GetAllCurrentValuesDelegate();
    public GetAllCurrentValuesDelegate m_getAllCurrentValues;
    public delegate void PlayerBecameGameOwnerDelegate();
    public PlayerBecameGameOwnerDelegate m_playerBecameGameOwner;

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

        //"Action, id, spawnerId, NpcType, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,         2,       3,          4,          5,                      6,     7
        /*0,1,*/
        int id = m_getId();
        string changedData = $",{id},,";
        /*2,  */ changedData += m_prevData.m_prevTransformData[0] != currValues[0]      ? $"{currValues[0]}," : ",";
        /*3,  */ changedData += m_prevData.m_prevTransformData[1] != currValues[1]      ? $"{currValues[1]}," : ",";
        /*4,  */ changedData += m_prevData.m_prevTransformData[2] != currValues[2]      ? $"{currValues[2]}," : ",";
        /*5,  */ changedData += m_prevData.m_state                != (int)currValues[3] ? $"{currValues[3]}" : "";
        if (changedData == $",{id},,,,,,")
        {
            return "Unchanged";
        }
        return changedData;
    }
    public string GetAllData()
    {
        //"id, type, position_X, position_Y, transform.localScale_X, state";
        //  0,    1,          2,          3,                      4,     5
        Debug.Log("Using m_getCurrentValues");
        List<float> currValues = m_getAllCurrentValues();

        //"Action, id, spawnerId, NpcType, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,         2,       3,          4,          5,                      6,     7
        /*0,1,*/ string allData = $",{currValues[0]},{currValues[1]},{currValues[2]},";
        /*3,  */ allData += $"{currValues[3]},";
        /*4,  */ allData += $"{currValues[4]},";
        /*5,  */ allData += $"{currValues[5]},";
        /*5,  */ allData += $"{currValues[6]},";
        /*6,  */ allData += $"{currValues[7]}";
        return allData;
    }

    public void PutChangedData(string[] changedDataList)
    {
        //"Action, id, spawnerId, NpcType, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,         2,       3,          4,          5,                      6,     7

        List<float?> possibleTransformChanges = new List<float?>() { null, null, null };
        if (changedDataList[4] != "") possibleTransformChanges[0] = float.Parse(changedDataList[4]);
        if (changedDataList[5] != "") possibleTransformChanges[1] = float.Parse(changedDataList[5]);
        if (changedDataList[6] != "") possibleTransformChanges[2] = float.Parse(changedDataList[6]);
        m_updateTransform(possibleTransformChanges);

        if (changedDataList[7] != "") m_updateState(int.Parse(changedDataList[7]));
    }

    public void PutAllData(string[] fullDataList)
    {
        //"Action, id, NpcType, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,       2,          3,          4,                      5,     6

        m_setIdSpawnerIdAndNpcType(int.Parse(fullDataList[1]), int.Parse(fullDataList[2]), int.Parse(fullDataList[3]));

        List<float?> transformValues = new List<float?>() { null, null, null };
        transformValues[0] = float.Parse(fullDataList[4]);
        transformValues[1] = float.Parse(fullDataList[5]);
        transformValues[2] = float.Parse(fullDataList[6]);
        m_updateTransform(transformValues);

        m_updateState(int.Parse(fullDataList[7]));
    }
}
