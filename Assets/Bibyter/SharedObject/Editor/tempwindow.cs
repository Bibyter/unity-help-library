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
        GUILayout.Toggle(_active, "bla");

        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        //Event evt = Event.current;

        if (GUILayout.Button("asdasd"))
        {
            _active ^= true;
            GUIUtility.hotControl = _active ? controlId : 0;
        }

        //


        Event evt = Event.current;

        switch (evt.GetTypeForControl(controlId))
        {

            case EventType.MouseDown:

                //Important, must set hotControl to receive MouseUp outside window!
                //if (_active)
                //{
                    //GUIUtility.hotControl = controlId;
                    Debug.Log("Mouse Down, Hoorah!");
                    evt.Use();
                //}

                break;

            case EventType.MouseUp:

                Debug.Log("Mouse Up, Hoorah!");
                evt.Use();
                break;
        }

        GUILayout.Label(GUIUtility.hotControl.ToString());
    }
}


