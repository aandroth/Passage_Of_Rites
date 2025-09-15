using UnityEngine;

public class WorkshopSupplyStationCircleCollider : MonoBehaviour
{
    public delegate void OnTriggerEnterDelegate();
    public OnTriggerEnterDelegate m_onTriggerEnterDelegate;
    public delegate void OnTriggerExitDelegate();
    public OnTriggerExitDelegate m_onTriggerExitDelegate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger");
        m_onTriggerEnterDelegate?.Invoke();
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        m_onTriggerExitDelegate?.Invoke();
    }
}
