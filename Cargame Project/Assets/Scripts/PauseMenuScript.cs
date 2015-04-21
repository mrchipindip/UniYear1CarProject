using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour 
{

	public string levelToLoad;
	public bool paused = false;
	public Font myFont;
    public GameObject cameraOne;
    public GameObject cameraTwo;
    public GameObject cameraReset;
	
	private void Start()
	{
		Time.timeScale=1; //Set the timeScale back to 1 to unpause the game incase its paused


	}
	
	private void Update()
	{
		
		if (Input.GetKeyDown(KeyCode.Escape) ) //check for the escape key
		{
			if (paused)
				paused = false;  //unpause the game if already paused
			else
				paused = true;  //pause the game if not paused
		}
		
		if(paused)
			Time.timeScale = 0;  //set the timeScale to 0 so that the game stops
		else
            cameraOne.transform.position = cameraReset.transform.position;
            cameraTwo.transform.position = cameraReset.transform.position;
			Time.timeScale = 1;  //revert it when unpausing
        
	}
	
	private void OnGUI()
	{

		//create new style for the standard HUD
		GUIStyle myStyle = new GUIStyle ();
		myStyle.font = myFont; //make the font the passed public variable
		myStyle.fontSize = 35; //make the size 35

		//if pause make the menu
		if (paused){    
			
			GUI.Label(new Rect(Screen.width/2-25, Screen.height/2-210, 100, 40), "PAUSED", myStyle);
			
			if (GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-150, 200, 90), "Resume", myStyle)){
				paused = false;
			}
			
			if (GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-45, 200, 90), "Restart", myStyle)){
				Application.LoadLevel(Application.loadedLevel);
			}
			
			if (GUI.Button(new Rect(Screen.width/2-100, Screen.height/2+60, 200, 90), "Mute", myStyle)){
				//AudioListener is outputing sounds at volume, mute. if not unmute
				if(AudioListener.volume == 1){
					AudioListener.volume = 0;
				}else {
					AudioListener.volume = 1;
				}
			} 

			if (GUI.Button(new Rect(Screen.width/2-100, Screen.height/2+175, 200, 90), "Main Menu", myStyle)){
				Application.LoadLevel(levelToLoad);
			} 
		}
	}
}