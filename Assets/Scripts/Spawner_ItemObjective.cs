using System.Collections;
using UnityEngine;

public class Spawner_ItemObjective : Spawner
{
    public void OnEnable()
    {
        StartCoroutine(SpawnSequenceCoroutine<ItemObjective>());
    }

    public override void SpawnAndSendToServer()
    {
        //m_requestServerSpawn();
    }

    public override void DestroyAndSendToServer(int id)
    {
        //m_requestServerSpawn();
    }

    //public ItemObjective ReceiveSpawnCommand()
    //{
    //    return SpawnObjectNetworkData<ItemObjective>();
    //}
}
