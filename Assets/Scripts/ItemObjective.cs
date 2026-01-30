using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static ItemObjectiveData;

public class ItemObjective : Interactable
{

    [SerializeField] public int m_ownerId = -1;
    [SerializeField] public ITEM_OWNER_TYPE m_ownerType = ITEM_OWNER_TYPE.SELF;
    [SerializeField] protected ITEM_STATE m_state = ITEM_STATE.NONE;


    [SerializeField] protected List<SpriteRenderer> m_suppliesNeededIcons = new List<SpriteRenderer>();
    [SerializeField] protected List<SupplyItemName> m_neededSupplyItems = new List<SupplyItemName>() {SupplyItemName.NOTHING};
    [SerializeField] protected SupplyItemName m_supplyItemOnInteraction = SupplyItemName.NOTHING;
    [SerializeField] protected SupplyItemName m_supplyItemOnCompletion = SupplyItemName.NOTHING;

    [SerializeField] protected List<SupplyItemName> m_neededSupplyItemsMasterList;
    [SerializeField] protected bool m_destroySelfOnCompletion = true;
    [SerializeField] public NetworkDataObject_Item m_networkDataObjectItem { get; private set; } = new NetworkDataObject_Item();

    public delegate void ReportItemCompletedDelegate();
    public ReportItemCompletedDelegate m_reportItemCompleted;
    public delegate void OnDestroyDelegate(int id);
    public OnDestroyDelegate m_onDestroy;


    public virtual void Start()
    {
        SetItemObjectiveValues();
    }

    public virtual void SetItemObjectiveValues()
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
        m_networkDataObjectItem.m_getParent = () => { return this; };
        m_networkDataObjectItem.m_passthroughExecuteInteract = ExecuteInteraction;
        m_networkDataObjectItem.m_passthroughSetItemValues = SetItemObjectiveValues;

        m_networkDataObjectItem.m_prevData.m_prevTransformData = new List<float>() { 0f, 0f, 0f };
        m_networkDataObjectItem.m_prevData.m_state = 0;

        m_networkDataObjectItem.SetChangedDataToCurrentValues();
    }


    public void AssignNeededSupplyItems(List<SupplyItemName> neededSuppliesList)
    {
        Debug.Log("AssignNeededSupplyItems");
        foreach (var s in m_neededSupplyItems)
        {
            Debug.Log("CanInteract: Needs " + s);
        }
        m_neededSupplyItems = new List<SupplyItemName>(neededSuppliesList);

        Debug.Log("After assignment");
        foreach (var s in m_neededSupplyItems)
        {
            Debug.Log("CanInteract: Needs " + s);
        }
    }

    public virtual bool IsObjectiveMet()
    {
        return m_neededSupplyItems.Count == 0;
    }

    public override Vector3 GetCenterPoint()
    {
        return gameObject.transform.position;
    }
    public override bool CanInteract(SupplyItemName supplyHeld = SupplyItemName.NOTHING)
    {
        foreach( var s in m_neededSupplyItems)
        {
            Debug.Log("CanInteract: Needs " + s);
        }
        Debug.Log("Supply is: " + supplyHeld);

        return m_neededSupplyItems.Contains(supplyHeld);
    }
    public void SetDestroySelfUponCompletion(bool b)
    {
        m_destroySelfOnCompletion = b;
    }

    public virtual void AttemptInteraction(Interactable interactable) 
    {
        if (!interactable.IsSupplier() || m_neededSupplyItems.Contains(interactable.GetItem()))
        {
            Debug.Log("ItemObject: Attempting interaction with " + interactable.gameObject.name);
            m_networkDataObjectItem.m_sendAttemptInteract(m_id, interactable.m_id);
        }
    }

    public override void ExecuteInteraction(Interactable interactable)
    {
        m_neededSupplyItems.Remove(interactable.Interact(m_supplyItemName));
        if (IsObjectiveMet())
        {
            m_reportItemCompleted?.Invoke();
            m_neededSupplyItems = new List<SupplyItemName>(m_neededSupplyItemsMasterList);
            if (m_destroySelfOnCompletion) DestroySelf();
        }
        m_state = ITEM_STATE.NONE;
    }
    public override SupplyItemName Interact(SupplyItemName s = SupplyItemName.NOTHING, bool isFromPlayer = false)
    {
        m_neededSupplyItems.Remove(s);
        if (IsObjectiveMet())
        {
            m_reportItemCompleted?.Invoke();
            m_neededSupplyItems = new List<SupplyItemName>(m_neededSupplyItemsMasterList);
            if (m_destroySelfOnCompletion) DestroySelf();
            return m_supplyItemOnCompletion;
        }
        m_state = ITEM_STATE.NONE;
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
        SetItem((SupplyItemName)(itemType));
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
            (int)m_supplyItemName,
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

    protected void DestroySelf()
    {
        Destroy(gameObject);
    }
}
