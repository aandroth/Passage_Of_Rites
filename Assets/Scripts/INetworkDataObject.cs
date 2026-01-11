using UnityEngine;
using static NpcWander;
using static UnityEngine.RuleTile.TilingRuleOutput;

public interface INetworkDataObject
{
    public void SetChangedDataToCurrentValues();
    public string GetChangedData();
    public string GetAllData();

    public void PutChangedData(string[] changedDataList);
}
