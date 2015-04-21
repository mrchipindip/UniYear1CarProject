//---------------------------------------------------------------------------
//code created by Marius Varga for ISS at Plymouth university
//---------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class PlayerCar : MonoBehaviour 
{
    //we use centre of mass to stabilise the car, the lower centre of mass, more stable will be.
    //You can Use this with a force pushing the car down based on the amount of air goes over the 
    //car, downforce
    public Renderer carRenderer;
    public Transform centreOfMass;
  
	public string inputForTurning = "";
	public string inputForAccelerating = "";

    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider backLeftWheel;
    public WheelCollider backRightWheel;

    [Range(200.0f, 4000.0f)]
    public float engineTorque = 500.0f;

    [Range(0.0f, 1.0f)]
    public float tractionBallance = 0.0f;

    [Range(0.0f, 300.0f)]
    public float maximumSpeed = 180.0f;

    [Range(0.0f, 100.0f)]
    public float downForceCoefficient = 0;

    [Range(1.0f, 10000.0f)]
    public float minimumDownForceCoefficient = 0;

    [Range(0.0f, 1000.0f)]
    public float brakeForceFront = 300.0f;

    [Range(0.0f, 1000.0f)]
    public float brakeForceBack = 100.0f;

	//stores the amount of boosts available
	public int boosts = 0;
    //we use this for applying torque to the wheels, the lower the value the higher the torque
    public float[] gearRatio;
    private int m_currentGear = 0;
    private float m_appliedTorque = 0.0f;

   //determines the gear the engine needs to be in 
    private float m_maxEngineRPM = 2000.0f;
    private float m_minEngineRPM = 1400.0f;
    private float m_engineRPM = 0.0f;
    private int m_desiredGear = 0;

    //boolean linked to braking
    private bool m_isBraking = false;
    private bool m_isHandBraking = false;

    //downforce
    private float m_downForce = 0;

    private float speed = 0;

	//Variable used when boosting
	private float boostSpeed = 1;

	//stores the original rotation of the car at start
	public Quaternion originalRotationValue;
	public float rotationResetSpeed = 2.0f;

	//stores the values of current lap and the total lap
	public int currentLap = 0;
	public int totalLaps = 3;

	private GameObject finishLine;
	private GameObject startFinish;

	//holds the gameobjects for both the racing camera and the finished camera
	private GameObject initialCamera;
	private GameObject finishedCamera;

	//stores the current value of the countdown and the bool that ditermines if its on
	private string countdown = "";
	private bool showCountdown = false;

	//stores the gameobject at which location the car will be reset to if it fgoes outside the map
	public GameObject resetPoint;

	//holds the font to be used in my GUI
	public Font myFont;

	//holds the gameobject that prevents the cars from going until the countdown is over
	private GameObject startPrevention;

	//holds all of the audio for the countdown
	public AudioSource countdown1;
	public AudioSource countdown2;
	public AudioSource countdown3;
	public AudioSource countdown4;
	// Use this for initialization
	void Start () 
    {
		startFinish = GameObject.Find ("StartFinish");
		//populate the variable gameobject wiht the startfinish line
		finishLine = GameObject.Find("StartFinish");

		initialCamera = GameObject.Find ("Main Camera Car One");
		finishedCamera = GameObject.Find ("Finish Camera Car One");
		finishedCamera.SetActive(false);
        rigidbody.centerOfMass = centreOfMass.localPosition;

		startPrevention = GameObject.Find ("StartColliders");

        //we calculate the down force coefficient using the formula: 
        //d = 1/2 * (car.width * car.height) * car.airDrag * (car.velocity * car.velocity);
        //m_downForce = m_downForceCoefficient * (car.velocity * car.velocity); - we do this in the update 
        //m_downForceCoefficient = 0.5f * (car.width * car.height) * car.airDrag; - we do this in start method 
        
        //this is half of car width and height
        //float halfCarWidth = carRenderer.bounds.extents.x;
       // float halfCarHeight = carRenderer.bounds.extents.y;
        //float widthHeightCoefficient = (carRenderer.bounds.extents.x - carRenderer.bounds.extents.y);

        //if (widthHeightCoefficient < 0.0001f)
        //{
        //    widthHeightCoefficient = 0.0001f;
        //}

        //m_downForceCoefficient = widthHeightCoefficient * rigidbody.drag;
        //Debug.Log(m_downForceCoefficient);

		StartCoroutine(StartCountdown());
	}

    void FixedUpdate()
    {
        var locVel = transform.InverseTransformDirection(rigidbody.velocity);
        speed = locVel.z;

       // m_downForce = m_downForceCoefficient * (locVel.z * locVel.z);
        m_downForce = minimumDownForceCoefficient + downForceCoefficient * locVel.z;
        rigidbody.AddForce(new Vector3(0.0f, -1.0f, 0.0f) * m_downForce * 100.0f);
    }
	
	// Update is called once per frame
	void Update () 
    {
        // Compute the engine RPM based on the average RPM of the two wheels, then call the shift gear function
        m_engineRPM = ((frontLeftWheel.rpm + frontRightWheel.rpm + backLeftWheel.rpm + backRightWheel.rpm) / 4.0f) * gearRatio[m_currentGear];
        ShiftGears();

        // set the audio pitch to the percentage of RPM to the maximum RPM plus one, this makes the sound play
        // up to twice it's pitch, where it will suddenly drop when it switches gears.
        audio.pitch = Mathf.Abs(m_engineRPM / m_maxEngineRPM) + 1.0f;
        // this line is just to ensure that the pitch does not reach a value higher than is desired.
        if (audio.pitch > 2.0f)
        {
            audio.pitch = 2.0f;
        }

        //if (Input.GetKey(KeyCode.RightShift))
        //{
            //apply braking
        //    if (!m_isHandBraking)
       //         HandBrake();
       // }
      //  else
      //  {
      //      if(m_isHandBraking)
      //          ReleaseHandBrake();
      //  }

		//test function for boost
		if (Input.GetKeyUp(KeyCode.RightShift))
		{
			Debug.Log ("called the routine");
			//check if the player has any available boosts
			if (boosts > 0)
			{
				Debug.Log("Called");
				//call the boost function
				boosts -= 1;
				StartCoroutine(BoostUsed());
			}
		}

		//Call For the FlipCar Function
		if (Input.GetKeyUp (KeyCode.Slash)) 
		{
			FlipCar();
		}

		//Call For the ResetCar Function
		if (Input.GetKeyUp (KeyCode.M)) 
		{
			ResetCar();
		}
        //here we apply the torque values to the wheels. If we brake we apply a brake torque, otherwise
        //we let the wheels spin freely or we accelarate when we press forward
        //IMPORTANT NOTE 1: this is applied only to the front wheels, if you need more control or you 
        //want to implement a 4x4 than use the back wheels as well (not implemented at the moment)
        //IMPORTANT NOTE 2: If we want to create a hand brake system, we lock only the back wheels
        // and that will create the car back to step out. Normal braking is a combination of both front
        //and back but handbrake is only back wheels 
        if (Input.GetKey(KeyCode.Space))
        {
            //apply braking
            if(!m_isBraking)
                Brake();
        }
        else
        {
            //release brake
            if (m_isBraking)
                ReleaseBrake();

            if (speed < maximumSpeed)
            {
                m_appliedTorque = engineTorque / gearRatio[m_currentGear] * Input.GetAxis(inputForAccelerating) * boostSpeed;
            }
            else
            {
                m_appliedTorque = 0.0f;
            }

            //apply torque if we press forward button
            frontLeftWheel.motorTorque = m_appliedTorque * (1.0f - tractionBallance);
            frontRightWheel.motorTorque = m_appliedTorque * (1.0f - tractionBallance); ;

            if (!m_isHandBraking)
            {
                backLeftWheel.motorTorque = m_appliedTorque * tractionBallance;
                backRightWheel.motorTorque = m_appliedTorque * tractionBallance;
            }
        }

        // the steer angle is an arbitrary value multiplied by the user input.
        frontLeftWheel.steerAngle = 25.0f * Input.GetAxis(inputForTurning);
        frontRightWheel.steerAngle = 25.0f * Input.GetAxis(inputForTurning);
	}

    //this is where we apply braking, please experiment with different values
    private void Brake()
    {
        frontLeftWheel.brakeTorque = brakeForceFront;
        frontRightWheel.brakeTorque = brakeForceFront;

        backLeftWheel.brakeTorque = brakeForceBack;
        backRightWheel.brakeTorque = brakeForceBack;

        frontLeftWheel.motorTorque = 0.0f;
        frontRightWheel.motorTorque = 0.0f;
        backLeftWheel.motorTorque = 0.0f;
        backRightWheel.motorTorque = 0.0f;

        m_isBraking = true;
    }

    //we readjust the brake torque when we stopped pressing the brake
    private void ReleaseBrake()
    {
        frontLeftWheel.brakeTorque = 0.0f;
        frontRightWheel.brakeTorque = 0.0f;

        backLeftWheel.brakeTorque = 0.0f;
        backRightWheel.brakeTorque = 0.0f;

        m_isBraking = false;
    }

    private void HandBrake()
    {
        backLeftWheel.brakeTorque = 3000;
        backRightWheel.brakeTorque = 3000;

        backLeftWheel.motorTorque = 0.0f;
        backRightWheel.motorTorque = 0.0f;

        m_isHandBraking = true;
    }

    private void ReleaseHandBrake()
    {
        backLeftWheel.brakeTorque = 0.0f;
        backRightWheel.brakeTorque = 0.0f;

        m_isHandBraking = false;
    }

    //here we execute the gear change
    private void ShiftGears() 
    {
        //Debug.Log(frontLeftWheel.rpm);
	    if ( m_engineRPM >= m_maxEngineRPM ) 
        {
            m_desiredGear = m_currentGear;
		
		    for ( var i = 0; i < gearRatio.Length; i ++ ) {
                if (frontLeftWheel.rpm * gearRatio[i] < m_maxEngineRPM)
                {
                    m_desiredGear = i;
				    break;
			    }
		    }

            m_currentGear = m_desiredGear;
	    }
	
	    if ( m_engineRPM <= m_minEngineRPM ) 
        {
            m_desiredGear = m_currentGear;
		
		    for ( var j = gearRatio.Length-1; j >= 0; j -- ) {
                if (frontLeftWheel.rpm * gearRatio[j] > m_minEngineRPM / 1.3)
                {
                    m_desiredGear = j;
				    break;
			    }
		    }

            m_currentGear = m_desiredGear;
	    }
    }
	//adds a passed amount of boosts to the players available boosts. With a limit of 3
	public void addBoost(int newboost)
	{
		//the passed amount is added
		boosts += newboost;
		Debug.Log ("BoostAdded");
		//checked to see if its over the amount, if it is, set to 3
		if (boosts > 3){
			boosts = 3;
		}
	}

	//Function called when a boost is used
	IEnumerator BoostUsed()
	{
		//increases the variable value so that in the update the applied torque is multiplied by the new value
		boostSpeed = 3;
		yield return new WaitForSeconds (2.0f); //Wait the duration of the boost
		Debug.Log("Variable reset");
		boostSpeed = 1;//then reset the variable
	}

	//Function used to rotate the car if it flips over
	void FlipCar()
	{
		Debug.Log ("Called FlipCar");
		//sets the rotation of the car back to its original rotation
		transform.rotation = Quaternion.Slerp (transform.rotation, originalRotationValue, Time.time * rotationResetSpeed);
	}

	//Adds a lap to current lap and checks to see if finsished
	public void addLap()
	{
		currentLap = currentLap + 1;
		Debug.Log ("addLap Called");

		startFinish.collider.enabled = false;
		//check if the previous lap was the final one
		if (currentLap >= (totalLaps + 1))
		{
			Debug.Log("Player one finished");
			//switch to the finished camera
			initialCamera.SetActive(false);
			finishedCamera.SetActive(true);
			//call function for player 1 finish
			finishLine.SendMessage("playerFinished", 1);
		}
	}

	//resets the cars location to teh start of the lap and changes its rotation
	void ResetCar()
	{
		//make the poition equal to the resetpoints poition
		transform.position = resetPoint.transform.position;
		//call FlipCar
		FlipCar ();
	}
	//step through a countdown on GUI
	IEnumerator StartCountdown()    
	{
		//change bool value so that it is displayed on GUI
		showCountdown = true;    

		countdown = "3";    
		countdown1.Play ();
		yield return new WaitForSeconds (1.5f); //Wait before moving to next number 
		
		countdown = "2";
		countdown2.Play ();
		yield return new WaitForSeconds (1.5f); //Wait before moving to next number 
		
		countdown = "1";
		countdown3.Play ();
		yield return new WaitForSeconds (1.5f); //Wait before moving to next number 
		
		countdown = "GO!!";
		countdown4.Play ();

		startPrevention.SetActive (false);
		showCountdown = false;
		countdown = "";  
	}

    void OnGUI()
    {
		//create new style for the standard HUD
		GUIStyle myStyle = new GUIStyle ();
		myStyle.font = myFont; //make the font the passed public variable
		myStyle.fontSize = 35; //make the size 35

		GUIStyle countdownStyle = new GUIStyle ();
		countdownStyle.font = myFont;
		countdownStyle.fontSize = 45;

        GUI.Label(new Rect(5.0f, Screen.height/2 + 10, 200.0f, 30.0f), "Speed: " + speed.ToString("#0.00"), myStyle);
		GUI.Label(new Rect(5.0f, Screen.height/2 + 50, 200.0f, 30.0f), "Boosts: " + boosts.ToString(), myStyle);
		GUI.Label(new Rect(Screen.width - 205.0f, (Screen.height/2) + 10, 200.0f, 30.0f), "Lap: " + currentLap.ToString() + "/" + totalLaps.ToString(), myStyle);

		//check if  countdown is running
		if (showCountdown)
		{    
			GUI.color = Color.red;    
			GUI.Label (new Rect (Screen.width / 2 - 100, 430.0f, 200.0f, 175.0f), "GET READY", countdownStyle);
			
			// display countdown    
			GUI.color = Color.white;    
			GUI.Label (new Rect (Screen.width / 2, 490.0f, 180.0f, 140.0f), countdown, countdownStyle);
		} 
    }
}
