using System.Collections.Generic;
using UnityEngine;

public static class NpcTypeData
{
    public enum NpcTypes { RAT, SLIME };
    public static readonly Dictionary<NpcTypes, bool> NpcTypeHasItem = new Dictionary<NpcTypes, bool> { {NpcTypes.RAT, true}, {NpcTypes.SLIME, false} };
    public static Dictionary<NpcTypes, GameObject> m_npcTypeToPrefab = null;

    private static string m_resourcePath = "Prefabs/";

    public static void LoadResourcesIntoTypePrefabDict()
    {
        m_npcTypeToPrefab = new Dictionary<NpcTypes, GameObject>();
        m_npcTypeToPrefab[NpcTypes.RAT] = (GameObject)Resources.Load($"{m_resourcePath}Rat");
    }
}
