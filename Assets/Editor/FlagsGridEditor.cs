


using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FlagsGrid))]
public class FlagsGridEditor : Editor
{

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();

        var reloadButton = new Button(OnReloadClicked)
        {
            text = "Reload"
        };
        root.Add(reloadButton);

        return root;
    }

    private void OnReloadClicked()
    {
        (target as FlagsGrid).Reload();
    }
}