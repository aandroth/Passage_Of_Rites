using System.Collections.Generic;
using UnityEngine;
using static ItemObjectiveData;

public class ItemObjective_Player : ItemObjective
{
    public SpriteRenderer m_supplyCarriedSpriteRenderer;

    [SerializeField] Interactable m_closestInteractableToMouse = null;
    List<Interactable> m_interactablesInPlayerRangeAndMouseRange = new List<Interactable>();
    public List<Interactable> m_interactablesInPlayerRange = new List<Interactable>();
    public CircleCollider2D m_circleCollider;

    public MouseFollowingCollider m_mouseFollowingCollider;
    public int interctablesCount = 0;
    Vector3 screenPointOfMouse;

    public bool m_mouseColliderFrozen = false;
    [SerializeField] private bool m_mouseIsControllable = false;

    public delegate void DazedCallbackDelegate();
    public DazedCallbackDelegate m_dazedCallback;


    public override void Start()
    {
        m_supplyItemName = SupplyItemName.NOTHING;
        m_ownerType = ITEM_OWNER_TYPE.PLAYER;
        m_mouseFollowingCollider.ColliderEnter = AddInteractableToFocusListIfWithinRangeAndMouse;
        m_mouseFollowingCollider.ColliderExit = RemoveInteractableIfInFocusList;
        SetItemObjectiveValues();
    }

    private void Update()
    {
        if (m_mouseIsControllable)
        {
            screenPointOfMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenPointOfMouse.z = 0;

            if (!m_mouseColliderFrozen)
            {
                m_mouseFollowingCollider.transform.position = screenPointOfMouse;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("MOuse Click");
                if (m_interactablesInPlayerRangeAndMouseRange.Count > 0)
                {
                    Interactable closestInteractable = GetClosestInteractableFromInteractablesList();
                    Debug.Log($"Closest in range is {closestInteractable.gameObject.name}");
                    if(closestInteractable.m_serverlessInteract) ActivateAndSetSupplyItem(closestInteractable.Interact(m_supplyItemName, true));
                    else AttemptInteraction(closestInteractable);
                }
            }
        }
    }

    public override void ExecuteInteraction(Interactable i) { ActivateAndSetSupplyItem(i.Interact(m_supplyItemName, true)); }

    public override SupplyItemName Interact(SupplyItemName s = SupplyItemName.NOTHING, bool isFromPlayer = false)
    {
        if (isFromPlayer || m_supplyItemName == SupplyItemName.NOTHING)
            m_dazedCallback();
        SupplyItemName temp = m_supplyItemName;
        m_supplyItemName = s;
        return temp;
    }

    public void AddInteractableToFocusListIfWithinRangeAndMouse(Interactable interactable)
    {
        if (m_interactablesInPlayerRange.Contains(interactable) && m_mouseFollowingCollider.m_interactablesInCollider.Contains(interactable) &&
            (InteractableIsSupplierAndHasItemPlayerNeeds(interactable)) ||
            InteractableIsReceiverAndNeedsItemPlayerHas(interactable))
        {
            m_interactablesInPlayerRangeAndMouseRange.Add(interactable);
            interctablesCount = m_interactablesInPlayerRangeAndMouseRange.Count;
            _ = GetClosestInteractableFromInteractablesList();
        }
    }

    public bool InteractableIsSupplierAndHasItemPlayerNeeds(Interactable interactable)
    {
        return interactable.IsSupplier() && CanInteract(interactable.GetItem());
    }

    public bool InteractableIsReceiverAndNeedsItemPlayerHas(Interactable interactable)
    {
        return !interactable.IsSupplier() && interactable.CanInteract(m_supplyItemName);
    }


    public void RemoveInteractableIfInFocusList(Interactable interactable)
    {
        if (m_interactablesInPlayerRangeAndMouseRange.Contains(interactable))
        {
            m_interactablesInPlayerRangeAndMouseRange.Remove(interactable);
            interctablesCount = m_interactablesInPlayerRangeAndMouseRange.Count;
            _ = GetClosestInteractableFromInteractablesList();
        }
    }

    public Interactable GetClosestInteractableFromInteractablesList()
    {
        float minDistance = float.MaxValue;
        Interactable closestInteractable = null;
        foreach (var item in m_interactablesInPlayerRangeAndMouseRange)
        {
            float dist = Vector3.Distance(m_mouseFollowingCollider.transform.position, item.GetCenterPoint());
            if (minDistance > dist)
            {
                minDistance = dist;
                closestInteractable = item;
            }
        }
        if (m_closestInteractableToMouse != closestInteractable)
        {
            m_closestInteractableToMouse?.OffFocus();
            closestInteractable?.OnFocus();
            m_closestInteractableToMouse = closestInteractable;
        }
        return closestInteractable;
    }

    public void InteractWithClosestInteractable(Interactable closestInteractable)
    {
        //Send Item_Interaction to server


        //if (closestInteractable.IsSupplier())
        //    ActivateAndSetSupplyItem(closestInteractable.Interact(m_supplyItemName, m_neededSupplyItems));
        //else
        //{
        //    closestInteractable.Interact(m_supplyItemName);
        //    if (m_neededSupplyItems.Contains(m_supplyItemName))
        //        m_neededSupplyItems[m_neededSupplyItems.IndexOf(m_supplyItemName)] = SupplyItemName.NOTHING;
        //    DeactivateAndRemoveSupplyItem();
        //}
    }


    public void ActivateAndSetSupplyItem(SupplyItemName supplyName)
    {
        m_supplyCarriedSpriteRenderer.gameObject.SetActive(true);
        m_supplyCarriedSpriteRenderer.sprite = SpriteOfSupplyItem(supplyName);
        m_supplyItemName = supplyName;
    }
    public void DeactivateAndRemoveSupplyItem()
    {
        Debug.Log("DeactivateAndRemoveSupplyItem");
        m_supplyCarriedSpriteRenderer.sprite = default;
        m_supplyItemName = SupplyItemName.NOTHING;
        m_supplyCarriedSpriteRenderer.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"OnTriggerEnter2D in PlayerSupplyItem, found {collision.gameObject.name}");
        Interactable interactable = collision.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            m_interactablesInPlayerRange.Add(interactable);
            AddInteractableToFocusListIfWithinRangeAndMouse(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            m_interactablesInPlayerRange.Remove(interactable);
            RemoveInteractableIfInFocusList(collision.gameObject.GetComponent<Interactable>());
        }
    }
    public override SupplyItemName GetSupplyItemName()
    {
        return m_supplyItemName;
    }
    public override void SetSupplyItem(SupplyItemName supplyItemName)
    {
        if (supplyItemName != SupplyItemName.NOTHING)
            ActivateAndSetSupplyItem(supplyItemName);
        else
            DeactivateAndRemoveSupplyItem();
    }
    public void SetMouseIsControllable(bool mouseIsControllable)
    {
        m_mouseIsControllable = mouseIsControllable;
    }
}
