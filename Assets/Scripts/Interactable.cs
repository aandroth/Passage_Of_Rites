using System.Collections.Generic;
using UnityEngine;
using static ItemObjective;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] bool m_isSupplier = true;
    [SerializeField] bool m_isHighlightable = true;
    public bool m_playerInRange = false;
    [SerializeField] protected SpriteRenderer m_highlightSprite;
    public abstract SupplyItemName Interact(ItemObjective.SupplyItemName supplyHeld, List<ItemObjective.SupplyItemName> suppliesNeeded = null);
    public abstract bool PlayerCanInteract(ItemObjective.SupplyItemName supplyHeld = ItemObjective.SupplyItemName.NOTHING, List<ItemObjective.SupplyItemName> suppliesNeeded = null);
    public virtual void OnFocus() { }
    public virtual void OffFocus() { }

    public abstract Vector3 GetCenterPoint();
    public virtual bool IsSupplier() { return m_isSupplier; }
    public virtual bool IsHighlightable() { return m_isHighlightable; }
}
