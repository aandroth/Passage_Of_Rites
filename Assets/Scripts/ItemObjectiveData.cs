using System.Collections.Generic;
using UnityEngine;

public static class ItemObjectiveData
{
    public enum SupplyItemName { NOTHING, SLOT_FILLED, METAL, OIL, SPIKES, CARVING, ROCK, LOG, RATS, ROPE, SPRING }
    [SerializeField] static string spritePath = "Sprites/SupplyItems/";
    [SerializeField] static Dictionary<SupplyItemName, Sprite> m_supplyItemNameToSpriteDict = null;
    [SerializeField] public static Sprite m_checkmarkSprite;

    public static void FillValues()
    {
        m_supplyItemNameToSpriteDict = new Dictionary<SupplyItemName, Sprite> {
            {SupplyItemName.NOTHING,     Resources.Load<Sprite>($"{spritePath}Blank")},
            {SupplyItemName.SLOT_FILLED, Resources.Load<Sprite>($"{spritePath}Blank")},
            {SupplyItemName.METAL,       Resources.Load<Sprite>($"{spritePath}MetalBar")},
            {SupplyItemName.OIL,         Resources.Load<Sprite>($"{spritePath}Oil")},
            {SupplyItemName.SPIKES,      Resources.Load<Sprite>($"{spritePath}Spikes")},
            {SupplyItemName.CARVING,     Resources.Load<Sprite>($"{spritePath}Carving")},
            {SupplyItemName.ROCK,        Resources.Load<Sprite>($"{spritePath}Rock")},
            {SupplyItemName.LOG,         Resources.Load<Sprite>($"{spritePath}Log")},
            {SupplyItemName.RATS,        Resources.Load<Sprite>($"{spritePath}Rat")},
            {SupplyItemName.ROPE,        Resources.Load<Sprite>($"{spritePath}Rope")},
            {SupplyItemName.SPRING,      Resources.Load<Sprite>($"{spritePath}Spring")}
        };
        m_checkmarkSprite = Resources.Load<Sprite>($"{spritePath}Checkmark");
    }

    public static Sprite SpriteOfSupplyItem(SupplyItemName supplyItemName)
    {
        if (m_supplyItemNameToSpriteDict == null)
            FillValues();

        return m_supplyItemNameToSpriteDict[supplyItemName];
    }
}
