using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WorkshopGame;

public class WorkshopSupplyStation : Interactable
{
    public const float M_TIME_TILL_USABLE_AGAIN = 5f;
    public float m_timeTillUsableCountdown = 0, m_animatorSpeedSafetyMargin = 0.1f;
    public bool m_isUsable = true, m_supplyNeededByPlayer = false;
    public WorkshopGame.SupplyStationName m_supplyStationResourceName = WorkshopGame.SupplyStationName.METAL;
    public SpriteRenderer m_supplyImageRenderer, m_frameSpriteRenderer;
    public Sprite m_supplyFrameHighlightSprite, m_supplyFrameDefaultSprite;
    public Animator m_supplyFrameAnimator;
    private string m_supplyFrameResetAnimationName = "SupplyFrameResetting";
    public Transform m_centerPoint;
    public float m_minMouseDistanceToCenter = 2f;
    public float m_minPlayerDistanceToCenter = 2f;

    public WorkshopSupplyStationCircleCollider m_supplyStationCircleCollider;
    public bool m_playerInRange = false;

    private bool m_playerMouseInRange = false;
    private PlayerSupplyItem m_playerSupplyItemInRange;


    public void Start()
    {
        m_supplyFrameAnimator.speed = 1f / M_TIME_TILL_USABLE_AGAIN + m_animatorSpeedSafetyMargin;
        m_supplyStationCircleCollider.m_onPlayerEnterDelegate = PlayerMovesIntoRange;
        m_supplyStationCircleCollider.m_onPlayerExitDelegate = PlayerMovesOutOfRange;
        m_supplyStationCircleCollider.m_onMouseEnterDelegate = PlayerMouseMovesIntoRange;
        m_supplyStationCircleCollider.m_onMouseExitDelegate = PlayerMouseMovesOutOfRange;
    }


    public override SupplyStationName Interact(SupplyStationName supplyHeld = SupplyStationName.NOTHING, List<SupplyStationName> suppliesNeeded = null)
    {
        if (m_isUsable && suppliesNeeded.Contains(m_supplyStationResourceName))
        {
            SupplyTaken();
            return m_supplyStationResourceName;
        }
        else
            return SupplyStationName.NOTHING;
    }
    public override bool PlayerCanInteract(SupplyStationName supplyHeld = SupplyStationName.NOTHING, List<SupplyStationName> suppliesNeeded = null)
    {
        return (m_isUsable && suppliesNeeded.Contains(m_supplyStationResourceName) && supplyHeld == SupplyStationName.NOTHING);
    }

    public override Vector3 GetCenterPoint()
    {
        return m_centerPoint.transform.position;
    }

    public void SetSupply(WorkshopGame.SupplyStationName supplyName, Sprite supplySprite)
    {
        m_supplyStationResourceName = supplyName;
        m_supplyImageRenderer.sprite = supplySprite;
    }

    public void SupplyTaken()
    {
        m_isUsable = false;
        UnhighlightSupplyIfHighlighted();
        StartCoroutine(SupplyTakenCoroutine());
    }
    public IEnumerator SupplyTakenCoroutine()
    {
        m_timeTillUsableCountdown = M_TIME_TILL_USABLE_AGAIN;
        m_supplyFrameAnimator.enabled = true;
        m_supplyFrameAnimator.Play(m_supplyFrameResetAnimationName);
        while (m_timeTillUsableCountdown > 0)
        {
            m_timeTillUsableCountdown -= Time.deltaTime;
            yield return null;
        }
        m_timeTillUsableCountdown = 0;
        m_isUsable = true;
        m_supplyFrameAnimator.Play("Idle");
        m_supplyFrameAnimator.enabled = false;
        m_frameSpriteRenderer.sprite = m_supplyFrameDefaultSprite;
        if (m_playerInRange)
            HighlightSupplyIfNeeded();
    }
    public bool SupplyNeededByPlayer(List<WorkshopGame.SupplyStationName> supplyNames)
    {
        return supplyNames.Contains(m_supplyStationResourceName);
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

    public void HighlightSupplyIfNeeded()
    {
        if (m_isUsable && m_playerMouseInRange && m_playerInRange && m_playerSupplyItemInRange != null && m_playerSupplyItemInRange.m_neededSuppliesList.Contains(m_supplyStationResourceName))
        {
            m_supplyFrameAnimator.Play("Idle");
            m_supplyFrameAnimator.enabled = false;
            m_frameSpriteRenderer.sprite = m_supplyFrameHighlightSprite;
        }
    }
    public void UnhighlightSupplyIfHighlighted()
    {
        if (m_frameSpriteRenderer.sprite == m_supplyFrameHighlightSprite)
        {
            m_supplyFrameAnimator.Play("Idle");
            m_supplyFrameAnimator.enabled = false;
            m_frameSpriteRenderer.sprite = m_supplyFrameDefaultSprite;
        }
    }
}
