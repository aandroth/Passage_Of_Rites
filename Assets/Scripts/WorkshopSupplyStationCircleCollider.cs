using NUnit.Framework;
using UnityEngine;

public class WorkshopSupplyStationCircleCollider : MonoBehaviour
{
    public delegate void OnPlayerEnterDelegate(PlayerSupplyItem playerSupplyItem);
    public OnPlayerEnterDelegate m_onPlayerEnterDelegate;
    public delegate void OnPlayerExitDelegate();
    public OnPlayerExitDelegate m_onPlayerExitDelegate;
    public delegate void OnMouseEnterDelegate();
    public OnMouseEnterDelegate m_onMouseEnterDelegate;
    public delegate void OnMouseExitDelegate();
    public OnMouseExitDelegate m_onMouseExitDelegate;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerSupplyItem playerSupplyItem = collision.gameObject.GetComponent<PlayerSupplyItem>();
        if (playerSupplyItem)
        {
            m_onPlayerEnterDelegate?.Invoke(playerSupplyItem);
        }
        else if(collision.gameObject.GetComponent<MouseFollowingCollider>())
        {
            m_onMouseEnterDelegate.Invoke();
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        PlayerSupplyItem playerSupplyItem = collision.gameObject.GetComponent<PlayerSupplyItem>();
        if (playerSupplyItem)
        {
            m_onPlayerExitDelegate?.Invoke();
        }
        else if (collision.gameObject.GetComponent<MouseFollowingCollider>())
        {
            m_onMouseExitDelegate.Invoke();
        }
    }
}
