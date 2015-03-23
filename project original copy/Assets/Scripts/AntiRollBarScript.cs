//---------------------------------------------------------------------------
//code created by Marius Varga for ISS at Plymouth university
//---------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

public class AntiRollBarScript : MonoBehaviour 
{
    // Coeefficient determining how much force is transfered by the bar.
    public float forceCoefficient = 10000.0f;
    public WheelCollider wheelFrontLeft;
    public WheelCollider wheelFrontRight;
    public WheelCollider wheelBackLeft;
    public WheelCollider wheelBackRight;

    private bool m_frontLeftGrounded;
    private bool m_frontRightGrounded;
    private bool m_backLeftGrounded;
    private bool m_backRightGrounded;

    private float m_travelFrontLeft = 1.0f;
    private float m_travelFrontRight = 1.0f;
    private float m_travelBackLeft = 1.0f;
    private float m_travelBackRight = 1.0f;

    void FixedUpdate()
    {

        //front axle
        //-----------------------------------------------------------------------
        //we get the collision for left front wheel
        WheelHit hitLeftFront;
        m_travelFrontLeft = 1.0f;

        m_frontLeftGrounded = wheelFrontLeft.GetGroundHit(out hitLeftFront);

        //if we have collision we calculate the travelling distance for the front left wheel
        if (m_frontLeftGrounded)
        {
            m_travelFrontLeft = (-wheelFrontLeft.transform.InverseTransformPoint(hitLeftFront.point).y - wheelFrontLeft.radius)
                     / wheelFrontLeft.suspensionDistance;
        }

        //we get the collision for right front wheel
        WheelHit hitRightFront;
        m_travelFrontRight = 1.0f;

        m_frontRightGrounded = wheelFrontRight.GetGroundHit(out hitRightFront);

        //if we have collision we calculate the travelling distance for the front right wheel
        if (m_frontRightGrounded)
        {
            m_travelFrontRight = (-wheelFrontRight.transform.InverseTransformPoint(hitRightFront.point).y - wheelFrontRight.radius)
                     / wheelFrontRight.suspensionDistance;
        }

        //here we calculate the force to be applied to the wheels, this is done per axle
        //here we deal with the front axle
        var antiRollFrontForce = (m_travelFrontLeft - m_travelFrontRight) * forceCoefficient;

        //if the left wheel is grounded we apply the force
        if (m_frontLeftGrounded)
            rigidbody.AddForceAtPosition(wheelFrontLeft.transform.up * antiRollFrontForce, wheelFrontLeft.transform.position);

        //if the right wheel is grounded we apply the force 
        if (m_frontRightGrounded)
            rigidbody.AddForceAtPosition(wheelFrontRight.transform.up * -antiRollFrontForce, wheelFrontRight.transform.position); 
        //-----------------------------------------------------------------------------------------


        //back axle
        //-----------------------------------------------------------------------
        //we get the collision for left back wheel
        WheelHit hitLeftBack;
        m_travelBackLeft = 1.0f;

        m_backLeftGrounded = wheelBackLeft.GetGroundHit(out hitLeftBack);

        //if we have collision we calculate the travelling distance for the back left wheel
        if (m_backLeftGrounded)
        {
            m_travelBackLeft = (-wheelBackLeft.transform.InverseTransformPoint(hitLeftBack.point).y - wheelBackLeft.radius)
                     / wheelBackLeft.suspensionDistance;
        }

        //we get the collision for right back wheel
        WheelHit hitRightBack;
        m_travelBackRight = 1.0f;

        m_frontRightGrounded = wheelBackRight.GetGroundHit(out hitRightBack);

        //if we have collision we calculate the travelling distance for the back right wheel
        if (m_frontRightGrounded)
        {
            m_travelBackRight = (-wheelBackRight.transform.InverseTransformPoint(hitRightBack.point).y - wheelBackRight.radius)
                     / wheelBackRight.suspensionDistance;
        }

        //here we calculate the force to be applied to the wheels, this is done per axle
        //here we deal with the back axle
        var antiRollBackForce = (m_travelBackLeft - m_travelBackRight) * forceCoefficient;

        //if the left back wheel is grounded we apply the force
        if (m_backLeftGrounded)
            rigidbody.AddForceAtPosition(wheelBackLeft.transform.up * antiRollBackForce, wheelBackLeft.transform.position);

        //if the right back wheel is grounded we apply the force 
        if (m_frontRightGrounded)
            rigidbody.AddForceAtPosition(wheelBackRight.transform.up * -antiRollBackForce, wheelBackRight.transform.position);
        //-----------------------------------------------------------------------------------------
    }
}
