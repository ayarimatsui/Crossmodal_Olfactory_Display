using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveTo2ndScent : MonoBehaviour
{
    public GameObject SerialHandler;
    public GameObject ButtonStop;
    float totalTime;

    // Start is called before the first frame update
    void Start()
    {
        totalTime = 15;
    }

    // Update is called once per frame
    void Update()
    {
        totalTime -= Time.deltaTime;

        // 15秒経ったら、2つめの匂いのシーンに遷移
        if (totalTime <= 0)
        {
            SerialHandler.GetComponent<SerialHandler>().enabled = false;
            ButtonStop.GetComponent<StopClicked>().enabled = false;
            SceneManager.LoadScene("2nd_Scent");
        }
    }
}
