using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class Screenshot : MonoBehaviour
{

    [Header("保存先の設定")]
    [SerializeField]
    string folderName = "Screenshots";

    bool isCreatingScreenShot = false;
    string path;

    void Start()
    {
        path = Application.dataPath + "/" + folderName + "/";
    }

    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            PrintScreen();
        }
    }

    public void PrintScreen()
    {
        StartCoroutine("PrintScreenInternal");
    }

    IEnumerator PrintScreenInternal()
    {
        if(isCreatingScreenShot)
        {
            yield break;
        }

        isCreatingScreenShot = true;

        yield return null;

        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string date = DateTime.Now.ToString("yy-MM-dd_HH-mm-ss");
        string fileName = path + date + ".png";

        ScreenCapture.CaptureScreenshot(fileName);

        Debug.Log("キャプチャーしました。ルートは：" + fileName);

        yield return new WaitUntil(() => File.Exists(fileName));

        isCreatingScreenShot = false;
    }

}