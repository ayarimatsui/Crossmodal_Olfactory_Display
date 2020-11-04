using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishIntermission : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //TrialIDの更新
        ExperimentID.addNumberOfTrialID();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            SceneManager.LoadScene("Waiting_Scene0");
        }
    }
}
