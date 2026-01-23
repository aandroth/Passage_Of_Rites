using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField] public int m_index = 0;
    [SerializeField] Transform m_SpawnCenterLocation;
    [SerializeField] float m_spawnRadius = 2f;
    [SerializeField] GameObject m_spawnPrefab;
    [SerializeField] float m_waitTimeMin = 2, m_waitTimeMax = 4;
    [SerializeField] float m_delaySpawnTime = 1;
    [SerializeField] protected int m_spawnLimit = 10, m_spawnCount = 0;
    protected IEnumerator m_spawnCoroutine;

    public delegate void RequestServerSpawnDelegate(NetworkDataObject_Npc dataObj);
    public RequestServerSpawnDelegate m_requestServerSpawn;
    public delegate void RemoveFromGameControllerDelegate(int npcId);
    public RemoveFromGameControllerDelegate m_removeFromGameController;
    public delegate void NpcDestroyedDelegate(int npcId);
    public NpcDestroyedDelegate m_npcDestroyed;
    public delegate bool SpawnCommandFromServerDelegate();
    public SpawnCommandFromServerDelegate m_spawnCommandFromServer;
    public delegate bool IsGameOwnerDelegate();
    public IsGameOwnerDelegate m_isGameOwner;
    public delegate void ReportItemCompletedDelegate();
    public ReportItemCompletedDelegate m_reportItemCompleted;

    public void Start()
    {
        if(NpcTypeData.m_npcTypeToPrefab == null)
        {
            NpcTypeData.LoadResourcesIntoTypePrefabDict();
        }
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
}
