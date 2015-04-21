using UnityEngine;
using System.Collections;

public class GUIControl : MonoBehaviour {

	private GameObject controls;

	void Start(){
		controls = GameObject.Find ("PanelControls");
		controls.SetActive(false);
	}
	//loads the scene with the index that is passed to it
	public void ChangeToScene (int sceneToChangeTo) {
		Application.LoadLevel (sceneToChangeTo);
	}

	public void QuitGame() {
		Application.Quit ();
	}

	public void EnableGameobject(){

		if (controls.activeInHierarchy == true)
		{
			controls.SetActive(false);
		} else if (controls.activeInHierarchy == false) {
			controls.SetActive(true);
		}

	}

	public void ToggleMute(){
		//AudioListener is outputing sounds at volume, mute. if not unmute
		if(AudioListener.volume == 1){
			AudioListener.volume = 0;
		}else {
			AudioListener.volume = 1;
		}
	}
}
