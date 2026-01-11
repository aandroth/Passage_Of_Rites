using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner_Npc : Spawner
{
    private void OnEnable()
    {
        m_spawnCoroutine = SpawnSequenceCoroutine<NpcWander>();
        StartCoroutine(m_spawnCoroutine);
    }

    public override void SpawnAndSendToServer()
    {
        SendNetworkDataObjectToServer(SpawnNpc().m_networkDataObjectNpc);
    }

    public NpcWander SpawnNpc()
    {
        if (m_spawnCount >= m_spawnLimit) return null;

        Debug.Log("SendSpawnRequest");
        NpcWander newNpc = SpawnObject<NpcWander>();
        newNpc.FillNetworkDataObjectDelegates();
        newNpc.m_onDestroyDelegate = DespawnAndSendToServer;

        if (m_isServerOwner != null && m_isServerOwner())
        {
            if (m_spawnCount >= m_spawnLimit && m_spawnCoroutine != null)
                StopCoroutine(m_spawnCoroutine);
        }
        ++m_spawnCount;

        return newNpc;
    }

    public void SendNetworkDataObjectToServer(NetworkDataObject_Npc n)
    {
        m_requestServerSpawn(n);
    }

    public override void DespawnAndSendToServer(int npcId)
    {
        Debug.Log("SendDespawnRequest");
        --m_spawnCount;

        if (m_isServerOwner != null && m_isServerOwner())
        {
            if (m_spawnCount < m_spawnLimit && m_spawnCoroutine != null)
                StopCoroutine(m_spawnCoroutine);
        }

        m_requestServerDespawn(npcId);
    }

    //public NpcWander ReceiveSpawnCommand(INetworkDataObject n)
    //{
    //    return SpawnObjectNetworkData<NpcWander>(n);
    //}
}
