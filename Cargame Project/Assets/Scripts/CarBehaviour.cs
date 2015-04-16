using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {
	
	public string collisionTag = "";
	public string collisionTag2 = "";
	public string collisionTag3 = "";
	public GameObject singleCar;
	public GameObject resetPoint;

	//variables used to which players have finished
	private bool playerOneFinished = false;
	private bool playerTwoFinished = false;
	//variable used to store the second place finisher
	private int lastCarAcrossLine;
	// Use this for initialization
	void Start () {
		
	}
	
	//function called if the car collides with something
	void OnCollisionEnter(Collision other){
		//compares the tag of the object it collded with, if it is one of the specified tags it carrys out an action
		if (other.gameObject.CompareTag (collisionTag)) {
			//Changes the cars location to be back on the track
			Debug.Log ("registered");
			singleCar.transform.position = resetPoint.transform.position;

		} else if (other.gameObject.CompareTag (collisionTag2)) {
			//do this (placeholder for boost collection)
			singleCar.SendMessage("addBoost", 1);
			Debug.Log ("message Sent");
		}
	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.CompareTag (collisionTag2)) {
			//Send the message to the pickup object to disables itself
			other.SendMessage("Disable");
			//add the boost
			singleCar.SendMessage("addBoost", 1);
			Debug.Log ("message Sent");

		} else if (other.gameObject.CompareTag (collisionTag3)) {
			singleCar.SendMessage("addLap");
		}

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
					//insert win switchin here for player 2
					Debug.Log("Player 2 Wins!");
				} else if (lastCarAcrossLine == 2)
				{
					//insert win switching here for player 2
					Debug.Log ("Player 1 Wins!");
				}
			}
		}
		Debug.Log (playerOneFinished);
		Debug.Log (playerTwoFinished);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
