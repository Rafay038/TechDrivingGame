using UnityEngine;

public class CarController : MonoBehaviour
{
    public float maxMotorTorque = 1500f;  // acceleration
    public float maxSteerAngle = 30f;     // steering
    public float brakeTorque = 3000f;     // braking force

    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;

    public Transform wheelFLTransform;
    public Transform wheelFRTransform;
    public Transform wheelRLTransform;
    public Transform wheelRRTransform;

    void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("throttle");
        float brake = Input.GetAxis("brake") > 0.1f ? brakeTorque : 0f;
        float steering = maxSteerAngle * Input.GetAxis("steer");

        // Apply motor and steering
        wheelFL.steerAngle = steering;
        wheelFR.steerAngle = steering;

        wheelRL.motorTorque = motor;
        wheelRR.motorTorque = motor;

        // Apply brake
        wheelFL.brakeTorque = brake;
        wheelFR.brakeTorque = brake;
        wheelRL.brakeTorque = brake;
        wheelRR.brakeTorque = brake;

        UpdateWheelPose(wheelFL, wheelFLTransform);
        UpdateWheelPose(wheelFR, wheelFRTransform);
        UpdateWheelPose(wheelRL, wheelRLTransform);
        UpdateWheelPose(wheelRR, wheelRRTransform);
    }

    void UpdateWheelPose(WheelCollider collider, Transform trans)
    {
        Vector3 pos;
        Quaternion quat;
        collider.GetWorldPose(out pos, out quat);
        trans.position = pos;
        trans.rotation = quat;
    }
}
