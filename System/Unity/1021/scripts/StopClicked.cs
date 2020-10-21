using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopClicked : MonoBehaviour
{

    public SerialHandler serialHandler;

    // ボタンが押された場合、今回呼び出される関数
    public void OnClick()
    {
        Debug.Log("Stop Button Clicked!");  // ログを出力
        serialHandler.Write("0");
    }
}

