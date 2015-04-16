using UnityEngine;
using System.Collections;

public class BoostPickupBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	//used to make the object appear to go away for an amount of time
	IEnumerator DisableForTime() 
	{
		//turns off the visible mesh and the collider
		renderer.enabled = false;
		collider.enabled = false;
		Debug.Log ("disabled");
		//waits 8 seconds
		yield return new WaitForSeconds (8.0f);
		Debug.Log ("enabled");
		//reenables the mesh and collider
		renderer.enabled = true;
		collider.enabled = true;
	}

	void Disable ()
	{
		StartCoroutine(DisableForTime());
	}
	// Update is called once per frame
	void Update () {
	
		transform.Rotate(0, (Time.deltaTime * 25), 0);
		transform.Rotate(0, (Time.deltaTime * 25), 0, Space.World);
	}
}
