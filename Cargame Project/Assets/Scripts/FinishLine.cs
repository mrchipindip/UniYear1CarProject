using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	//variables used to which players have finished
	private bool playerOneFinished = false;
	private bool playerTwoFinished = false;
	//variable used to store the second place finisher
	private int lastCarAcrossLine;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void playerFinished(int playerNum)
	{
		Debug.Log ("Finished method called");
		if (playerNum == 1)
		{
			Debug.Log ("1 finished");
			playerOneFinished = true;
			lastCarAcrossLine = 1;
			//Do somethign to player Ones Camera
			
		} else if (playerNum == 2) {
			Debug.Log ("2 finished");
			playerTwoFinished = true;
			lastCarAcrossLine = 2;
			//do something to player 2's camera
			
		}
		Debug.Log (playerOneFinished);
		Debug.Log (playerTwoFinished);
		if (playerOneFinished == true) 
		{
			if(playerTwoFinished == true)
			{
				if (lastCarAcrossLine == 1)
				{

					Debug.Log("Player 2 Wins!");
				} else if (lastCarAcrossLine == 2)
				{
					//insert win switching here for player 1
					Debug.Log ("Player 1 Wins!");
				}
			}
		}
		Debug.Log (playerOneFinished);
		Debug.Log (playerTwoFinished);
	}

}
