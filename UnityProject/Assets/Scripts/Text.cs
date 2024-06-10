using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Text : MonoBehaviour
{
    [SerializeField] private string _pretext;
    [SerializeField] private TMP_Text _text;

    void Awake()
    {
        _text.text = null;
    }

    public void ChangeText(string value)
    {
        _text.text = _pretext + value;
    }
}
