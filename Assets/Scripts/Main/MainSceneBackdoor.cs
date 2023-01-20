using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainSceneBackdoor : MonoBehaviour
{
    private PlayerModel _player;

    void Start()
    {
        _player = FindObjectOfType<PlayerModel>();
    }

    public void SetBackdoorActive(bool isActive)
    {
        enabled = isActive;
    }

    // 備考：プライヤーをむりやり移動できるように、Updateの代わりに、LateUpdateを使う。
    private void LateUpdate()
    {
        if((Gamepad.current != null && Gamepad.current.leftShoulder.isPressed && Gamepad.current.rightShoulder.isPressed) ||
            (Keyboard.current != null && Keyboard.current.digit5Key.wasPressedThisFrame))
        {
            // デフォルトの位置
            transform.position = new Vector3(13.665f, 0.0f, -38.775f);
            transform.eulerAngles = new Vector3(0.0f, -18.26f, 0.0f);
            GameProgress.Save(transform, new List<CardType> { CardType.White, CardType.Red, CardType.Blue, CardType.Yellow });
            SceneControl.ChangeScene(SceneControl.MainSceneName);
        }
        else if((Gamepad.current != null && Gamepad.current.leftTrigger.isPressed && Gamepad.current.rightTrigger.isPressed) ||
            (Keyboard.current != null && Keyboard.current.digit6Key.wasPressedThisFrame))
        {
            GameProgress.Reset();
            SceneControl.ChangeScene(SceneControl.MainSceneName);
        }
        else if((Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame) ||
               (Keyboard.current != null && Keyboard.current.digit9Key.wasPressedThisFrame))
        {
            SceneControl.ChangeScene(SceneControl.MainSceneName);
        }
        else if((Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.digit0Key.wasPressedThisFrame))
        {
            SceneControl.ChangeScene(SceneControl.TitleSceneName);
        }
        else if((Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame))
        {
            // 終点の辺り
            SetPlayerTransform(new Vector3(-6, 0, 0), new Vector3(0, 90, 0));
        }
        else if((Gamepad.current != null && Gamepad.current.leftShoulder.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame))
        {
            // Card_Blueの辺り
            SetPlayerTransform(new Vector3(-12.8f, 0, -39.1f), new Vector3(0, 135, 0));
        }
        else if((Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.digit3Key.wasPressedThisFrame))
        {
            // Card_Redの辺り
            SetPlayerTransform(new Vector3(28.6f, 0, 23.3f), new Vector3(0, 135, 0));
        }
        else if((Gamepad.current != null && Gamepad.current.leftTrigger.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.digit4Key.wasPressedThisFrame))
        {
            // Card_Yellowの辺り
            SetPlayerTransform(new Vector3(-24.45f, 0, 0f), new Vector3(0, 270, 0));
        }
    }

    private void SetPlayerTransform(Vector3 position, Vector3 rotation)
    {
        if(_player == null)
        {
            Debug.LogWarning("プレイヤーはヌルになる。移動できない。");
            return;
        }

        _player.transform.position = position;
        _player.transform.eulerAngles = rotation;
    }
}
