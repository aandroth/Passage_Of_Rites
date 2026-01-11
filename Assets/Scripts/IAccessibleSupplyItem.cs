using UnityEngine;
using static ItemObjectiveData;

public interface IAccessibleSupplyItem
{
    public SupplyItemName GetSupplyItemName();
    public void SetSupplyItem(SupplyItemName supplyItemName);
}
