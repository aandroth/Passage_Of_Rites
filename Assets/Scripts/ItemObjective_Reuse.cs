using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ItemObjectiveData;

public class ItemObjective_Reuse : ItemObjective
{
    [SerializeField] private List<SupplyItemName> m_neededSupplyItemsMasterList;
    [SerializeField] private bool m_destroySelfOnCompletion;

    public delegate void ReportItemCompleted();
    public ReportItemCompleted m_reportItemCompleted;

    public enum ITEM_STATE { NONE, PICKED_UP, DROPPED, COMPLETED, DESTROYED }

    public void Start()
    {
        m_neededSupplyItemsMasterList = new List<SupplyItemName>(m_neededSupplyItems);
    }

    public void SetDestroySelfUponCompletion(bool b)
    {
        m_destroySelfOnCompletion = b;
    }

    public override SupplyItemName Interact(SupplyItemName supplyHeld, List<SupplyItemName> suppliesNeeded = null)
    {
        m_neededSupplyItems.Remove(supplyHeld);
        if (IsObjectiveMet())
        {
            m_reportItemCompleted?.Invoke();
            m_neededSupplyItems = new List<SupplyItemName>(m_neededSupplyItemsMasterList);
            if(m_destroySelfOnCompletion) DestroySelf();
            return m_supplyItemOnCompletion;
        }
        return m_supplyItemOnInteraction;
    }

    public override void OnFocus()
    {
        if (IsHighlightable()) { m_highlightSprite?.gameObject.SetActive(true); }
    }

    public override void OffFocus()
    {
        if (IsHighlightable()) { m_highlightSprite?.gameObject.SetActive(false); }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
