using System.Collections.Generic;
using UnityEngine;

public static class NpcTypeData
{
    public enum NpcTypes { RAT, SLIME };
    public static Dictionary<NpcTypes, GameObject> m_npcTypeToPrefab = new Dictionary<NpcTypes, GameObject>();

    private static string m_resourcePath = "Prefabs/";

    public static void Start()
    {
        m_npcTypeToPrefab[NpcTypes.RAT] = (GameObject)Resources.Load($"{m_resourcePath}Rat");
    }
}
