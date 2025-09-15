using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static WorkshopGame;

public class PlayerStation : Interactable
{
    public bool m_playerInRange = false;
    public List<SupplyStationName> m_neededSupplyItems;
    public int m_suppliesGatheredCount = 0;
    public List<SpriteRenderer> m_suppliesNeededIcons;
    public GameObject m_finishedTrapObject;
    public TMPro.TextMeshPro m_finishedTrapNameTMP;
    public SpriteRenderer m_finishedTrapSpriteRenderer;
    public float m_showCompletedTrapTime = 3f;
    public Sprite m_checkmarkSprite;
    public TrapType m_trapType;

    public delegate void ReportTrapCompleted(TrapType t);
    public ReportTrapCompleted m_reportTrapCompleted;
    public delegate void ReportSupplyCheckedOff();
    public ReportTrapCompleted m_reportSupplyCheckedOff;

    private void Start()
    {
        m_isSupplier = false;
    }

    public void AssignTrapToComplete(TrapType trapType)
    {
        m_trapType = trapType;
        m_neededSupplyItems.Clear();
        foreach (var item in m_trapToSuppliesDict[trapType])
        {
            m_neededSupplyItems.Add(item);
        }
        for (int i = 0; i < m_suppliesNeededIcons.Count; i++)
        {
            m_suppliesNeededIcons[i].sprite = WorkshopGame.m_nameToSpriteDict[m_neededSupplyItems[i]];
        }
        m_finishedTrapSpriteRenderer.sprite = m_trapToSpriteDict[trapType];
        m_finishedTrapNameTMP.text = m_trapToNameDict[trapType];
    }

    public override SupplyStationName Interact(SupplyStationName supplyHeld = SupplyStationName.NOTHING, List<SupplyStationName> suppliesNeeded = null)
    {
        CheckOffSupply(supplyHeld);
        if (m_suppliesGatheredCount == m_suppliesNeededIcons.Count)
        {
            StartCoroutine(CompleteTrapCoroutine());
        }
        return SupplyStationName.NOTHING;
    }
    public override bool PlayerCanInteract(SupplyStationName supplyHeld = SupplyStationName.NOTHING, List<SupplyStationName> suppliesNeeded = null)
    {
        return supplyHeld != SupplyStationName.NOTHING && m_neededSupplyItems.Contains(supplyHeld);
    }

    public override Vector3 GetCenterPoint()
    {
        return gameObject.transform.position;
    }

    public void CheckOffSupply(SupplyStationName supplyFromPlayer)
    {
        ++m_suppliesGatheredCount;
        int index = m_neededSupplyItems.IndexOf(supplyFromPlayer);
        m_suppliesNeededIcons[index].sprite = m_checkmarkSprite;
        m_neededSupplyItems[index] = SupplyStationName.NOTHING;
    }

    public IEnumerator CompleteTrapCoroutine()
    {
        m_finishedTrapObject.SetActive(true);
        float timeToShowCompletedTrapCountdown = m_showCompletedTrapTime;
        while(timeToShowCompletedTrapCountdown > 0)
        {
            timeToShowCompletedTrapCountdown -= Time.deltaTime;
            yield return null;
        }

        m_finishedTrapObject.SetActive(false);
        m_suppliesGatheredCount = 0;

        m_reportTrapCompleted(m_trapType);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_playerInRange=true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        m_playerInRange = false;
    }
}
