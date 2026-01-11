using System.Collections.Generic;
using UnityEngine;
using static ItemObjectiveData;

public abstract class ItemObjective : Interactable
{
    [SerializeField] public int m_id { get; private set; }
    [SerializeField] public int m_idOfOwningNpc = -1;
    [SerializeField] protected List<SpriteRenderer> m_suppliesNeededIcons = new List<SpriteRenderer>();
    [SerializeField] protected List<SupplyItemName> m_neededSupplyItems = new List<SupplyItemName>();
    [SerializeField] protected SupplyItemName m_supplyItemOnInteraction = SupplyItemName.NOTHING;
    [SerializeField] protected SupplyItemName m_supplyItemOnCompletion = SupplyItemName.NOTHING;


    // abstract method to check if the objective is met
    public virtual bool IsObjectiveMet()
    {
        return m_neededSupplyItems.Count == 0;
    }

    public override Vector3 GetCenterPoint()
    {
        return gameObject.transform.position;
    }
    public override bool PlayerCanInteract(SupplyItemName supplyHeld = SupplyItemName.NOTHING, List<SupplyItemName> suppliesNeeded = null)
    {
        return m_neededSupplyItems.Contains(supplyHeld);
    }
}
