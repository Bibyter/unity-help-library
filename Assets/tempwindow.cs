using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class tempwindow : EditorWindow
{
    bool _active;

    [MenuItem("Window/tempwindow")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (tempwindow)EditorWindow.GetWindow(typeof(tempwindow));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.TextField("blabla");

        if (GUILayout.Button("asdasd"))
        {
            Debug.Log(Event.current.clickCount);
        }
    }
}


