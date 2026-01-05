using System.Collections.Generic;
using UnityEngine;
using static NpcWander;

public class NetworkDataObject_Npc : MonoBehaviour
{
    public struct PreviousData_Npc
    {
        public List<float> m_prevTransformData;
        public int m_state;
    }

    public PreviousData_Npc m_prevData = new PreviousData_Npc();

    public delegate void UpdateTransform(List<float?> t);
    UpdateTransform m_updateTransform;
    public delegate void UpdateState(int s);
    UpdateState m_updateState;
    public delegate Vector4 GetCurrentValues();
    GetCurrentValues m_getCurrentValues;

    public void SetChangedDataToCurrentValues()
    {
        //"position_X, position_Y, transform.localScale_X, state";
        //          0,          1,                      2,     3
        Vector4 currValues = m_getCurrentValues();

        //"Action, id, position_X, position_Y, m_playerSprite.localScale_X, state";
        //      0,  1,          2,          3,                           4,     5
        /*2*/ m_prevData.m_prevTransformData[0] = currValues[0];
        /*3*/ m_prevData.m_prevTransformData[1] = currValues[1];
        /*4*/ m_prevData.m_prevTransformData[2] = currValues[2];
        /*5*/ m_prevData.m_state = (int)currValues[3];
    }
    public string GetChangedData()
    {
        //"position_X, position_Y, transform.localScale_X, state";
        //          0,          1,                      2,     3
        Vector4 currValues = m_getCurrentValues();

        //"Action, id, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,          2,          3,                      4,     5
        /*0,1,*/
        string changedData = $",{gameObject.GetInstanceID()},";
        /*2,  */ changedData += m_prevData.m_prevTransformData[0] != currValues[0]      ? $"{currValues[0]}," : ",";
        /*3,  */ changedData += m_prevData.m_prevTransformData[1] != currValues[1]      ? $"{currValues[1]}," : ",";
        /*4,  */ changedData += m_prevData.m_prevTransformData[2] != currValues[2]      ? $"{currValues[2]}," : ",";
        /*5,  */ changedData += m_prevData.m_state                != (int)currValues[3] ? $"{currValues[3]}," : "";
        if (changedData == $",{gameObject.GetInstanceID()},,,,")
        {
            return "Unchanged";
        }
        return changedData;
    }

    public void PutChangedData(string[] changedDataList)
    {
        //"Action, id, position_X, position_Y, transform.localScale_X, state";
        //      0,  1,          2,          3,                      4,     5

        List<float?> possibleTransformChanges = new List<float?>() { null, null, null };
        if (changedDataList[2] != "") possibleTransformChanges[0] = float.Parse(changedDataList[2]);
        if (changedDataList[3] != "") possibleTransformChanges[1] = float.Parse(changedDataList[3]);
        if (changedDataList[4] != "") possibleTransformChanges[2] = float.Parse(changedDataList[4]);
        m_updateTransform(possibleTransformChanges);

        if (changedDataList[5] != "") m_updateState(int.Parse(changedDataList[5]));
    }
}
