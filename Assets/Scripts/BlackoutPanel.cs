using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutPanel : MonoBehaviour
{
    [SerializeField]
    Image m_panel = null;
    [SerializeField]
    float m_fadeTime = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(m_panel != null) m_panel.color = Color.black;
    }

    public void StartFadeOut(float fadeOutTime = 3)
    {
        m_fadeTime = fadeOutTime;
        StartCoroutine(FadeOutCoroutine());
    }

    public void StartFadeIn(float fadeInTime = 3)
    {
        m_fadeTime = fadeInTime;
        StartCoroutine(FadeInCoroutine());
    }
    public IEnumerator FadeOutCoroutine()
    {
        float fadeOutTime = m_fadeTime;
        Color currColor = m_panel.color;
        currColor.a = 1;
        while (fadeOutTime > 0)
        {
            fadeOutTime -= Time.deltaTime;
            currColor.a = fadeOutTime / m_fadeTime;
            m_panel.color = currColor;
            yield return null;
        }
        currColor.a = 0;
        m_panel.color = currColor;
    }
    public IEnumerator FadeInCoroutine()
    {
        float fadeInTime = 0;
        Color currColor = m_panel.color;
        currColor.a = 0;
        while (fadeInTime < m_fadeTime)
        {
            fadeInTime += Time.deltaTime;
            currColor.a = fadeInTime / m_fadeTime;
            m_panel.color = currColor;
            yield return null;
        }
        currColor.a = 1;
        m_panel.color = currColor;
    }
}
