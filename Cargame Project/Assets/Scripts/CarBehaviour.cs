using UnityEngine;
using System.Collections;

public class CarBehaviour : MonoBehaviour {
	
	public string collisionTag = "";
	public string collisionTag2 = "";
	public GameObject singleCar;
	public GameObject resetPoint;
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
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
