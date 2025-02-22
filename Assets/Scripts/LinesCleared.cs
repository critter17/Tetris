using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LinesCleared : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI linesClearedText;

    private void Awake()
    {
        EventManager.LineCleared += UpdateLinesValue;
    }

    public void UpdateLinesValue()
    {
        int textValue = Int32.Parse(linesClearedText.text);
        linesClearedText.text = (++textValue).ToString();
    }
}
