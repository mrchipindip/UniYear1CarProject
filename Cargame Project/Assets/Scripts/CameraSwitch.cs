using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {
	public GameObject cameraOne;
	public GameObject cameraTwo;
	public GameObject cameraToDisable;
	// Use this for initialization
	void Start () {
		StartCoroutine (switchCamera ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator switchCamera()
	{

		yield return new WaitForSeconds (0.0001f); 
		cameraOne.SetActive (true);
		cameraTwo.SetActive (true);
		cameraToDisable.SetActive (false);
	}
}
