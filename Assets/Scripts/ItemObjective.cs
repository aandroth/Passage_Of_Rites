using System.Collections.Generic;
using UnityEngine;

public abstract class ItemObjective : Interactable
{
    public enum SupplyItemName { NOTHING, SLOT_FILLED, METAL, OIL, SPIKES, CARVING, ROCK, LOG, RATS, ROPE, SPRING }

    [SerializeField] protected static string spritePath = "Sprites/SupplyItems/";
    [SerializeField] private static Dictionary<SupplyItemName, Sprite> m_supplyItemNameToSpriteDict = null;
    [SerializeField] public static Sprite SpriteOfSupplyItem(SupplyItemName supplyItemName) { return m_supplyItemNameToSpriteDict[supplyItemName]; }

    [SerializeField] protected List<SpriteRenderer> m_suppliesNeededIcons = new List<SpriteRenderer>();
    [SerializeField] protected List<SupplyItemName> m_neededSupplyItems = new List<SupplyItemName>();
    [SerializeField] protected SupplyItemName m_supplyItemOnInteraction = SupplyItemName.NOTHING;
    [SerializeField] protected SupplyItemName m_supplyItemOnCompletion = SupplyItemName.NOTHING;
    [SerializeField] protected Sprite m_checkmarkSprite;


    protected virtual void Awake()
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


    // abstract method to check if the objective is met
    public virtual bool IsObjectiveMet()
    {
        return m_neededSupplyItems.Count == 0;
    }

    public override Vector3 GetCenterPoint()
    {
        return gameObject.transform.position;
    }
    public override bool PlayerCanInteract(SupplyItemName supplyHeld = SupplyItemName.NOTHING, List<SupplyItemName> suppliesNeeded = null)
    {
        return m_neededSupplyItems.Contains(supplyHeld);
    }
}
