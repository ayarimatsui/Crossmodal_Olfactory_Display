using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderControll : MonoBehaviour
{
    Slider _slider;

    // Start is called before the first frame update
    void Start()
    {
        // スライダーを取得する
        _slider = GameObject.Find("Slider").GetComponent<Slider>();
    }

    //スライダーの値
    int val = 3;
    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickLeft))
        {
            if (val <= 0)
            {
                val = 0;
            }
            else
            {
                val -= 1;
            }
        }
        else if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickRight))
        {
            if (val >= 6)
            {
                val = 6;
            }
            else
            {
                val += 1;
            }
        }
        //スライダーの値を変更
        _slider.value = val;
    }
}
