using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Game;
using static ItemObjectiveData;
using static ItemObjective ;

public class PlayerSupplyItem : MonoBehaviour, IAccessibleSupplyItem
{
    public SpriteRenderer m_supplyCarriedSpriteRenderer;
    public SupplyItemName m_supplyStationResourceName;
    public List<SupplyItemName> m_neededSuppliesList = new List<SupplyItemName>();

    [SerializeField] Interactable m_closestInteractableToMouse = null;
    List<Interactable> m_interactablesInPlayerRangeAndMouseRange = new List<Interactable>();
    public List<Interactable> m_interactablesInPlayerRange = new List<Interactable>();
    public CircleCollider2D m_circleCollider;

    public MouseFollowingCollider m_mouseFollowingCollider;
    public int iCount = 0;
    Vector3 screenPointOfMouse;

    public bool m_mouseColliderFrozen = false;
    [SerializeField] private bool m_mouseIsControllable = false;

    private void Start()
    {
        m_mouseFollowingCollider.ColliderEnter = AddInteractableToFocusListIfWithinRangeAndMouse;
        m_mouseFollowingCollider.ColliderExit = RemoveInteractableIfInFocusList;
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
                ExecuteInteraction();
            }
        }
    }

    public void AssignNeededSupplyItems(List<SupplyItemName> neededSuppliesList)
    {
        m_neededSuppliesList = neededSuppliesList;
    }

    public void ExecuteInteraction()
    {
        if (m_interactablesInPlayerRangeAndMouseRange.Count > 0)
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
        if (m_interactablesInPlayerRange.Contains(interactable) && m_mouseFollowingCollider.m_interactablesInCollider.Contains(interactable))
        {
            m_interactablesInPlayerRangeAndMouseRange.Add(interactable);
            iCount = m_interactablesInPlayerRangeAndMouseRange.Count;
            _ = GetClosestInteractableFromInteractablesList();
        }
    }

    public void RemoveInteractableIfInFocusList(Interactable interactable)
    {
        if (m_interactablesInPlayerRangeAndMouseRange.Contains(interactable))
        {
            m_interactablesInPlayerRangeAndMouseRange.Remove(interactable);
            iCount = m_interactablesInPlayerRangeAndMouseRange.Count;
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
        if(m_closestInteractableToMouse != closestInteractable)
        {
            m_closestInteractableToMouse?.OffFocus();
            closestInteractable?.OnFocus();
            m_closestInteractableToMouse = closestInteractable;
        }
        return closestInteractable;
    }

    public void InteractWithClosestInteractable(Interactable closestInteractable)
    {
        if(closestInteractable.IsSupplier())
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
        m_supplyCarriedSpriteRenderer.sprite = SpriteOfSupplyItem(supplyName);
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
    public SupplyItemName GetSupplyItemName()
    {
        return m_supplyStationResourceName;
    }
    public void SetSupplyItem(SupplyItemName supplyItemName)
    {
        if(supplyItemName != SupplyItemName.NOTHING)
            ActivateAndSetSupplyItem(supplyItemName);
        else
            DeactivateAndRemoveSupplyItem();
    }
    public void SetMouseIsControllable(bool mouseIsControllable)
    {
        m_mouseIsControllable = mouseIsControllable;
    }
}
