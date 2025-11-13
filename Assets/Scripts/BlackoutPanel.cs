using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutPanel : MonoBehaviour
{
    [SerializeField]
    Image m_panel = null;
    [SerializeField]
    float m_fadeOutTime = 3f;
    [SerializeField]
    float m_fadeInTime = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(m_panel != null) m_panel.color = Color.black;
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeInOrOutCoroutine(-1));
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeInOrOutCoroutine(1));
    }
    public IEnumerator FadeInOrOutCoroutine(int sign)
    {
        float fadeOutTime = m_fadeOutTime;
        Color currColor = m_panel.color;
        while (fadeOutTime > 0)
        {
            fadeOutTime += Time.deltaTime * sign;
            currColor.a = fadeOutTime / m_fadeOutTime;
            m_panel.color = currColor;
            yield return null;
        }
        currColor.a = sign == 1 ? 1 : 0;
        m_panel.color = currColor;
    }
}
