using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTrialID : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int TestID = ExperimentID.getTestID();
        if (TestID <= 2)
        {
            Debug.Log("Test : " + TestID.ToString());
        }
        else
        {
            int TrialID = ExperimentID.getTrialID();
            Debug.Log("Trial : " + TrialID.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
