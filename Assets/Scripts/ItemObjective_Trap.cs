using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemObjectiveData;

public class ItemObjective_Trap : ItemObjective
{
    public enum TrapType { NOTHING, FALLING_RATS, SWINGING_BLADE, SWINGING_SPIKE_LOG, SLIDE_INTO_SPIKES, RAT_ON_A_STICK, BURNING_OIL }
    public static Dictionary<TrapType, SupplyItemName[]> m_trapTypeToSupplyItemDict = new Dictionary<TrapType, SupplyItemName[]> {
        {TrapType.FALLING_RATS,         new SupplyItemName[] { SupplyItemName.SPRING,  SupplyItemName.RATS,    SupplyItemName.ROCK}},
        {TrapType.SWINGING_BLADE,       new SupplyItemName[] { SupplyItemName.CARVING, SupplyItemName.METAL,   SupplyItemName.ROPE}},
        {TrapType.SWINGING_SPIKE_LOG,   new SupplyItemName[] { SupplyItemName.LOG,     SupplyItemName.SPIKES,  SupplyItemName.ROPE}},
        {TrapType.SLIDE_INTO_SPIKES,    new SupplyItemName[] { SupplyItemName.SPIKES,  SupplyItemName.OIL,     SupplyItemName.METAL}},
        {TrapType.RAT_ON_A_STICK,       new SupplyItemName[] { SupplyItemName.RATS,    SupplyItemName.CARVING, SupplyItemName.LOG}},
        {TrapType.BURNING_OIL,          new SupplyItemName[] { SupplyItemName.OIL,     SupplyItemName.ROCK,    SupplyItemName.SPRING}}
    };

    public static Dictionary<TrapType, string> m_trapTypeToNameDict = new Dictionary<TrapType, string> {
        {TrapType.FALLING_RATS,         "Falling Rats"},
        {TrapType.SWINGING_BLADE,       "Swinging Blade"},
        {TrapType.SWINGING_SPIKE_LOG,   "Swinging Spike Log"},
        {TrapType.SLIDE_INTO_SPIKES,    "Slide to Spikes"},
        {TrapType.RAT_ON_A_STICK,       "Rat on a Stick"},
        {TrapType.BURNING_OIL,          "Burning Oil"}
    };

    private static string spritePath_Traps = "Sprites/Traps/";
    public static Dictionary<TrapType, Sprite> m_trapTypeToSpriteDict = null;

    public GameObject m_finishedTrapObject;
    public TMPro.TextMeshPro m_finishedTrapNameTMP;
    public SpriteRenderer m_finishedTrapSpriteRenderer = new SpriteRenderer();
    public float m_showCompletedTrapTime = 3f;
    public TrapType m_trapType;
    private List<TrapType> m_trapsToComplete = new List<TrapType>(m_trapTypeToSupplyItemDict.Keys);
    [SerializeField] int m_suppliesGatheredCount = 0;


    public delegate void ReportTrapCompleted();
    public ReportTrapCompleted m_reportTrapCompleted;
    public delegate void RequestAssignNewTrap();
    public RequestAssignNewTrap m_requestAssignNewTrap;
    public delegate void ReportSupplyCheckedOff();
    public ReportTrapCompleted m_reportSupplyCheckedOff;

    public void Start()
    {
        if (m_trapTypeToSpriteDict == null)
        {
            m_trapTypeToSpriteDict = new Dictionary<TrapType, Sprite> {
            {TrapType.NOTHING,             Resources.Load<Sprite>($"{spritePath_Traps}Blank")},
            {TrapType.FALLING_RATS,        Resources.Load<Sprite>($"{spritePath_Traps}FallingRats")},
            {TrapType.SWINGING_BLADE,      Resources.Load<Sprite>($"{spritePath_Traps}SwingingBlade")},
            {TrapType.SWINGING_SPIKE_LOG,  Resources.Load<Sprite>($"{spritePath_Traps}SwingingSpikeLog")},
            {TrapType.SLIDE_INTO_SPIKES,   Resources.Load<Sprite>($"{spritePath_Traps}OiledSpikeTrap")},
            {TrapType.RAT_ON_A_STICK,      Resources.Load<Sprite>($"{spritePath_Traps}RatFlail")},
            {TrapType.BURNING_OIL,         Resources.Load<Sprite>($"{spritePath_Traps}BurningOil")}
            };
        }
        Debug.Log($"m_finishedTrapSpriteRenderer: {m_finishedTrapSpriteRenderer.gameObject}");
    }

    public override SupplyItemName Interact(SupplyItemName supplyHeld = SupplyItemName.NOTHING, List<SupplyItemName> suppliesNeeded = null)
    {
        CheckOffSupply(supplyHeld);
        ++m_suppliesGatheredCount;
        if (IsObjectiveMet())
        {
            StartCoroutine(CompleteTrapCoroutine());
            return m_supplyItemOnCompletion;
        }
        return m_supplyItemOnInteraction;
    }
    public override bool IsObjectiveMet()
    {
        return m_suppliesGatheredCount == m_suppliesNeededIcons.Count;
    }
    public void CheckOffSupply(SupplyItemName supplyFromPlayer)
    {
        ++m_suppliesGatheredCount;
        int index = m_neededSupplyItems.IndexOf(supplyFromPlayer);
        m_suppliesNeededIcons[index].sprite = m_checkmarkSprite;
        m_neededSupplyItems[index] = SupplyItemName.SLOT_FILLED;
    }
    public TrapType GetNextTrap()
    {
        if (m_trapsToComplete.Count == 0)
            m_trapsToComplete = new List<TrapType>(m_trapTypeToSupplyItemDict.Keys);

        return m_trapsToComplete[(int)(UnityEngine.Random.value * m_trapsToComplete.Count)];
    }
    public TrapType AssignTrapToComplete()
    {
        TrapType trapType = GetNextTrap();
        m_neededSupplyItems = new List<SupplyItemName>(m_trapTypeToSupplyItemDict[trapType]);

        for (int i = 0; i < m_suppliesNeededIcons.Count; i++)
            m_suppliesNeededIcons[i].sprite = SpriteOfSupplyItem(m_neededSupplyItems[i]);

        m_finishedTrapSpriteRenderer.sprite = m_trapTypeToSpriteDict[trapType];
        m_finishedTrapNameTMP.text = m_trapTypeToNameDict[trapType];

        return trapType;
    }
    public void TrapCompleted(TrapType trapType)
    {
        m_reportTrapCompleted();
        m_trapsToComplete.Remove(trapType);
        if (m_trapsToComplete.Count == 0)
        {
            foreach (var name in m_trapTypeToSupplyItemDict.Keys)
                m_trapsToComplete.Add(name);
        }
    }
    public IEnumerator CompleteTrapCoroutine()
    {
        m_reportTrapCompleted();
        m_finishedTrapObject.SetActive(true);
        float timeToShowCompletedTrapCountdown = m_showCompletedTrapTime;
        while (timeToShowCompletedTrapCountdown > 0)
        {
            timeToShowCompletedTrapCountdown -= Time.deltaTime;
            yield return null;
        }

        m_finishedTrapObject.SetActive(false);
        m_suppliesGatheredCount = 0;

        m_requestAssignNewTrap();
    }
}
