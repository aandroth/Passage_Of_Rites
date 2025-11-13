using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static WorkshopGame;

public class PlayerSupplyItem : MonoBehaviour, IAccessibleSupplyItem
{
    public SpriteRenderer m_supplyCarriedSpriteRenderer;
    public SupplyItemName m_supplyStationResourceName;
    public List<SupplyItemName> m_neededSuppliesList;

    List<Interactable> m_interactablesInRangeAndMouseFocus = new List<Interactable>();
    public List<Interactable> m_interactablesInRange = new List<Interactable>();
    public CircleCollider2D m_circleCollider;

    public MouseFollowingCollider m_mouseFollowingCollider;
    public int iCount = 0;
    Vector3 screenPointOfMouse;

    public bool m_mouseColliderFrozen = false;

    private void Start()
    {
        m_mouseFollowingCollider.ColliderEnter = AddInteractableToFocusListIfWithinRangeAndMouse;
        m_mouseFollowingCollider.ColliderExit = RemoveInteractableIfInFocusList;
    }

    private void Update()
    {
        screenPointOfMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        screenPointOfMouse.z = 0;

        if (!m_mouseColliderFrozen)
        {
            m_mouseFollowingCollider.transform.position = screenPointOfMouse;
        }

        if(Input.GetMouseButtonUp(0))
        {
            ExecuteInteraction();
        }
    }

    public void AssignTrapToComplete(TrapType trapName)
    {
        Debug.Log("TrapAssignedToPlayer");
        m_neededSuppliesList = WorkshopGame.m_trapToSuppliesDict[trapName].ToList();
    }

    public void ExecuteInteraction()
    {
        if (m_interactablesInRangeAndMouseFocus.Count > 0)
        {
            Interactable closestInteractable = GetClosestInteractableFromInteractablesList();
            if (closestInteractable.PlayerCanInteract(m_supplyStationResourceName, m_neededSuppliesList))
            {
                InteractWithClosestInteractable(closestInteractable);
            }
        }
    }

    public void AddInteractableToFocusListIfWithinRangeAndMouse(Interactable interactable)
    {
        if (m_interactablesInRange.Contains(interactable) && m_mouseFollowingCollider.m_interactablesInCollider.Contains(interactable))
        {
            m_interactablesInRangeAndMouseFocus.Add(interactable);
            iCount = m_interactablesInRangeAndMouseFocus.Count;
        }
    }

    public void RemoveInteractableIfInFocusList(Interactable interactable)
    {
        if (m_interactablesInRangeAndMouseFocus.Contains(interactable))
        {
            m_interactablesInRangeAndMouseFocus.Remove(interactable);
            iCount = m_interactablesInRangeAndMouseFocus.Count;
        }
    }

    public Interactable GetClosestInteractableFromInteractablesList()
    {
        float minDistance = float.MaxValue;
        Interactable closestInteractable = null;
        foreach (var item in m_interactablesInRangeAndMouseFocus)
        {
            float dist = Vector3.Distance(m_mouseFollowingCollider.transform.position, item.GetCenterPoint());
            if (minDistance > dist)
            {
                minDistance = dist;
                closestInteractable = item;
            }
        }
        return closestInteractable;
    }

    public void InteractWithClosestInteractable(Interactable closestInteractable)
    {
        if(closestInteractable.m_isSupplier)
            ActivateAndSetSupplyItem(closestInteractable.Interact(m_supplyStationResourceName, m_neededSuppliesList));
        else
        {
            closestInteractable.Interact(m_supplyStationResourceName);
            if(m_neededSuppliesList.Contains(m_supplyStationResourceName))
                m_neededSuppliesList[m_neededSuppliesList.IndexOf(m_supplyStationResourceName)] = SupplyItemName.NOTHING;
            DeactivateAndRemoveSupplyItem();
        }
    }

    public void ActivateAndSetSupplyItem(SupplyItemName supplyName)
    {
        m_supplyCarriedSpriteRenderer.gameObject.SetActive(true);
        m_supplyCarriedSpriteRenderer.sprite = m_nameToSpriteDict[supplyName];
        m_supplyStationResourceName = supplyName;
    }
    public void DeactivateAndRemoveSupplyItem()
    {
        Debug.Log("DeactivateAndRemoveSupplyItem");
        m_supplyCarriedSpriteRenderer.sprite = default;
        m_supplyStationResourceName = SupplyItemName.NOTHING;
        m_supplyCarriedSpriteRenderer.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            m_interactablesInRange.Add(interactable);
            AddInteractableToFocusListIfWithinRangeAndMouse(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            m_interactablesInRange.Remove(interactable);
            RemoveInteractableIfInFocusList(collision.gameObject.GetComponent<Interactable>());
        }
    }
    public WorkshopGame.SupplyItemName GetSupplyItemName()
    {
        return m_supplyStationResourceName;
    }
    public void SetSupplyItem(WorkshopGame.SupplyItemName supplyItemName)
    {
        if(supplyItemName != WorkshopGame.SupplyItemName.NOTHING)
            ActivateAndSetSupplyItem(supplyItemName);
        else
            DeactivateAndRemoveSupplyItem();
    }
}
