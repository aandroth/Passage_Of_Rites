using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WorkshopGame : MonoBehaviour
{
    public enum TrapType { NOTHING, FALLING_RATS, SWINGING_BLADE, SWINGING_SPIKE_LOG, SLIDE_INTO_SPIKES, RAT_ON_A_STICK, BURNING_OIL }
    public enum SupplyStationName { NOTHING, METAL, OIL, SPIKES, CARVING, ROCK, LOG, RATS, ROPE, SPRING }
    public SupplyStationName m_supplyStationNames = SupplyStationName.METAL;
    public List<WorkshopSupplyStation> m_supplyStations = new List<WorkshopSupplyStation>();
    public List<GameObject> m_supplyStationObjects;
    public List<PlayerStation> m_playerStations;
    public int m_playerStationIdx = 0;
    public PlayerSupplyItem m_playerSupplyItem = null;
    public int m_playerScore = 0;

    public PlayerStation m_playerStation;

    public static Dictionary<TrapType, SupplyStationName[]> m_trapToSuppliesDict = new Dictionary<TrapType, SupplyStationName[]>();
    public static Dictionary<TrapType, string> m_trapToNameDict = new Dictionary<TrapType, string>();
    public static Dictionary<WorkshopGame.TrapType, Sprite> m_trapToSpriteDict = new Dictionary<TrapType, Sprite>();
    public static Dictionary<WorkshopGame.SupplyStationName, Sprite> m_nameToSpriteDict = new Dictionary<SupplyStationName, Sprite>();
    public List<TrapType> m_trapsToComplete = new List<TrapType>();
    public List<Sprite> m_trapSprites = new List<Sprite>();
    public List<Sprite> m_supplySprites = new List<Sprite>();

    private void Start()
    {
        m_trapToSuppliesDict[WorkshopGame.TrapType.FALLING_RATS]         = new SupplyStationName[] { SupplyStationName.SPRING, SupplyStationName.RATS, SupplyStationName.ROCK};
        m_trapToSuppliesDict[WorkshopGame.TrapType.SWINGING_BLADE]       = new SupplyStationName[] { SupplyStationName.CARVING, SupplyStationName.METAL, SupplyStationName.ROPE};
        m_trapToSuppliesDict[WorkshopGame.TrapType.SWINGING_SPIKE_LOG]   = new SupplyStationName[] { SupplyStationName.LOG, SupplyStationName.SPIKES, SupplyStationName.ROPE};
        m_trapToSuppliesDict[WorkshopGame.TrapType.SLIDE_INTO_SPIKES]    = new SupplyStationName[] { SupplyStationName.SPIKES, SupplyStationName.OIL, SupplyStationName.METAL};
        m_trapToSuppliesDict[WorkshopGame.TrapType.RAT_ON_A_STICK]       = new SupplyStationName[] { SupplyStationName.RATS, SupplyStationName.CARVING, SupplyStationName.LOG};
        m_trapToSuppliesDict[WorkshopGame.TrapType.BURNING_OIL]          = new SupplyStationName[] { SupplyStationName.OIL, SupplyStationName.ROCK, SupplyStationName.SPRING};

        m_trapToNameDict[WorkshopGame.TrapType.FALLING_RATS]         = "Falling Rats";
        m_trapToNameDict[WorkshopGame.TrapType.SWINGING_BLADE]       = "Swinging Blade";
        m_trapToNameDict[WorkshopGame.TrapType.SWINGING_SPIKE_LOG]   = "Swinging Spike Log";
        m_trapToNameDict[WorkshopGame.TrapType.SLIDE_INTO_SPIKES]    = "Slide to Spikes";
        m_trapToNameDict[WorkshopGame.TrapType.RAT_ON_A_STICK]       = "Rat on a Stick";
        m_trapToNameDict[WorkshopGame.TrapType.BURNING_OIL]          = "Burning Oil";

        m_trapToSpriteDict[WorkshopGame.TrapType.FALLING_RATS]         = m_trapSprites[0];
        m_trapToSpriteDict[WorkshopGame.TrapType.SWINGING_BLADE]       = m_trapSprites[1];
        m_trapToSpriteDict[WorkshopGame.TrapType.SWINGING_SPIKE_LOG]   = m_trapSprites[2];
        m_trapToSpriteDict[WorkshopGame.TrapType.SLIDE_INTO_SPIKES]    = m_trapSprites[3];
        m_trapToSpriteDict[WorkshopGame.TrapType.RAT_ON_A_STICK]       = m_trapSprites[4];
        m_trapToSpriteDict[WorkshopGame.TrapType.BURNING_OIL]          = m_trapSprites[5];


        m_nameToSpriteDict[WorkshopGame.SupplyStationName.METAL]   = m_supplySprites[0];
        m_nameToSpriteDict[WorkshopGame.SupplyStationName.OIL]     = m_supplySprites[1];
        m_nameToSpriteDict[WorkshopGame.SupplyStationName.SPIKES]  = m_supplySprites[2];
        m_nameToSpriteDict[WorkshopGame.SupplyStationName.CARVING] = m_supplySprites[3];
        m_nameToSpriteDict[WorkshopGame.SupplyStationName.ROCK]    = m_supplySprites[4];
        m_nameToSpriteDict[WorkshopGame.SupplyStationName.LOG]     = m_supplySprites[5];
        m_nameToSpriteDict[WorkshopGame.SupplyStationName.RATS]    = m_supplySprites[6];
        m_nameToSpriteDict[WorkshopGame.SupplyStationName.ROPE]    = m_supplySprites[7];
        m_nameToSpriteDict[WorkshopGame.SupplyStationName.SPRING]  = m_supplySprites[8];

        foreach (var name in m_trapToSuppliesDict.Keys)
            m_trapsToComplete.Add(name);

        m_playerStation.m_reportTrapCompleted = TrapCompleted;
        AssignTrapToPlayerAndStation();
    }


    void Update()
    {
        //if (Input.GetMouseButtonUp(0) && m_supplyStations.Count > 0)
        //{
        //    Vector3 screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    screenPoint.z = 0;
        //    for(int i=0; i< m_playerStations.Count; ++i)
        //    {
        //        m_playerStations[i].m_reportSupplyCheckedOff = SupplyCheckedOff;
        //        m_playerStations[i].m_reportTrapCompleted = TrapCompleted;
        //    }
        //    for(int i=0; i< m_supplyStations.Count; ++i)
        //    {
        //        if (Vector3.Distance(m_supplyStations[i].m_centerPoint.transform.position, screenPoint) < m_supplyStations[i].m_minMouseDistanceToCenter &&
        //            m_supplyStations[i].m_playerInRange &&
        //            m_supplyStations[i].SupplyNeededByPlayer(m_playerStation.m_neededSupplyItems))
        //        {
        //            var supplyImageAndName = SupplyStationUsed(i);
        //            m_playerSupplyItem.ActivateAndSetSupplyItem(supplyImageAndName.Item1,
        //                                                        supplyImageAndName.Item2);
        //        }
        //    }
        //}
    }

    public void AssignTrapToPlayerAndStation()
    {
        TrapType t = m_trapsToComplete[(int)(Random.value * m_trapsToComplete.Count)];
        m_playerSupplyItem.AssignTrapToComplete(t);
        m_playerStation.AssignTrapToComplete(t);
    }

    public void TrapCompleted(TrapType trapType)
    {
        m_playerScore++;
        m_trapsToComplete.Remove(trapType);

        if (m_trapsToComplete.Count > 0)
            AssignTrapToPlayerAndStation();
    }
}
