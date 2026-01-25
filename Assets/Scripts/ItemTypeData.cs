using System.Collections.Generic;
using UnityEngine;

public class ItemTypeData : MonoBehaviour
{
    public enum ITEM_TYPES { PICKUP, OWNED };
    public enum ITEM_OWNER_TYPE { SELF, PLAYER, NPC };
    public static Dictionary<ITEM_TYPES, GameObject> m_itemTypeToPrefab = null;
    public enum ITEM_STATE { NONE, COMPLETED, DESTROYED }

    private static string m_resourcePath = "Prefabs/";

    public static void LoadResourcesIntoTypePrefabDict()
    {
        m_itemTypeToPrefab = new Dictionary<ITEM_TYPES, GameObject>();
        //m_itemTypeToPrefab[ItemTypes.RAT] = (GameObject)Resources.Load($"{m_resourcePath}Rat");
    }
}
