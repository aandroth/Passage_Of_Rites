using UnityEngine;
using static ItemObjectiveData;

public class ItemObjective_Receiver : ItemObjective
{
    public override bool CanInteract(SupplyItemName supplyHeld = SupplyItemName.NOTHING)
    {
        foreach (var s in m_neededSupplyItems)
        {
            Debug.Log("CanInteract: Needs " + s);
        }
        Debug.Log("Supply is: " + supplyHeld);

        return m_neededSupplyItems.Contains(supplyHeld);
    }
}
