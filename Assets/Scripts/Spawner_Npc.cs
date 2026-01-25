using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner_Npc : Spawner
{
    [SerializeField] public NpcTypeData.NpcTypes m_npcType { get; private set; } = NpcTypeData.NpcTypes.RAT;
    private void OnEnable()
    {
        m_spawnCoroutine = SpawnSequenceCoroutine<NpcWander>();
        StartCoroutine(m_spawnCoroutine);
    }

    public override void SpawnAndSendToServer()
    {
        NpcWander newNpc = SpawnNpc();
        if (newNpc != null)
        {
            SendNetworkDataObjectToServer(newNpc.m_networkDataObjectNpc);
            if (newNpc.HasItem())
            {
                newNpc.SetItemObjectValues();
                SendItemNetworkDataObjectToServer(newNpc.GetNpcItem().m_networkDataObjectItem);
            }
        }
    }


    public NpcWander SpawnNpc()
    {
        Debug.Log("Spawn attempt");

        if (m_spawnCount >= m_spawnLimit) return null;

        NpcWander newNpc = SpawnObject<NpcWander>();
        newNpc.SetNpcValues();
        newNpc.SetIndexOfSpawnerInGame(m_indexOfSpawnerInGame);
        newNpc.FillNetworkDataNpcObjectDelegates();
        if (newNpc.HasItem()) newNpc.SetItemObjectValues();
        newNpc.m_onDestroy = DestroyAndSendToServer;
        ++m_spawnCount;

        if (m_isGameOwner != null && m_isGameOwner())
        {
            newNpc.StartWandering();
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
        m_requestServerSpawnNpc(n);
    }

    public void SendItemNetworkDataObjectToServer(NetworkDataObject_Item n)
    {
        Debug.Log("SendItemRequest");
        m_requestServerRegisterItem(n);
    }

    public override void DestroyAndSendToServer(int npcId)
    {
        Debug.Log("SendDespawnRequest");
        --m_spawnCount;

        if (m_isGameOwner != null && m_isGameOwner())
        {
            if (m_spawnCount < m_spawnLimit && m_spawnCoroutine != null)
                StartCoroutine(m_spawnCoroutine);
        }
        m_removeNpcFromGameController(npcId);
    }

    public void ReceiveSpawnCommand(INetworkDataObject n)
    {

        //return SpawnObjectNetworkData<NpcWander>(n);
    }
}
