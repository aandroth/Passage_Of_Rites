using System.Collections.Generic;
using UnityEngine;

public class MouseFollowingCollider : MonoBehaviour
{
    public delegate void ColliderEnterCallback(Interactable i);
    public ColliderEnterCallback ColliderEnter;
    public delegate void ColliderExitCallback(Interactable i);
    public ColliderEnterCallback ColliderExit;

    public List<Interactable> m_interactablesInCollider = new List<Interactable>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            m_interactablesInCollider.Add(interactable);
            ColliderEnter.Invoke(interactable);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            m_interactablesInCollider.Remove(interactable);
            ColliderExit.Invoke(interactable);
        }
    }
}
