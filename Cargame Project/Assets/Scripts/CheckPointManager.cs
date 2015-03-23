//---------------------------------------------------------------------------
//code created by Marius Varga for ISS at Plymouth university
//---------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class CheckPointManager : MonoBehaviour 
{
    public CheckPointBehaviour[] checkPoints;

    private int m_nextCheckPointID = 0;
    private float m_timer = 0.0f;
    private bool m_timerRunning = false;
	
	// we enable the next checkpoint
	private void EnableNextCheckPoint (int _checkPointID) 
    {
        //we move to next checkpoint
        m_nextCheckPointID = _checkPointID + 1;

        //if the next point is outside the array 
        //we loop to the beginning of the array
        if (m_nextCheckPointID == checkPoints.Length)
        {
            m_nextCheckPointID = 0;
        }

        //we enable the next checkpoint in the array
        checkPoints[m_nextCheckPointID].checkpointEnabled = true;
	}


    //this is triggered individually by each checkpoint
    public void CheckPoint(int _checkPointID)
    {
        //if checkpoint is 0 (our start finish line)
        //we start or stop the timer
        if (_checkPointID == 0)
        {
            m_timerRunning = !m_timerRunning;

            //if we decide to start the timer we reset it first
            if (m_timerRunning)
            {
                ResetTimer();
            }
        }

        //we enable the next checkpoint
        EnableNextCheckPoint(_checkPointID);
    }

    void Update()
    {
        //if the timer is running we update the timer
        if (m_timerRunning)
        {
            m_timer += Time.deltaTime;
        }

        Debug.Log(m_timer);
       // Debug.Log(nextCheckPointID);
    }

    //resets the timer
    private void ResetTimer()
    {
        m_timer = 0.0f;
    }
}
