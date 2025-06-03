using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Math;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

// TODO:
// Add a down pressure force proportional to speed to prevent flip-overs if necessary

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbrakeForce;
    private bool isBraking;
    private bool isBoosting;
    public static bool isOnTheGround;

    public static Transform carPosition;
    public static float carSpeed;
    [SerializeField] private float carSpeedMax;
    public static int gearNum = 1;
    [SerializeField] public static float wheelsRPM;
    [SerializeField] public static float engineRPM;
    [SerializeField] public static float maxRPM = 1300;
    [SerializeField] public static float minRPM = 1000;
    public static bool isReverse = false;
    public static float totalPower;
    [SerializeField] private float[] gearChangeSpeed;
    private float radius = 0.6f;

    public static float nitrusValue;
    [SerializeField] public static float nitrusValueMax;
    [SerializeField] private float nitrusForce;
    public bool nitrusFlag = false;

    public float MaxSteerAngle
    {
        get => maxSteerAngle;
        set => maxSteerAngle = Mathf.Clamp(value, 0f, 90f);
    }

    public float MotorForce
    {
        get => motorForce;
        set => motorForce = Mathf.Max(0, value);
    }

    internal enum driveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }
    [SerializeField] private driveType drive;

    internal enum gearBox
    {
        automatic,
        manual
    }

    [SerializeField] private gearBox gearChange;


    private Vector3 _lastPosition;

    [SerializeField] public static double carHealthMax = 1000;
    [SerializeField] public static double carHealth;

    // Car parameters
    [SerializeField] private float motorForce, brakeForce, maxSteerAngle;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheel transforms
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private WheelCollider[] wheels;

    // Car gameobject
    [SerializeField] private GameObject playerCar;
    [SerializeField] private Rigidbody playerCarBody;
    [SerializeField] AnimationCurve enginePower;

    private void Start()
    {
        _lastPosition = playerCar.transform.position;
        carHealth = 1000;
        isBoosting = false;
        nitrusValueMax = 10;
        nitrusValue = nitrusValueMax;
        // This mess is to initialize the array properly without going through the hoops of upgrading the langver
        // I might end up doing it anyway, but I am afraid it'd break stuff
        //wheels[0] = frontLeftWheelCollider;
        //wheels[1] = frontRightWheelCollider;
        //wheels[2] = rearLeftWheelCollider;
        //wheels[3] = rearRightWheelCollider;
        isOnTheGround = true;
        StartCoroutine(timedLoop());
        carHealth = carHealthMax;
    }

    private void FixedUpdate()
    {
        // Variable to display on the speedometer
        carSpeed = playerCarBody.linearVelocity.magnitude;

        // This is where I'd put the down force method if and when it's needed
        //applyDownForce();

        UpdateWheels();
        HandleSteering();
        CalculateEnginePower();

        if (carHealth > 0)
            GetInput();
        else
        {
            verticalInput = 0;
            horizontalInput = 0;
            isBraking = false;
        }
        

        // Update position
        _lastPosition = playerCar.transform.position;

        // Reset scene
        if (Input.GetKey(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // This method might be added later in case the car flips out too easily
    private void ApplyDownForce()
    {

    }

    private bool checkGears()
    {
        if (gearNum == 0) return true;
        else if (carSpeed >= gearChangeSpeed[gearNum]) return true;
        else return false;
    }

    private void CalculateEnginePower()
    {
        WheelRPM();

        if (verticalInput != 0)
            playerCarBody.linearDamping = 0.005f;
        if (verticalInput == 0)
            playerCarBody.linearDamping = 0.1f;

        totalPower = 3.6f * enginePower.Evaluate(carSpeed) * verticalInput * 1000000;


        isOnTheGround = IsGrounded();

        if (isOnTheGround)
            HandleMotor();

        ShiftGear();

        // This should be moved to the input manager in due time
        if (Input.GetKey(KeyCode.P))
            isBoosting = true;
        else
            isBoosting = false;

        ActivateNitrus();
    }

    private void WheelRPM()
    {
        float sum = 0;
        int R = 0;

        /*
        for (int i = 0; i < 4; i++)
        {
            sum = wheels[i].rpm;
            R++;
        }
        */

        sum += frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm + rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm;

        wheelsRPM = (R != 0F) ? sum / R : 0;

        if (wheelsRPM < 0 && !isReverse)
        {
            isReverse = true;
        }
        else if (wheelsRPM > 0 && isReverse)
        {
            isReverse = false;
        }
    }

    private bool IsGrounded()
    {
        //if (wheels[0].isGrounded && wheels[1].isGrounded && wheels[2].isGrounded && wheels[3].isGrounded)
        if (frontLeftWheelCollider.isGrounded && frontRightWheelCollider.isGrounded && rearLeftWheelCollider.isGrounded && rearRightWheelCollider.isGrounded)
            return true;
        else
            return false;
    }

    private void ShiftGear()
    {
        // Manual gear shifting
        if (gearChange == gearBox.manual)
        {
            if (Input.GetKeyDown(KeyCode.Q) && gearNum < gearChangeSpeed.Length && checkGears())
                gearNum++;
            else if (Input.GetKeyDown(KeyCode.E) && (gearNum - 1) > 0)
                gearNum--;
        }
        // Automatic gear shifting
        else
        {
            if (carSpeed < gearChangeSpeed[gearNum] && (gearNum - 1) > 0)
                gearNum--;
            else if (carSpeed > gearChangeSpeed[gearNum] && !isReverse && checkGears() && (gearNum + 1 < gearChangeSpeed.Length))
                gearNum++;
        }
            
    }

    // This method gets entered whenever a collision (coming into contact with another rigidbody) occurs
    private void OnCollisionEnter(Collision collision)
    {
        if ( !(collision.collider.GetType() == typeof(TerrainCollider)))
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
        if (!isBraking && gearNum != 0 && carSpeed < carSpeedMax)
        {
            if (drive == driveType.allWheelDrive)
            {
                /*
                for (int i = 0; i < wheels.Length; i++)
                {
                    wheels[i].motorTorque = totalPower / 4;
                }
                */
                if (verticalInput != 0)
                {
                    frontLeftWheelCollider.motorTorque = totalPower / 4;
                    frontRightWheelCollider.motorTorque = totalPower / 4;
                    rearLeftWheelCollider.motorTorque = totalPower / 4;
                    rearRightWheelCollider.motorTorque = totalPower / 4;
                }
                else
                {
                    frontLeftWheelCollider.motorTorque = 0;
                    frontRightWheelCollider.motorTorque = 0;
                    rearLeftWheelCollider.motorTorque = 0;
                    rearRightWheelCollider.motorTorque = 0;
                }
                
            }
            else if (drive == driveType.rearWheelDrive)
            {
                if (verticalInput != 0)
                {
                    rearRightWheelCollider.motorTorque = totalPower / 4;
                    rearLeftWheelCollider.motorTorque = totalPower / 4;
                }
                else
                {
                    frontLeftWheelCollider.motorTorque = 0;
                    frontRightWheelCollider.motorTorque = 0;
                    rearLeftWheelCollider.motorTorque = 0;
                    rearRightWheelCollider.motorTorque = 0;
                }
            }
            // Front wheel drive
            else
            {
                if (verticalInput != 0)
                {
                    frontLeftWheelCollider.motorTorque = totalPower / 4;
                    frontRightWheelCollider.motorTorque = totalPower / 4;
                }
                else
                {
                    frontLeftWheelCollider.motorTorque = 0;
                    frontRightWheelCollider.motorTorque = 0;
                    rearLeftWheelCollider.motorTorque = 0;
                    rearRightWheelCollider.motorTorque = 0;
                }
            }
        }

        /*
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        
        */

        ApplyBraking();
    }

    // Basic function to apply motor forces to the wheel colliders when braking
    private void ApplyBraking()
    {
        currentbrakeForce = isBraking ? brakeForce : 0f;

        frontRightWheelCollider.brakeTorque = currentbrakeForce;
        frontLeftWheelCollider.brakeTorque = currentbrakeForce;
        rearLeftWheelCollider.brakeTorque = currentbrakeForce;
        rearRightWheelCollider.brakeTorque = currentbrakeForce;

        //If the car reaches maximum speed, limit it by applying braking
        if (carSpeed >= carSpeedMax && !isBoosting)
        {
            rearRightWheelCollider.brakeTorque = brakeForce;
            rearLeftWheelCollider.brakeTorque = brakeForce;
            frontLeftWheelCollider.brakeTorque = brakeForce;
            frontRightWheelCollider.brakeTorque = brakeForce;
        }
    }

    // Basic function to apply angle-based steering to the wheels
    private void HandleSteering()
    {
        /*
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
        */

        if (horizontalInput > 0)
        {
            //rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontalInput;
        }
        else if (horizontalInput < 0)
        {
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontalInput;
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
            //transform.Rotate(Vector3.up * steerHelping);

        }
        else
        {
            frontLeftWheelCollider.steerAngle = 0;
            frontRightWheelCollider.steerAngle = 0;
        }
    }

    private IEnumerator timedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            radius = 6 + carSpeed / 20;

        }
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

    // Simple nitrus boosting function
    public void ActivateNitrus()
    {
        if (nitrusFlag && gearNum != 0)
        {
            // At the moment I'm making the nitrus charge over time since we don't have pickups yet
            if (!isBoosting && nitrusValue <= 10)
            {
                nitrusValue += Time.deltaTime / 2;
                if (nitrusValue >= nitrusValueMax)
                    nitrusValue = nitrusValueMax;
            }
            else
            {
                nitrusValue -= (nitrusValue <= 0) ? 0 : Time.deltaTime;
            }

            if (isBoosting)
            {
                if (nitrusValue > 0 && gearNum != 0)
                {
                    playerCarBody.AddForce(transform.forward * nitrusForce);
                }
            }
        } 
        else return;
    }
}