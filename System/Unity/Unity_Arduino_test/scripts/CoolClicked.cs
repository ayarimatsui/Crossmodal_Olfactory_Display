using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolClicked : MonoBehaviour
{

    public SerialHandler serialHandler;

    // ボタンが押された場合、今回呼び出される関数
    public void OnClick()
    {
        Debug.Log("Cool Button Clicked!");  // ログを出力
        serialHandler.Write("C");
    }
}
