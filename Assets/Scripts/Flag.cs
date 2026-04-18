using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Flag : MonoBehaviour
{

    public FlagData FlagData;

    public void InitialiseFromData()
    {
        GetComponentInChildren<Image>().sprite = Sprite.Create(FlagData.Image, new Rect(0, 0, FlagData.Image.width, FlagData.Image.height), Vector2.zero);
        GetComponentInChildren<TextMeshProUGUI>().text = FlagData.DisplayValue;

    }

    public Texture2D FlagImage()
    {
        return FlagData.Image;
    }
}
