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
        newNpc.m_onDestroy = DestroyAndSendToServer;
        ++m_spawnCount;

        if (m_isGameOwner != null && m_isGameOwner())
        {
            if (m_spawnCount >= m_spawnLimit && m_spawnCoroutine != null)
            {
                StopCoroutine(m_spawnCoroutine);
            }
        }

        return newNpc;
    }


    public void SetNpcItemDelegates(NpcWander npc)
    {
        if(npc.m_hasItem)
        {
            //ItemObjective_Reuse item = ItemObjectiveData_Reuse.Instance.GetItemObjectiveByNpcType(npc.m_npcType);
            //if (item != null)
            //{
            //    npc.m_npcItem = item;
            //    npc.m_npcItem.m_onPickedUp = npc.StopWandering;
            //    npc.m_npcItem.m_onDropped = npc.StartWandering;
            //}
        }
    }

    public void SendNetworkDataObjectToServer(NetworkDataObject_Npc n)
    {
        Debug.Log("SendSpawnRequest");
        m_requestServerSpawn(n);
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

        m_removeFromGameController(npcId);
    }

    public void ReceiveSpawnCommand(INetworkDataObject n)
    {

        //return SpawnObjectNetworkData<NpcWander>(n);
    }
}
