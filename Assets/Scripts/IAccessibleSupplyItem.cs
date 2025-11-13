using UnityEngine;

public interface IAccessibleSupplyItem
{
    public WorkshopGame.SupplyItemName GetSupplyItemName();
    public void SetSupplyItem(WorkshopGame.SupplyItemName supplyItemName);
}
