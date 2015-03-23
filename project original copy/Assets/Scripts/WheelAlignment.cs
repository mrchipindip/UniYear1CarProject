//---------------------------------------------------------------------------
//code created by Marius Varga for ISS at Plymouth university
//---------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class WheelAlignment : MonoBehaviour
{
    //coresponding wheel collider
    public WheelCollider wheelCollider;

    //prefab used for dust or smoke when we skid
    public GameObject slipPrefab;

    //value used to rotate the wheels
    private float m_rotationValue = 0.0f;

    //the layers used for collision checking with the raycast
    private int m_layerMask;

    //values used for aligning the wheels to the wheel colliders on the car
    private Vector3 m_colliderCenterPoint;
    private Transform m_colliderTransform;

	// Use this for initialization
	void Start () 
    {
        //we use  layer 8 for car collider and we inverse to force the check to happen on all other layers 
        m_layerMask = 1 << 8;
        m_layerMask = ~m_layerMask;

        m_colliderTransform = wheelCollider.transform;
	}
	
	// we do the aligning of the wheels in the update because it's a visual element and we should refresh it 
    //as often as possible
    void Update()
    {

        RaycastHit hit;

        //we find the collider centre point
        m_colliderCenterPoint = m_colliderTransform.TransformPoint(wheelCollider.center);

        //we use the raycast to align the wheels with the ground, if the distance is longer than the suspension distance
        //we align the wheel to maximum suspension distance
        if (Physics.Raycast(m_colliderCenterPoint, -wheelCollider.transform.up, out hit, wheelCollider.suspensionDistance + wheelCollider.radius, m_layerMask))
        {
            transform.position = hit.point + (wheelCollider.transform.up * wheelCollider.radius);
        }
        else
        {
            transform.position = m_colliderCenterPoint - (wheelCollider.transform.up * wheelCollider.suspensionDistance);
        }

        //we use the wheel roation with the value of the steering
        transform.rotation = wheelCollider.transform.rotation * Quaternion.Euler(m_rotationValue, wheelCollider.steerAngle, 0);

        // we match the wheel rotation to the wheel collider rotation - degrees per second
        m_rotationValue += wheelCollider.rpm * (360 / 60) * Time.deltaTime;

        //we grab all the information from GetGroundHit in order to create instances for skidding effect
        WheelHit correspondingGroundHit;
        wheelCollider.GetGroundHit(out correspondingGroundHit);

        //i'm using an arbitrary value here, feel free to experiment and make consistent to all wheels
        if (Mathf.Abs(correspondingGroundHit.sidewaysSlip) > 1.5)
        {
            if (slipPrefab)
            {
                Instantiate(slipPrefab, correspondingGroundHit.point, Quaternion.identity);
            }
        }
    }
}
