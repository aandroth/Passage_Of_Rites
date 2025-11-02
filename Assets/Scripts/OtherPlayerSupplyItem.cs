using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static WorkshopGame;

public class OtherPlayerSupplyItem : Interactable
{
    public bool m_isUsable = true, m_supplyNeededByPlayer = false;
    public WorkshopGame.SupplyStationName m_supplyStationResourceName = WorkshopGame.SupplyStationName.METAL;
    public SpriteRenderer m_supplyImageRenderer;
    public GameObject m_frameSpriteRenderer;
    public Transform m_centerPoint;
    public float m_minMouseDistanceToCenter = 2f;
    public float m_minPlayerDistanceToCenter = 2f;
    public PlayerControls m_playerControls;

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

    public override SupplyStationName Interact(SupplyStationName supplyHeld = SupplyStationName.NOTHING, List<SupplyStationName> suppliesNeeded = null)
    {
        if (m_isUsable && suppliesNeeded.Contains(m_supplyStationResourceName))
        {
            var supplyName = SupplyStolen();
            return supplyName;
        }
        else
            return SupplyStationName.NOTHING;
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

    public override bool PlayerCanInteract(SupplyStationName supplyHeld = SupplyStationName.NOTHING, List<SupplyStationName> suppliesNeeded = null)
    {
        return (m_isUsable && 
            suppliesNeeded.Contains(m_supplyStationResourceName) && 
            supplyHeld == SupplyStationName.NOTHING);
    }

    public WorkshopGame.SupplyStationName SupplyStolen()
    {
        // Inform backend
        m_supplyImageRenderer.sprite = null;
        UnhighlightSupplyIfHighlighted();
        var supplyName = m_supplyStationResourceName;
        m_supplyStationResourceName = SupplyStationName.NOTHING;
        m_playerControls.Dazed();
        return supplyName;
    }
}
