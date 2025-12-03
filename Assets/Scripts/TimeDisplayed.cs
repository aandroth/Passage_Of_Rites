using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeDisplayed : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_timeText;

    public void SetTime(int time)
    {
        m_timeText.text = time.ToString();
    }
}
