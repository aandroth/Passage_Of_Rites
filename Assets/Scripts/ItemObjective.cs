using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ItemObjectiveData;
using static ItemTypeData;

public class ItemObjective : Interactable
{
    [SerializeField] public int m_id { get; private set; }
    [SerializeField] public int m_ownerId = -1;
    [SerializeField] protected ITEM_TYPES m_itemType = ITEM_TYPES.PICKUP;
    [SerializeField] public ITEM_OWNER_TYPE m_ownerType = ITEM_OWNER_TYPE.SELF;
    [SerializeField] protected ITEM_STATE m_state = ITEM_STATE.NONE;


    [SerializeField] protected List<SpriteRenderer> m_suppliesNeededIcons = new List<SpriteRenderer>();
    [SerializeField] protected List<SupplyItemName> m_neededSupplyItems = new List<SupplyItemName>();
    [SerializeField] protected SupplyItemName m_supplyItemOnInteraction = SupplyItemName.NOTHING;
    [SerializeField] protected SupplyItemName m_supplyItemOnCompletion = SupplyItemName.NOTHING;

    [SerializeField] protected List<SupplyItemName> m_neededSupplyItemsMasterList;
    [SerializeField] protected bool m_destroySelfOnCompletion;
    [SerializeField] public NetworkDataObject_Item m_networkDataObjectItem { get; private set; } = new NetworkDataObject_Item();

    public delegate void ReportItemCompletedDelegate();
    public ReportItemCompletedDelegate m_reportItemCompleted;
    public delegate void OnDestroyDelegate(int id);
    public OnDestroyDelegate m_onDestroy;


    public virtual void Start()
    {
        SetItemObjectiveValues();
    }

    public void SetItemObjectiveValues()
    {
        m_id = gameObject.GetInstanceID();
        m_neededSupplyItemsMasterList = new List<SupplyItemName>(m_neededSupplyItems);
    }
    public void FillNetworkDataItemObjectDelegates()
    {
        m_networkDataObjectItem.m_getId = () => { return m_id; };
        m_networkDataObjectItem.m_getOwnerId = () => { return m_ownerId; };
        m_networkDataObjectItem.m_setIdSpawnerIdOwnerTypeAndNpcType = SetIdOwnerIdItemTypeAndOwnerType;
        m_networkDataObjectItem.m_getCurrentValues = GetCurrentValues;
        m_networkDataObjectItem.m_getAllCurrentValues = GetAllCurrentValues;
        m_networkDataObjectItem.m_getCurrentState = () => { return m_state; };
        m_networkDataObjectItem.m_updateTransform = UpdateTransformValues;
        m_networkDataObjectItem.m_updateState = UpdateState;

        m_networkDataObjectItem.m_prevData.m_prevTransformData = new List<float>() { 0f, 0f, 0f };
        m_networkDataObjectItem.m_prevData.m_state = 0;

        m_networkDataObjectItem.SetChangedDataToCurrentValues();
    }

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
            if (m_destroySelfOnCompletion) DestroySelf();
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

    public void SetIdOwnerIdItemTypeAndOwnerType(int id, int ownerId, int itemType, int ownerType)
    {
        m_id = id;
        m_ownerId = ownerId;
        m_itemType = (ITEM_TYPES)(itemType);
        m_ownerType = (ITEM_OWNER_TYPE)(ownerType);
    }

    public void SetOwnerIdAndType_FromOwner(int ownerId, ITEM_OWNER_TYPE ownerType)
    {
        m_ownerId = ownerId;
        m_ownerType = ownerType;
    }

    public void UpdateTransformValues(List<float?> possibleValues)
    {
        Vector3 position = transform.localPosition;
        if (possibleValues[0] != null) position.x = (float)possibleValues[0];
        if (possibleValues[1] != null) position.y = (float)possibleValues[1];
        if (possibleValues[0] != null || possibleValues[1] != null)
            transform.localPosition = position;

        if (possibleValues[2] != null)
        {
            Vector3 newScale = transform.localScale;
            newScale.x = (float)possibleValues[2];
            transform.localScale = newScale;
        }
    }

    public void UpdateState(int newState)
    {
        Debug.Log($"UpdateState: {newState}, {(ITEM_STATE)newState}");
        ITEM_STATE oldState = m_state;
        m_state = (ITEM_STATE)newState;
        if (m_state == ITEM_STATE.DESTROYED && oldState != ITEM_STATE.DESTROYED)
        {
            Debug.Log("State update to DESTROYED. Destroying item");
            Destroy(gameObject);
        }
    }


    public List<float> GetCurrentValues()
    {
        return new List<float>() {
            transform.localPosition.x,
            transform.localPosition.y,
            transform.localScale.x,
            (float)m_state
        };
    }

    public List<float> GetAllCurrentValues()
    {
        return new List<float>() {
            m_id,
            m_ownerId,
            (int)m_itemType,
            (int)m_ownerType,
            transform.localPosition.x,
            transform.localPosition.y,
            transform.localScale.x,
            (float)m_state
        };
    }
    public void OnDestroy()
    {
        m_onDestroy?.Invoke(m_id);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
