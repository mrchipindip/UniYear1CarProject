//---------------------------------------------------------------------------
//code created by Marius Varga for ISS at Plymouth university
//---------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class CameraSmoothFollow : MonoBehaviour 
{
    //// The target we are following
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 6.5f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    // Look above the target (height * this ratio)
    public float targetHeightRatio = 0.5f;
    // How fast we reach the target values
    public float heightDamping = 20.0f;

    public float rotDamping = 20.0f;

    public bool clipingCalculations = true;	//shall we do the clipping calculations?
    public float collisionDamping = 4.25f;		//damping when we coillide
    public float afterCollisionDamping = 1.0f;	//damping when we finished colliding
    public float brakeSmooth = 1.0f;
    public float boostSmooth = 1.0f;
    public float defaultSmooth = 1.0f;

    public bool showDebugLines = true;

    private Vector3 lastPos = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private float wantedRotationAngle = 0.0f;

    private float additionalDistance = 0.0f;

    private bool isBrakeOn = false;
    //
    public bool cameraShake = true;
    public float shakeIntensity = 0.3f;
    public float shakeDecay = 2.0f;

    private float m_shakeInt = 0;
    //
    //private ParticleSystem speedParticles;
    //

    //
    private Transform camTransform;
    //private var desiredPosition : Vector3;
    //
    //@HideInInspector
    public bool reset = true;		// Make true from scripting for resetting the direction settings

    //public float Distance = 5.0f;
    public float distanceMin = 3.0f;
    public float distanceMax = 10.0f;

    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;
    public float Z_Smooth = 0.1f;

    public float OcclusionDistanceStep = 0.5f;
    public int MaxOcclusionChecks = 10;

    public float distanceResumeSmooth = 1.0f;

    private float distanceSmooth = 0.0f;
    private float preOccludedDistance = 0.0f;

    private float velX = 0.0f;
    private float velY = 0.0f;
    private float velZ = 0.0f;

    private float velDistance = 0.0f;
    private float startDistance = 0.0f;

    //[HideInInspector]
    public Vector3 position = Vector3.zero;

    private Vector3 desiredPosition = Vector3.zero;
    private float desiredDistance = 0.0f;

    private Vector3 UpperLeft;
    private Vector3 UpperRight;
    private Vector3 LowerLeft;
    private Vector3 LowerRight;

    private Camera camMain;

    void OnEnable()
    {
        //EventManagerJavaScript.onBrake += OnBrake;
        //EventManagerJavaScript.onSpeeding += OnSpeeding;
        //EventManagerJavaScript.onKillSpeeding += OnKillSpeeding;

	    //speedParticles = transform.GetComponentInChildren<ParticleSystem>() as ParticleSystem;
	    //speedParticles.emissionRate = 0;
	    //speedParticles.active = false;
	    //ResetCamera();

	    isBrakeOn = false;
	    //camTransform = transform.Find("MainCamera");
	    camMain = transform.GetComponent<Camera>();
	    camTransform = transform;
        position = new Vector3(camTransform.position.x, camTransform.position.y, camTransform.position.z - distance);

        distance = Mathf.Clamp(distance, distanceMin, distanceMax);
        startDistance = distance;
	    Reset();
    }

    void OnDisable()
    {
        //EventManagerJavaScript.onBrake -= OnBrake;
        //EventManagerJavaScript.onSpeeding -= OnSpeeding;
        //EventManagerJavaScript.onKillSpeeding -= OnKillSpeeding;

	    //speedParticles.emissionRate = 0;
    }

    private void Reset()
    {
        distance = startDistance;
        desiredDistance = distance;
        preOccludedDistance = distance;
    }

    //private void OnBrake(bool brakeValue)
    //{
    //    if(brakeValue)
    //    {
    //        isBrakeOn = true;
    //    }
    //    else
    //    {
    //        isBrakeOn = false;
    //    }
    //}

    private void OnSpeeding(float speedingValue)
    {
	    //speedShake = shakeIntensity * speedingValue;
	    //DoShake(shakeVal);
	
	    //speedParticles.emissionRate = 15;
	
    //	if(!speedParticles.active)
    //    	speedParticles.active = true;
    	
        //speedParticles.emissionRate = 15 * speedingValue;
    }

    private void OnKillSpeeding()
    {
	    //speedShake = 0;
	    //speedParticles.emissionRate = 0.0f;
    //	if(speedParticles.active)
    //		speedParticles.active = false;
    }

    private void ResetCamera()
    {
        lastPos = target.position + (target.position-target.forward * 200.0f);
	    wantedRotationAngle = target.eulerAngles.y;
	    currentVelocity = target.forward * 2.0f;
    }

    void LateUpdate () 
    {
	    // Early out if we don't have a target
        if (!target)
        {
            Debug.Log("No Target!!!!!!!");
            return;
        }
		
	    var count = 0;
	    do
	    {
		    CalculateDesiredPosition();
		    count++;
	    } while (CheckIfOcculded(count));

	
	    UpdatePosition();
	
    //	if (reset)
    //		{
    //		lastPos = target.position;
    //		wantedRotationAngle = target.eulerAngles.y;
    //		currentVelocity = target.forward * 2.0;
    //		reset = false;
    //		}
    //		
    ////	var count = 0;
    ////	do
    ////	{
    ////		CalculateDesiredPosition();
    ////		count++;
    ////	} while (CheckIfOcculded(count));
    //	CalculateDesiredPosition();
    //	UpdatePosition();
    }

    private bool CheckIfOcculded(int count)
    {
	    var isOcculded = false;
	
	    var nearestDistance = CheckCameraPoints(target.position, desiredPosition);
	
	    if(nearestDistance != -1)
	    {
		    if(count < MaxOcclusionChecks)
		    {
			    isOcculded = true;
                distance -= OcclusionDistanceStep;

                if (distance < distanceMin)
                    distance = distanceMin;	
		    } 
		    else
                distance = nearestDistance - camMain.nearClipPlane;

            desiredDistance = distance;
            distanceSmooth = distanceResumeSmooth;
	    }
	
	    return isOcculded;
    }

    private void PopulateClipPoints(Vector3 pos)
    {
        if(camMain == null)
		    return;
	
	    //var transform = Camera.main.transform;
	    var halfFOV = (camMain.fieldOfView / 2) * Mathf.Deg2Rad;
	    var aspect = camMain.aspect;
	    var distance = camMain.nearClipPlane;
	
	    var height = distance * Mathf.Tan(halfFOV);
	    var width = height * aspect;
	
	    LowerRight = pos + camTransform.right * width;
	    LowerRight -= camTransform.up * height;
	    LowerRight += camTransform.forward * distance;
	
	    LowerLeft = pos - camTransform.right * width;
	    LowerLeft -= camTransform.up * height;
	    LowerLeft += camTransform.forward * distance;
	
	    UpperRight = pos + camTransform.right * width;
	    UpperRight += camTransform.up * height;
	    UpperRight += camTransform.forward * distance;
	
	    UpperLeft = pos - camTransform.right * width;
	    UpperLeft += camTransform.up * height;
	    UpperLeft += camTransform.forward * distance;
    }

    private float CheckCameraPoints (Vector3 from, Vector3 to)
    {
	    var nearestDistance = -1f;
	
	    RaycastHit hitInfo;
	
	    PopulateClipPoints(to);
	
	    if( showDebugLines ) {
		    // Draw lines in the editor to make it easier to visualise
	        Debug.DrawLine(from, to + camTransform.forward * -camMain.nearClipPlane, Color.red);
		    Debug.DrawLine(from, UpperLeft);
		    Debug.DrawLine(from, LowerLeft);
		    Debug.DrawLine(from, UpperRight);
		    Debug.DrawLine(from, LowerRight);
		
		
		    Debug.DrawLine(UpperLeft, UpperRight);
		    Debug.DrawLine(UpperRight, LowerRight);
		    Debug.DrawLine(LowerRight, LowerLeft);
		    Debug.DrawLine(LowerLeft, UpperLeft);
	    }
	
	    if(Physics.Linecast(from,UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player")
		    nearestDistance = hitInfo.distance;
	
	    if(Physics.Linecast(from,LowerLeft, out hitInfo) && hitInfo.collider.tag != "Player")
		    if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			    nearestDistance = hitInfo.distance;
	
	    if(Physics.Linecast(from,UpperRight, out hitInfo) && hitInfo.collider.tag != "Player")
		    if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			    nearestDistance = hitInfo.distance;
	
	    if(Physics.Linecast(from,LowerRight, out hitInfo) && hitInfo.collider.tag != "Player")
		    if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			    nearestDistance = hitInfo.distance;
	
	    if(Physics.Linecast(from, to + camTransform.forward * -camMain.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
		    if(hitInfo.distance < nearestDistance || nearestDistance == -1)
			    nearestDistance = hitInfo.distance;
	
	
	    return nearestDistance;
    }

    private void UpdatePosition()
    {
        position.x = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        position.y = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        position.z = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, Z_Smooth);
	
	    camTransform.position = position;
	
	    if (target.rigidbody)
	    {
	    // We use centerOfMass instead of worldCenterOfMass because the first one is interpolated.
		
		    Vector3 CoM = Vector3.Scale(target.rigidbody.centerOfMass, new Vector3(1.0f/target.transform.localScale.x, 1.0f/target.transform.localScale.y, 1.0f/target.transform.localScale.z));
		    CoM = target.transform.TransformPoint(CoM);
		
		    camTransform.LookAt (CoM + Vector3.up*height*targetHeightRatio);
	    }
	    else
		    camTransform.LookAt (target.position + Vector3.up*height*targetHeightRatio);
		
		    if(!cameraShake)
		    return;
		
		    //m_shakeInt = speedShake;
		
		    if(m_shakeInt > 0)
	        {
	            camTransform.position = camTransform.position + Random.insideUnitSphere * m_shakeInt;
                float rotShake = m_shakeInt * 0.25f;
	            camTransform.rotation = new Quaternion(camTransform.rotation.x + Random.Range(-rotShake, rotShake)*.25f,
	                                      camTransform.rotation.y + Random.Range(-rotShake, rotShake)*.25f,
	                                      camTransform.rotation.z + Random.Range(-rotShake, rotShake)*.25f,
	                                      camTransform.rotation.w + Random.Range(-rotShake, rotShake)*.25f);
	
	            m_shakeInt -= shakeDecay * Time.deltaTime;
	        }
    }

    private void CalculateDesiredPosition()
    {
	    // evaluate distance
	    ResetDesiredDistance();
        distance = Mathf.SmoothDamp(distance, desiredDistance, ref velDistance, distanceSmooth);

        if (distance > distanceMax)
            distance = distanceMax;
	
	    // calculate desired position
        desiredPosition = CalculatePosition(distance);
    }

    private Vector3 CalculatePosition(float distance)
        {

            var currentRotationAngle = camTransform.eulerAngles.y;
            
            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle,
                                         rotDamping * Time.deltaTime);

            wantedRotationAngle = target.eulerAngles.y;

            if(isBrakeOn)
            {
        	    var brakeDist = additionalDistance - 0.5f;
                additionalDistance = Mathf.Lerp(additionalDistance, brakeDist, brakeSmooth * Time.deltaTime);
            }
            else
            {
                additionalDistance = Mathf.Lerp(additionalDistance, distance, defaultSmooth * Time.deltaTime);
            }

            float finalDistance = Mathf.Clamp(additionalDistance, 3.5f, 15);
	
            var tempPosition = target.position;

            var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
	
            //currentDistance = finalDistance;
            tempPosition -= currentRotation * (Vector3.forward * finalDistance);
            //tempPosition -= currentRotation * (target.forward * finalDistance);
	
            var wantedHeight = target.position.y + height;
            var currentHeight = camTransform.position.y;

           // if(currentHeight > wantedHeight)
           // {
               // currentHeight = wantedHeight;
           // }
           // else if(currentHeight < wantedHeight)
            //{
               // currentHeight = wantedHeight;
           // }
          //  else
          //  {
                currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
          //  }
	
	    tempPosition.y = currentHeight;

	    return tempPosition;
    }

    private void ResetDesiredDistance()
    {
	    if(desiredDistance < preOccludedDistance)
	    {
		    var pos = CalculatePosition(preOccludedDistance);
		
		    var nearestDistance = CheckCameraPoints(target.position, pos);
		
		    if(nearestDistance == -1 || nearestDistance > preOccludedDistance)
		    {
			    desiredDistance = preOccludedDistance;
		    }
	    }
    }
}
