using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner_Npc : Spawner
{
    [SerializeField] NpcTypeData.NpcTypes m_npcType = NpcTypeData.NpcTypes.RAT;
    private void OnEnable()
    {
        m_spawnCoroutine = SpawnSequenceCoroutine<NpcWander>();
        StartCoroutine(m_spawnCoroutine);
    }

    public override void SpawnAndSendToServer()
    {
        NpcWander newNpc = SpawnNpc();
        if (newNpc != null)
            SendNetworkDataObjectToServer(newNpc.m_networkDataObjectNpc);
    }


    public NpcWander SpawnNpc()
    {
        Debug.Log("Spawn attempt");

        if (m_spawnCount >= m_spawnLimit) return null;

        NpcWander newNpc = SpawnObject<NpcWander>();
        newNpc.FillNetworkDataObjectDelegates();
        newNpc.SetIdSpawnerIdAndNpcType(newNpc.gameObject.GetInstanceID(), m_index, (int)m_npcType);
        newNpc.m_onDestroyDelegate = DespawnAndSendToServer;
        ++m_spawnCount;

        if (m_isServerOwner != null && m_isServerOwner())
        {
            if (m_spawnCount >= m_spawnLimit && m_spawnCoroutine != null)
            {
                StopCoroutine(m_spawnCoroutine);
            }
        }

        return newNpc;
    }

    public void SendNetworkDataObjectToServer(NetworkDataObject_Npc n)
    {
        Debug.Log("SendSpawnRequest");
        m_requestServerSpawn(n);
    }

    public override void DespawnAndSendToServer(int npcId)
    {
        Debug.Log("SendDespawnRequest");
        --m_spawnCount;

        if (m_isServerOwner != null && m_isServerOwner())
        {
            if (m_spawnCount < m_spawnLimit && m_spawnCoroutine != null)
                StartCoroutine(m_spawnCoroutine);
        }

        m_requestServerDespawn(npcId);
    }

    public void ReceiveSpawnCommand(INetworkDataObject n)
    {

        //return SpawnObjectNetworkData<NpcWander>(n);
    }
}
