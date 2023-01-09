using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditTitleName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _name;

    public void SetDetails(string title, string name)
    {
        _title.text = title;
        _name.text = name;
    }
}
