using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class Temp : MonoBehaviour
{
    string _path;

    private void Start()
    {
        print(System.IO.Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory));
        _path = Directory.GetCurrentDirectory();

        print($"CurrentPath: {_path}");
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.U))
        {
            var parent = Directory.GetParent(_path);

            if (parent != null)
            {
                _path = parent.FullName;
                print($"CurrentPath: {_path}");
            }
            else
            {
                print("error");
            }
        }

        // print directory
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            PrintFolder(_path);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            var desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            print(desktopPath);
            PrintFolder(desktopPath);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            var bytes = File.ReadAllBytes("C:/Users/Nikita/Desktop/1.PNG");
            File.WriteAllBytes("C:/Users/Nikita/Desktop/temp.PNG", bytes);
        }
    }

    private void PrintFolder(string path)
    {
        var files = Directory.GetFiles(path);
        var folders = Directory.GetDirectories(path);

        var printstr = "Files: \n";

        for (int i = 0; i < folders.Length; i++)
        {
            printstr += folders[i] + '\n';
        }

        for (int i = 0; i < files.Length; i++)
        {
            printstr += files[i] + '\n';
        }

        print(printstr);
    }
}
