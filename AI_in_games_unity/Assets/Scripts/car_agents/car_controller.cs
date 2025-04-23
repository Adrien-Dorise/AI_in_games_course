using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Car controller taken from GameDevChef
/// https://github.com/GameDevChef/CarController
/// </summary>
public class car_controller : MonoBehaviour
{

    public bool isAgent;
    private int wheel_number;

    public float horizontalInput;
    public float verticalInput;
    public bool isBreaking;

    private float currentSteerAngle;
    private float currentbreakForce;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    private Transform[] wheels = new Transform[4];
    private WheelCollider[] wheelColliders = new WheelCollider[4];
    
    private void Awake()
    {
        wheel_number = 4;
        isAgent = false;
        for(int i=0; i<wheel_number; i++)
        {
            wheels[i] = this.transform.GetChild(1).GetChild(i);
            wheelColliders[i] = this.transform.GetChild(2).GetChild(i).GetComponent<WheelCollider>();
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
    }
    

    private void HandleMotor()
    {
        wheelColliders[1].motorTorque = verticalInput * motorForce;
        wheelColliders[3].motorTorque = verticalInput * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();       
    }

    private void ApplyBreaking()
    {
        foreach(WheelCollider wheelColl in wheelColliders)
        {
            wheelColl.brakeTorque = currentbreakForce;
        }
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        wheelColliders[1].steerAngle = currentSteerAngle;
        wheelColliders[3].steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        for(int i=0; i<wheel_number; i++)
        {
            UpdateSingleWheel(wheelColliders[i], wheels[i]);
        }
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot * Quaternion.Euler(new Vector3(0, -90, 0));;
        wheelTransform.position = pos;
    }

    private void FixedUpdate()
    {
        if(!isAgent)
        {
            GetInput();
        }
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }
}
