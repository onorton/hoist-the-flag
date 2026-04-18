using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class FlagsGrid : MonoBehaviour
{


    [SerializeField]
    private GameObject _flagPrefab;


    public void Reload()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        var files = Directory.GetFiles("Assets/Flags", "*.asset", SearchOption.TopDirectoryOnly);


        foreach (var file in files)
        {
            var flagData = (FlagData)AssetDatabase.LoadAssetAtPath(file, typeof(FlagData));

            var flag = Instantiate(_flagPrefab, transform);
            flag.GetComponent<Flag>().FlagData = flagData;
            flag.GetComponent<Flag>().InitialiseFromData();
        }
    }

    public List<Flag> GetFlags()
    {
        var flags = new List<Flag>();
        foreach (Transform flag in transform)
        {
            flags.Add(flag.GetComponent<Flag>());
        }

        return flags;
    }


}
