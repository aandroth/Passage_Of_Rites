using UnityEngine;

public class NpcGetRandomColor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer = null;


    void Start()
    {
        m_spriteRenderer.color = new Color(Random.Range(0, 100) * 0.01f, Random.Range(0, 100) * 0.01f, Random.Range(0, 100) * 0.01f, 1);
    }
}
