using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UrlButton : MonoBehaviour
{
    public Backend m_backend = null;
    public TMP_Text m_buttonText;

    public void TextToReturnValue()
    {
        if (m_backend == null)
            return;

        if (m_backend.GetTextData == null)
            m_backend.GetTextData = WriteTextInButton;

        m_backend.ReturnUrlResult();
    }
    public void WriteTextInButton(string str)
    {
        m_buttonText.text = str;
    }
}