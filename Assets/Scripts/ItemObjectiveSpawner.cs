using System.Collections;
using UnityEngine;

public class ItemObjectiveSpawner : MonoBehaviour
{
    [SerializeField] float m_spawnRadius = 2f;
    [SerializeField] ItemObjective m_itemObjectivePrefab;
    [SerializeField] float m_waitTimeMin, m_waitTimeMax;
    [SerializeField] float m_spawnTimeAfterBushShake;

    public void OnEnable()
    {
        StartCoroutine(SpawnSequenceCoroutine());
    }

    private IEnumerator SpawnSequenceCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(m_waitTimeMin, m_waitTimeMax) - m_spawnTimeAfterBushShake); // Adjust spawn interval as needed
            // Bush shake animation/effect can be triggered here
            yield return new WaitForSeconds(m_spawnTimeAfterBushShake); // Adjust spawn interval as needed
            SpawnItemObjective();
        }
    }

    public ItemObjective SpawnItemObjective()
    {
        Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle.normalized * m_spawnRadius; // Spawns a rat at the edge of the spawn radius
        ItemObjective newItemObjective = Instantiate(m_itemObjectivePrefab, spawnPosition, Quaternion.identity);
        return newItemObjective;
    }
    void OnDrawGizmosSelected()
    {
        // Set the color of the gizmo
        Gizmos.color = Color.green; // You can choose any color

        // Draw a wire sphere at the object's position with the specified radius
        // This visualizes the detection area in the Scene view
        Gizmos.DrawWireSphere(transform.position, m_spawnRadius);
    }
}
