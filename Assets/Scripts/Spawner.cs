using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using static ItemObjectiveData;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField] public int m_indexOfSpawnerInGame = 0;
    [SerializeField] Transform m_SpawnCenterLocation;
    [SerializeField] float m_spawnRadius = 2f;
    [SerializeField] GameObject m_spawnPrefab;
    [SerializeField] float m_waitTimeMin = 2, m_waitTimeMax = 4;
    [SerializeField] float m_delaySpawnTime = 1;
    [SerializeField] protected int m_spawnLimit = 10, m_spawnCount = 0;
    protected IEnumerator m_spawnCoroutine;

    public delegate void RequestServerSpawnNpcDelegate(NetworkDataObject_Npc dataObj);
    public RequestServerSpawnNpcDelegate m_requestServerSpawnNpc;
    public delegate void RemoveNpcFromGameControllerDelegate(int npcId);
    public RemoveNpcFromGameControllerDelegate m_removeNpcFromGameController;
    public delegate void NpcDestroyedDelegate(int npcId);
    public NpcDestroyedDelegate m_npcDestroyed;

    public delegate void RequestServerRegisterItemDelegate(NetworkDataObject_Item dataObj);
    public RequestServerRegisterItemDelegate m_requestServerRegisterItem;
    public delegate void DeregisterItemFromGameControllerDelegate(int itemId);
    public DeregisterItemFromGameControllerDelegate m_deregisterItemFromGameController;
    public delegate void ItemDestroyedDelegate(int itemId);
    public ItemDestroyedDelegate m_itemDestroyed;

    public delegate bool SpawnCommandFromServerDelegate();
    public SpawnCommandFromServerDelegate m_spawnCommandFromServer;
    public delegate bool IsGameOwnerDelegate();
    public IsGameOwnerDelegate m_isGameOwner;
    public delegate void ReportItemCompletedDelegate();
    public ReportItemCompletedDelegate m_reportItemCompleted;

    public void Start()
    {
    }

    public IEnumerator SpawnSequenceCoroutine<T>()
    {
        while (true)
        {
            Debug.Log($"SpawnSequenceCoroutine running");
            yield return new WaitForSeconds(Random.Range(m_waitTimeMin, m_waitTimeMax) - m_delaySpawnTime); // Adjust spawn interval as needed
            // Spawner animation/effect can be triggered here
            yield return new WaitForSeconds(m_delaySpawnTime); // Adjust spawn interval as needed
            SpawnAndSendToServer();
        }
    }

    public abstract void SpawnAndSendToServer();
    public abstract void DestroyAndSendToServer(int id);

    public T SpawnObject<T>()
    {
        Vector2 spawnPosition = (Vector2)m_SpawnCenterLocation.position + Random.insideUnitCircle.normalized * m_spawnRadius; // Spawns at the edge of the spawn radius
        T newObject = Instantiate(m_spawnPrefab, spawnPosition, Quaternion.identity).GetComponent<T>();
        return newObject;
    }

    public static ItemObjective SpawnItemObjectiveFromData(string[] data)
    {
        Vector2 spawnPosition = new Vector2(float.Parse(data[5]), float.Parse(data[6]));
        ItemObjective newObject = Instantiate(m_supplyItemNameToPrefab[(SupplyItemName)int.Parse(data[3])], spawnPosition, Quaternion.identity).GetComponent<ItemObjective>();
        newObject.FillNetworkDataItemObjectDelegates();
        newObject.m_networkDataObjectItem.PutAllData(data);
        return newObject;
    }
}
