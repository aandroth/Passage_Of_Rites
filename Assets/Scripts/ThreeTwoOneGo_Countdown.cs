using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ThreeTwoOneGo_Countdown : MonoBehaviour
{

    [SerializeField] TMPro.TextMeshProUGUI m_timeText;
    [SerializeField] float m_time;

    //public void Start()
    //{
    //    StartCountdown(m_time);
    //}

    public void StartCountdown(float time)
    {
        StartCoroutine(ThreeTwoOneGo_CountdownCoroutine(time));
    }

    private IEnumerator ThreeTwoOneGo_CountdownCoroutine(float totalTime)
    {
        float timePerNumber = totalTime * 0.25f;
        float timeToGrowOrShrink = timePerNumber * 0.5f;
        float timeCount = 0;
        int currCount = 3;
        m_timeText.text = currCount.ToString();
        Color lerpColor = new Color(1f, 1f, 1f, 1f);
        while (currCount > 0)
        {
            m_timeText.color = new Color(1f, 1f, 1f, 1f);
            // Grow
            while (timeCount < timeToGrowOrShrink)
            {
                timeCount += Time.deltaTime;
                float lerpScale = Mathf.Lerp(0, 1, timeCount / timeToGrowOrShrink);
                Vector3 lerpVector = new Vector3(lerpScale, lerpScale, 1);
                m_timeText.gameObject.transform.localScale = lerpVector;
                yield return null;
            }
            timeCount = timeToGrowOrShrink;

            // Fade Out
            while (timeCount > 0)
            {
                timeCount -= Time.deltaTime;
                float lerpScale = Mathf.Lerp(0, 1, timeCount / timeToGrowOrShrink);
                lerpColor.a = lerpScale;
                m_timeText.color = lerpColor;
                yield return null;
            }

            //// Shrink
            //while (timeCount > 0)
            //{
            //    timeCount -= Time.deltaTime;
            //    float lerpScale = Mathf.Lerp(0, 1, timeCount / timeToGrowOrShrink);
            //    Vector3 lerpVector = new Vector3(lerpScale, lerpScale, 1);
            //    m_timeText.gameObject.transform.localScale = lerpVector;
            //    yield return null;
            //}
            timeCount = 0;

            currCount--;
            if(currCount > 0)
                m_timeText.text = currCount.ToString();
        }
        m_timeText.text = "GO!";
        m_timeText.color = new Color(1f, 1f, 1f, 1f);
        // Grow
        while (timeCount < timeToGrowOrShrink)
        {
            timeCount += Time.deltaTime;
            float lerpScale = Mathf.Lerp(0, 1, timeCount / timeToGrowOrShrink);
            Vector3 lerpVector = new Vector3(lerpScale, lerpScale, 1);
            m_timeText.gameObject.transform.localScale = lerpVector;
            yield return null;
        }
        timeCount = timeToGrowOrShrink;
        // Fade Out
        lerpColor = new Color(1f, 1f, 1f, 1f);
        while (timeCount > 0)
        {
            timeCount -= Time.deltaTime;
            float lerpScale = Mathf.Lerp(0, 1, timeCount / timeToGrowOrShrink);
            lerpColor.a = lerpScale;
            m_timeText.color = lerpColor;
            yield return null;
        }
    }
}
