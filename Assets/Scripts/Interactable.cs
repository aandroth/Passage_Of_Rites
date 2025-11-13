using System.Collections.Generic;
using UnityEngine;
using static WorkshopGame;

public abstract class Interactable : MonoBehaviour
{
    public bool m_isSupplier = true;
    public abstract SupplyItemName Interact(SupplyItemName supplyHeld, List<SupplyItemName> suppliesNeeded = null);
    public abstract bool PlayerCanInteract(SupplyItemName supplyHeld = SupplyItemName.NOTHING, List<SupplyItemName> suppliesNeeded = null);

    public abstract Vector3 GetCenterPoint();
}
