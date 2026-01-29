using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEV_SendMessageToBackend : MonoBehaviour
{
    [SerializeField] Backend m_backend = null;
    public List<string> m_rawDataStrings = new List<string>() {""};
    public bool m_createNpcForItem = false;
    public int m_npcId = 0;

    public void OnEnable()
    {
        StartCoroutine(PlayAllStrings());
    }

    public IEnumerator PlayAllStrings()
    {
        foreach(string s in m_rawDataStrings)
        {
            m_backend.ReceivedMessage(s);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
