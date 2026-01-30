using System.Collections.Generic;
using UnityEngine;
using static ItemObjectiveData;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] public int m_id { get; protected set; }
    [SerializeField] private bool m_isSupplier = true;
    [SerializeField] public bool m_serverlessInteract = false;
    [SerializeField] bool m_isHighlightable = true;
    public bool m_playerInRange = false;
    [SerializeField] protected SpriteRenderer m_highlightSprite;
    [SerializeField] protected float m_interactionCooldown;
    [SerializeField] protected SupplyItemName m_supplyItemName = SupplyItemName.POT_LID;

    public virtual void ExecuteInteraction(Interactable i) { m_supplyItemName = i.Interact(m_supplyItemName); }
    public virtual SupplyItemName Interact(SupplyItemName s = SupplyItemName.NOTHING, bool isFromPlayer = false) { SupplyItemName temp = m_supplyItemName;
                                                                                        m_supplyItemName = s;
                                                                                        return temp; }
    public virtual void SetItem(SupplyItemName itemName = SupplyItemName.NOTHING){ m_supplyItemName = itemName; }
    public virtual SupplyItemName GetItem(){ return m_supplyItemName; }
    public abstract bool CanInteract(SupplyItemName supplyHeld = SupplyItemName.NOTHING);
    public virtual void OnFocus() { }
    public virtual void OffFocus() { }

    public abstract Vector3 GetCenterPoint();
    public virtual bool IsSupplier() { return m_isSupplier; }
    public virtual bool IsHighlightable() { return m_isHighlightable; }

    public virtual SupplyItemName GetSupplyItemName() { return SupplyItemName.NOTHING; }
    public virtual void SetSupplyItem(SupplyItemName supplyItemName) { }

    //public struct ItemObjectiveIcon{
    //    public SpriteRenderer m_iconSpriteRenderer;
    //    public Animator       m_iconSpriteAnimator;
    //    public bool           m_isResusable;
    //    public float          m_resusableCooldown;

    //}
}
