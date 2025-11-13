using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WorkshopGame : Game
{
    public delegate void SetPlayerLocation();
    public enum TrapType { NOTHING, FALLING_RATS, SWINGING_BLADE, SWINGING_SPIKE_LOG, SLIDE_INTO_SPIKES, RAT_ON_A_STICK, BURNING_OIL }
    public enum SupplyItemName { NOTHING, METAL, OIL, SPIKES, CARVING, ROCK, LOG, RATS, ROPE, SPRING }
    public SupplyItemName m_supplyItemNames = SupplyItemName.METAL;
    public List<WorkshopSupplyStation> m_supplyStations = new List<WorkshopSupplyStation>();
    public List<GameObject> m_supplyStationObjects;
    public List<PlayerStation> m_playerStations;
    public int m_playerStationIdx = 0;
    public PlayerSupplyItem m_playerSupplyItem = null;
    public int m_playerScore = 0;

    public PlayerStation m_playerStation;
    public List<Transform> m_playerSpawnLocations;
    public static List<Transform> m_playerTableLocations;

    public static Dictionary<TrapType, SupplyItemName[]> m_trapToSuppliesDict = new Dictionary<TrapType, SupplyItemName[]>();
    public static Dictionary<TrapType, string> m_trapToNameDict = new Dictionary<TrapType, string>();
    public static Dictionary<WorkshopGame.TrapType, Sprite> m_trapToSpriteDict = new Dictionary<TrapType, Sprite>();
    public static Dictionary<WorkshopGame.SupplyItemName, Sprite> m_nameToSpriteDict = new Dictionary<SupplyItemName, Sprite>();
    public List<TrapType> m_trapsToComplete = new List<TrapType>();
    public List<Sprite> m_trapSprites = new List<Sprite>();
    public List<Sprite> m_supplySprites = new List<Sprite>();

    private void Start()
    {
        m_trapToSuppliesDict[WorkshopGame.TrapType.FALLING_RATS]         = new SupplyItemName[] { SupplyItemName.SPRING, SupplyItemName.RATS, SupplyItemName.ROCK};
        m_trapToSuppliesDict[WorkshopGame.TrapType.SWINGING_BLADE]       = new SupplyItemName[] { SupplyItemName.CARVING, SupplyItemName.METAL, SupplyItemName.ROPE};
        m_trapToSuppliesDict[WorkshopGame.TrapType.SWINGING_SPIKE_LOG]   = new SupplyItemName[] { SupplyItemName.LOG, SupplyItemName.SPIKES, SupplyItemName.ROPE};
        m_trapToSuppliesDict[WorkshopGame.TrapType.SLIDE_INTO_SPIKES]    = new SupplyItemName[] { SupplyItemName.SPIKES, SupplyItemName.OIL, SupplyItemName.METAL};
        m_trapToSuppliesDict[WorkshopGame.TrapType.RAT_ON_A_STICK]       = new SupplyItemName[] { SupplyItemName.RATS, SupplyItemName.CARVING, SupplyItemName.LOG};
        m_trapToSuppliesDict[WorkshopGame.TrapType.BURNING_OIL]          = new SupplyItemName[] { SupplyItemName.OIL, SupplyItemName.ROCK, SupplyItemName.SPRING};

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


        m_nameToSpriteDict[WorkshopGame.SupplyItemName.METAL]   = m_supplySprites[0];
        m_nameToSpriteDict[WorkshopGame.SupplyItemName.OIL]     = m_supplySprites[1];
        m_nameToSpriteDict[WorkshopGame.SupplyItemName.SPIKES]  = m_supplySprites[2];
        m_nameToSpriteDict[WorkshopGame.SupplyItemName.CARVING] = m_supplySprites[3];
        m_nameToSpriteDict[WorkshopGame.SupplyItemName.ROCK]    = m_supplySprites[4];
        m_nameToSpriteDict[WorkshopGame.SupplyItemName.LOG]     = m_supplySprites[5];
        m_nameToSpriteDict[WorkshopGame.SupplyItemName.RATS]    = m_supplySprites[6];
        m_nameToSpriteDict[WorkshopGame.SupplyItemName.ROPE]    = m_supplySprites[7];
        m_nameToSpriteDict[WorkshopGame.SupplyItemName.SPRING]  = m_supplySprites[8];

        foreach (var name in m_trapToSuppliesDict.Keys)
            m_trapsToComplete.Add(name);

        m_playerStation.m_reportTrapCompleted = TrapCompleted;
        AssignTrapToPlayerAndStation();
    }

    public static Vector3 GetSpawnLocationForId(int id)
    {
        //if(id < 0 || id > m_playerSpawnLocations.Count)
            return Vector3.zero;

        //return m_playerSpawnLocations[id].position;
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
        else
        {
            foreach (var name in m_trapToSuppliesDict.Keys)
                m_trapsToComplete.Add(name);
            AssignTrapToPlayerAndStation();
        }
    }
}
