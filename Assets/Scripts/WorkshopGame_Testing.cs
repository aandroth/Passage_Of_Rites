#if UNITY_EDITOR
using NUnit.Framework;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopGame_Testing : MonoBehaviour
{
#if UNITY_EDITOR
    public GameObject m_stationPrefab = null;
    public GameObject m_playerSupplyItemPrefab = null;
    public WorkshopGame m_workshopGame = null;
    WorkshopSupplyStation m_station = null;
    public SpriteRenderer m_spriteRenderer;
    public PlayerSupplyItem m_playerSupplyItem = null;

    public void Update()
    {
        if(Input.GetKeyUp(KeyCode.L))
            StartCoroutine(ExecuteTests());
    }

    public IEnumerator ExecuteTests()
    {
        StartCoroutine(TestSupplyStation(5));
        yield return new WaitForSeconds(20);
        StartCoroutine(TestPlayerSupplyItemInteractingWithSupplyStation(5));
        yield return new WaitForSeconds(20);
        StartCoroutine(TestPlayerSupplyItemInteractingWithSupplyStation(5));
    }

    public IEnumerator TestSupplyStation(float timeToWait)
    {
        Debug.Log("Testing SupplyStation");
        if (m_station == null)
        {
            m_station = GameObject.Instantiate(m_stationPrefab).GetComponent<WorkshopSupplyStation>();
            m_workshopGame.m_supplyStations.Add(m_station);
        }
        Debug.Log("Station Created");
        Debug.Log("Testing m_isUsable and isActiveAndEnabled");
        yield return new WaitForSeconds(6);

        Assert.True(m_station.m_isUsable);
        Assert.True(m_station.isActiveAndEnabled);

        Debug.Log("Testing interaction");
        Assert.AreEqual(WorkshopGame.SupplyItemName.NOTHING, m_station.Interact(WorkshopGame.SupplyItemName.NOTHING, new List<WorkshopGame.SupplyItemName>() { WorkshopGame.SupplyItemName.METAL }));
        Assert.AreEqual(WorkshopGame.SupplyItemName.ROPE, m_station.Interact(WorkshopGame.SupplyItemName.NOTHING, new List<WorkshopGame.SupplyItemName>() { WorkshopGame.SupplyItemName.ROPE }));
        Assert.False(m_station.m_isUsable);

        Debug.Log("Testing m_isUsable and sprite");
        yield return new WaitForSeconds(timeToWait + 1);
        Assert.True(m_station.m_isUsable);
        Assert.AreEqual(m_station.m_supplyImageRenderer.sprite, m_spriteRenderer.sprite);
        yield return new WaitForSeconds(timeToWait + 1);
        Debug.Log("Station tests Passed");
    }

    //public IEnumerator TestPlayerAndPlayerStationAssignedTrap(float timeToWait)
    //{
    //    Debug.Log("Testing TestPlayerAndPlayreStationAssignedTrap");
    //    if (m_playerSupplyItem == null)
    //    {
    //        m_playerSupplyItem = GameObject.Instantiate(m_playerSupplyItemPrefab).GetComponent<PlayerSupplyItem>();

    //    }
    //    Debug.Log("PlayerSupplyItem Created");



    //    Debug.Log("PlayerSupplyItem tests Passed");
    //}

    public IEnumerator TestPlayerSupplyItemInteractingWithSupplyStation(float timeToWait)
    {
        Debug.Log("Testing SupplyStation interaction");

        m_playerSupplyItem.transform.position = Vector3.zero;

        Assert.AreEqual(WorkshopGame.SupplyItemName.NOTHING, m_playerSupplyItem.m_supplyStationResourceName);
        m_playerSupplyItem.transform.position = m_station.transform.position;
        m_playerSupplyItem.m_mouseColliderFrozen = true;
        m_playerSupplyItem.m_mouseFollowingCollider.transform.position = m_station.transform.position;
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Testing station m_isUsable and interaction");
        Assert.True(m_station.m_isUsable);
        m_playerSupplyItem.ExecuteInteraction();
        yield return new WaitForSeconds(0.1f);
        Assert.False(m_station.m_isUsable);
        Assert.AreEqual(WorkshopGame.SupplyItemName.ROPE, m_playerSupplyItem.m_supplyStationResourceName);
        Debug.Log("PlayerSupplyItem tests Passed");
    }

    //public IEnumerator TestPlayerSupplyItemInteractingWithWorkstation(float timeToWait)
    //{
    //    Debug.Log("Testing SupplyStation interaction");
    //    if (m_playerSupplyItem == null)
    //    {
    //        m_playerSupplyItem = GameObject.Instantiate(m_playerSupplyItemPrefab).GetComponent<PlayerSupplyItem>();

    //    }
    //    m_playerSupplyItem.transform.position = m_station.transform.position;
    //    m_playerSupplyItem.m_mouseColliderFrozen = true;
    //    m_playerSupplyItem.m_mouseFollowingCollider.transform.position = m_station.transform.position;
    //    yield return new WaitForSeconds(0.1f);
    //    Debug.Log("PlayerSupplyItem Created");
    //    Debug.Log("Testing station m_isUsable and interaction");
    //    Assert.True(m.m_isUsable);
    //    m_playerSupplyItem.ExecuteInteraction();
    //    yield return new WaitForSeconds(0.1f);
    //    Assert.False(m_station.m_isUsable);
    //    Assert.AreEqual(WorkshopGame.SupplyStationName.ROPE, m_playerSupplyItem.m_supplyName);
    //    Debug.Log("PlayerSupplyItem tests Passed");
    //}
#endif
}
