using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static WorkshopGame;

public class OtherPlayerSupplyItem : Interactable, IAccessibleSupplyItem
{
    public bool m_isUsable = true, m_supplyNeededByPlayer = false;
    public WorkshopGame.SupplyItemName m_supplyStationResourceName = WorkshopGame.SupplyItemName.NOTHING;
    public SpriteRenderer m_supplyCarriedSpriteRenderer;
    public GameObject m_frameSpriteRenderer;
    public Transform m_centerPoint;
    public float m_minMouseDistanceToCenter = 2f;
    public float m_minPlayerDistanceToCenter = 2f;

    public delegate void PlayerControlsDazedCallback();
    public PlayerControlsDazedCallback m_playerControlsDazedCallback;


    public WorkshopSupplyStationCircleCollider m_otherPlayerSupplyCircleCollider;
    public bool m_playerInRange = false;

    private bool m_playerMouseInRange = false;
    private PlayerSupplyItem m_playerSupplyItemInRange;

    public void Start()
    {
        m_otherPlayerSupplyCircleCollider.m_onPlayerEnterDelegate = PlayerMovesIntoRange;
        m_otherPlayerSupplyCircleCollider.m_onPlayerExitDelegate = PlayerMovesOutOfRange;
        m_otherPlayerSupplyCircleCollider.m_onMouseEnterDelegate = PlayerMouseMovesIntoRange;
        m_otherPlayerSupplyCircleCollider.m_onMouseExitDelegate = PlayerMouseMovesOutOfRange;
    }

    public override Vector3 GetCenterPoint()
    {
        return m_centerPoint.transform.position;
    }

    public override SupplyItemName Interact(SupplyItemName supplyHeld = SupplyItemName.NOTHING, List<SupplyItemName> suppliesNeeded = null)
    {
        if (m_isUsable && suppliesNeeded.Contains(m_supplyStationResourceName))
        {
            var supplyName = SupplyStolen();
            return supplyName;
        }
        else
            return SupplyItemName.NOTHING;
    }

    public void HighlightSupplyIfNeeded()
    {
        if (m_isUsable && m_playerMouseInRange && 
            m_playerInRange && 
            m_playerSupplyItemInRange != null && 
            m_playerSupplyItemInRange.m_neededSuppliesList.Contains(m_supplyStationResourceName))
        {
            m_frameSpriteRenderer.SetActive(true);
        }
    }
    public void UnhighlightSupplyIfHighlighted()
    {
        if (m_frameSpriteRenderer.activeSelf)
        {
            m_frameSpriteRenderer.SetActive(false);
        }
    }

    public void PlayerMovesIntoRange(PlayerSupplyItem playerSupplyItem)
    {
        m_playerSupplyItemInRange = playerSupplyItem;
        m_playerInRange = true;
        HighlightSupplyIfNeeded();
    }
    public void PlayerMovesOutOfRange()
    {
        m_playerInRange = false;
        UnhighlightSupplyIfHighlighted();
    }

    public void PlayerMouseMovesIntoRange()
    {
        m_playerMouseInRange = true;
        HighlightSupplyIfNeeded();
    }
    public void PlayerMouseMovesOutOfRange()
    {
        m_playerMouseInRange = false;
        UnhighlightSupplyIfHighlighted();
    }

    public override bool PlayerCanInteract(SupplyItemName supplyHeld = SupplyItemName.NOTHING, List<SupplyItemName> suppliesNeeded = null)
    {
        return (m_isUsable && 
            suppliesNeeded.Contains(m_supplyStationResourceName) && 
            supplyHeld == SupplyItemName.NOTHING);
    }

    public WorkshopGame.SupplyItemName SupplyStolen()
    {
        var supplyName = m_supplyStationResourceName;
        UnhighlightSupplyIfHighlighted();
        DeactivateAndRemoveSupplyItem();
        if(m_playerControlsDazedCallback != null) m_playerControlsDazedCallback();
        return supplyName;
    }

    public void ActivateAndSetSupplyItem(SupplyItemName supplyName)
    {
        m_supplyCarriedSpriteRenderer.gameObject.SetActive(true);
        m_supplyCarriedSpriteRenderer.sprite = m_nameToSpriteDict[supplyName];
        m_supplyStationResourceName = supplyName;
        m_frameSpriteRenderer.SetActive(true);
    }
    public void DeactivateAndRemoveSupplyItem()
    {
        Debug.Log("DeactivateAndRemoveSupplyItem");
        m_supplyCarriedSpriteRenderer.sprite = default;
        m_supplyStationResourceName = SupplyItemName.NOTHING;
        m_supplyCarriedSpriteRenderer.gameObject.SetActive(false);
        m_frameSpriteRenderer.SetActive(false);
    }
    public WorkshopGame.SupplyItemName GetSupplyItemName()
    {
        return m_supplyStationResourceName;
    }
    public void SetSupplyItem(WorkshopGame.SupplyItemName supplyItemName)
    {
        if (supplyItemName != WorkshopGame.SupplyItemName.NOTHING)
            ActivateAndSetSupplyItem(supplyItemName);
        else
            DeactivateAndRemoveSupplyItem();
    }
}
