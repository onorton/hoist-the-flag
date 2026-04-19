using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelData : ScriptableObject
{
    public List<string> StringsToTranslate;

    public float TimerValue;

    public GameObject TutorialPrefab;
}
