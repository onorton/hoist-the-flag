using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TranslationText : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _key;
    [SerializeField]
    private Transform _valuesTransform;

    [SerializeField]
    private GameObject _valuePrefab;

    public string Key;
    public List<string> KnownValues;

    private void Start()
    {
        _key.text = $"{Key} = ";

        foreach (var value in KnownValues)
        {
            var v = Instantiate(_valuePrefab, _valuesTransform);
            v.GetComponent<TextMeshProUGUI>().text = value;
        }
    }

}
