//---------------------------------------------------------------------------
//code created by Marius Varga for ISS at Plymouth university
//---------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class CheckPointBehaviour : MonoBehaviour 
{
    public int checkPointID = -1;
    public bool checkpointEnabled = false;

    private CheckPointManager m_checkPointManager;  //reference to the checkpoint manager

    void Start()
    {
        //we get the reference to the checkpoint manager
        m_checkPointManager = transform.parent.GetComponent<CheckPointManager>();
    }

	// Use this for initialization
	void OnTriggerEnter () 
    {
        //if checkpoint is not enable we exit the method
        if (!checkpointEnabled)
            return;

        m_checkPointManager.CheckPoint(checkPointID); // we check the point on the manager

        // we disable this point so it won't trigger if we drive back through it
        checkpointEnabled = false; 
	}
}
