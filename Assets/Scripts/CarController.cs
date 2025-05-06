using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Math;

// TODO:
// Add a down pressure force proportional to speed to prevent flip-overs if necessary

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbrakeForce;
    private bool isBraking;

    public static Transform carPosition;
    public static double carSpeed;

    private Vector3 _lastPosition;

    [SerializeField] public static double carHealth = 1000;

    // Car parameters
    [SerializeField] private float motorForce, brakeForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheel transforms
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    // Car gameobject
    [SerializeField] private GameObject playerCar;

    private void Start()
    {
        _lastPosition = playerCar.transform.position;
        carHealth = 1000;
    }

    private void FixedUpdate()
    {
        // Variable to display on the speedometer
        carSpeed = Math.Truncate((playerCar.transform.position - _lastPosition).magnitude * 1000);

        if (carHealth > 0)
            GetInput();
        else
        {
            verticalInput = 0;
            horizontalInput = 0;
            isBraking = false;
        }

        HandleMotor();
        HandleSteering();
        UpdateWheels();

        // Update position
        _lastPosition = playerCar.transform.position;

        // Reset scene
        if (Input.GetKey(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ApplyDownPressure()
    {

    }

    // This method gets entered whenever a collision (coming into contact with another rigidbody) occurs
    private void OnCollisionEnter(Collision collision)
    {
        // Simple function of damage based on the other collider's mass. Tweak and expand upon later.
        carHealth -= (collision.rigidbody.mass * carSpeed)/5000;
    }

    // Simple wrapper function around basic Unity input system functions.
    // Improve later.
    private void GetInput()
    {
        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        // Acceleration Input
        verticalInput = Input.GetAxis("Vertical");

        // Breaking Input
        isBraking = Input.GetKey(KeyCode.Space);
    }

    // Basic function to apply motor forces to the wheel colliders when accelerating
    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbrakeForce = isBraking ? brakeForce : 0f;
        ApplyBraking();
    }

    // Basic function to apply motor forces to the wheel colliders when braking
    private void ApplyBraking()
    {
        frontRightWheelCollider.brakeTorque = currentbrakeForce;
        frontLeftWheelCollider.brakeTorque = currentbrakeForce;
        rearLeftWheelCollider.brakeTorque = currentbrakeForce;
        rearRightWheelCollider.brakeTorque = currentbrakeForce;
    }

    // Basic function to apply angle-based steering to the wheels
    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    // Wrapper around the UpdateSingleWheel function to handle each wheel individually (could be done better)
    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    // Simple function to visibly update the rotation of the wheels when turning
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}