using UnityEngine;

public interface IAccessibleSupplyItem
{
    public ItemObjective.SupplyItemName GetSupplyItemName();
    public void SetSupplyItem(ItemObjective.SupplyItemName supplyItemName);
}
