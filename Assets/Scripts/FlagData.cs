using UnityEngine;

[CreateAssetMenu(fileName = "Flag", menuName = "Flag")]
public class FlagData : ScriptableObject
{
    public Texture2D Image;

    public string DisplayValue;

    public string Value;

    public SignalType SignalType;
}

public enum SignalType
{
    Value,
    Finishing,
    EndOfCode,
    Numeric,
    EndOfSpelling
}