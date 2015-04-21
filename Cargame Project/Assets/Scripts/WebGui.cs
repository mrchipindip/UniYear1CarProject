using UnityEngine;
using System.Collections;

public class WebGui : MonoBehaviour
{

    private bool controlsEnabled = false;

    void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 150, 150), "Main Menu");

        // a button that when clicked will play the game
        if (GUI.Button(new Rect(20, 40, 130, 20), "Race!"))
        {
            Application.LoadLevel(1);
        }

        // make a button to open the controls
        if (GUI.Button(new Rect(20, 70, 130, 20), "Controls"))
        {
            controlsEnabled = !controlsEnabled;
        }

        // make a button to open the controls
        if (GUI.Button(new Rect(20, 100, 130, 20), "Mute"))
        {
            //AudioListener is outputing sounds at volume, mute. if not unmute
            if (AudioListener.volume == 1)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = 1;
            }
        }

        // make a button to open the controls
        if (GUI.Button(new Rect(20, 130, 130, 20), "Quit"))
        {
            Application.Quit();
        }

        if (controlsEnabled == true)
        {
            // Make a background box
            GUI.Box(new Rect(170, 10, 180, 390), "Controls");

            GUI.Label(new Rect(180, 40, 170, 20), "Pause Menu = Esc");

            GUI.Label(new Rect(180, 70, 170, 20), "Player 1");
            GUI.Label(new Rect(180, 100, 170, 20), "Driving: Arrow Keys");
            GUI.Label(new Rect(180, 130, 170, 20), "Boost: Right Shift");
            GUI.Label(new Rect(180, 160, 170, 20), "Flip Car: /");
            GUI.Label(new Rect(180, 190, 170, 20), "Reset Car: M");
            GUI.Label(new Rect(180, 220, 170, 20), "");
            GUI.Label(new Rect(180, 250, 170, 20), "Player 2");
            GUI.Label(new Rect(180, 280, 170, 20), "Driving: WASD");
            GUI.Label(new Rect(180, 310, 170, 20), "Boost: Left Shift");
            GUI.Label(new Rect(180, 340, 170, 20), "Flip Car: E");
            GUI.Label(new Rect(180, 370, 170, 20), "Reset Car: R");
        }



    }
}