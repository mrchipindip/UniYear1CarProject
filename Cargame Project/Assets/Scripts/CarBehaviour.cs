using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {
	
	public string collisionTag = "";
	public string collisionTag2 = "";
	public string collisionTag3 = "";
	public string collisionTag4 = "";
	public string collisionTag5 = "";
	public string startFinishInput = "";

	public GameObject singleCar;
	public GameObject resetPoint;
	public GameObject resetPoint2;

	public GameObject startFinish;

	private bool isColliding;
	// Use this for initialization
	void Start () {
		startFinish = GameObject.Find (startFinishInput);
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

		} else if (other.gameObject.CompareTag (collisionTag5)) {
			//REset the cars location to back on the track
			singleCar.transform.position = resetPoint2.transform.position;
		}
	}

	void OnTriggerEnter(Collider other){

		if (other.gameObject.CompareTag (collisionTag2)) {
			//Send the message to the pickup object to disables itself
			other.SendMessage ("Disable");
			//add the boost
			singleCar.SendMessage ("addBoost", 1);
			Debug.Log ("message Sent");

		} else if (other.gameObject.CompareTag (collisionTag3)) {
				
			if (isColliding)return;
			isColliding = true;	

			singleCar.SendMessage("addLap");
		} else if (other.gameObject.CompareTag (collisionTag4)) {

			startFinish.collider.enabled = true;
			Debug.Log ("startfinish activated");

		}
	}

	
	// Update is called once per frame
	void Update () {
		isColliding = false;
	}
}
