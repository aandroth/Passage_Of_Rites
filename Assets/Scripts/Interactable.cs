using System.Collections.Generic;
using UnityEngine;
using static WorkshopGame;

public abstract class Interactable : MonoBehaviour
{
    public bool m_isSupplier = true;
    public abstract SupplyStationName Interact(SupplyStationName supplyHeld, List<SupplyStationName> suppliesNeeded = null);
    public abstract bool PlayerCanInteract(SupplyStationName supplyHeld = SupplyStationName.NOTHING, List<SupplyStationName> suppliesNeeded = null);

    public abstract Vector3 GetCenterPoint();
}
