using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class car_agent_template_traffic_navigation : car_agent
{
    // Initialisation
    private navigation_correction nav_script;
    private enum situationals {none, traffic_light};
    private situationals current_situational;
    private GameObject situational_object;


    /// <summary>
    /// Call back of the Unity Start() function.
    /// It is called at the instantiation of a car_agent game object.
    /// In this project, the base Start() function is used to instantiate car_agent variables.
    /// </summary>
    protected override void _start()
    {
        nav_script = this.transform.parent.GetComponentInChildren<navigation_correction>();
        situational_object = null;
    }

    /// <summary>
    /// Call back of the mlagent OnEpisodeBegin() function.
    /// In this project, the base OnEpisodeBegin() function is used to initialise car agent 
    /// and target positions.
    /// </summary>
    protected override void _onEpisodeBegin()
    {
        //Here we initialse a start point and a goal point and activate the pathfinding algorithm.
        nav_script.set_random_trip();
        bool is_navigation_initialised = nav_script.activate_navigation(nav_script.start_point, nav_script.end_point) != -1;
        if(!is_navigation_initialised)
        {
            EndEpisode();
        }
    }


    /// <summary>
    /// Find and return the target to be used at the start of an episode.
    /// It is called in the base OnEpisodeBegin() function.
    /// </summary>
    /// <returns>Return the transform of the wanted target.</returns>
    protected override Transform _getTarget()
    {
        return nav_script.active_target.transform.GetChild(1); 
    }

    /// <summary>
    /// Find and return the "start" object to be used to initialise the car agent position
    /// at the start of an episode.
    /// It is called in the base OnEpisodeBegin() function.
    /// </summary>
    /// <returns>Return the transform of the wanted "start" object.</returns>
    protected override Transform _getStart()
    {
        Transform startPos =  nav_script.active_target.transform.GetChild(0);
        bool is_next_path_null = nav_script.activate_next_node() == 0;
        if(is_next_path_null)
        {
            EndEpisode();
        }
        return startPos;
    }

    /// <summary>
    /// Regroup all observations used by the car agent during training.
    /// Use "sensor.AddObservation(obs)" to add an observation.
    /// </summary>
    /// <param name="sensor">Vector listing all the agent observations</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target direction
        //We use a try/catch in the event that path is not found yet and no target is assigned which would
        // throw an error.
        try
        {
            sensor.AddObservation(distanceVector(this.transform, target)); //3 observations
        }
        catch
        {
            sensor.AddObservation(distanceVector(this.transform, this.transform)); //3 observations
            Debug.LogWarning("No target position found. Replaced with null vector3");
        }

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);

        //Situational
        switch(current_situational)
        {
            case situationals.traffic_light:
                switch(situational_object.GetComponent<traffic_light>().current_state)
                {
                    case traffic_light.traffic_states.green:
                        sensor.AddObservation(1);
                        break;
                    case traffic_light.traffic_states.orange:
                        sensor.AddObservation(2);
                        break;
                    case traffic_light.traffic_states.red:
                        sensor.AddObservation(3);
                        break;
                }
                break;
            
            default:
                sensor.AddObservation(0);
                break;
        }
    }


    private float lastDistance;
    /// <summary>
    /// Set the rewards given to the agent during training.
    /// Use "AddReward(float)" or "SetReward(float)" to add reward goal.
    /// Note that rewards depending on collision are to be set in "_collisionRewards" or 
    /// "_triggerRewards"
    /// </summary>
    protected override void _fixRewards()
    {
        // Approached target
        float distanceToTarget = Vector3.Distance(this.transform.position, target.position);
        if(distanceToTarget < lastDistance)
        {
            AddReward(0.00001f);
        }
        else
        {
            AddReward(-0.001f);
        }

        // Traffic light
        float current_speed = Mathf.Abs(rBody.velocity.x) + Mathf.Abs(rBody.velocity.z);
        switch(current_situational)
        {
            case situationals.traffic_light:
                switch(situational_object.GetComponent<traffic_light>().current_state)
                {
                    case traffic_light.traffic_states.green:
                        if(current_speed <= 5e-2)
                        {
                            SetReward(-0.2f);
                        }
                        
                        break;
                    case traffic_light.traffic_states.orange:
                    case traffic_light.traffic_states.red:
                        if(current_speed <= 5e-2)
                        {
                            SetReward(0.1f);
                        }
                        else
                        {
                            SetReward(-0.2f);
                        }
                        break;
                }
                break;
        }


        lastDistance = distanceToTarget;
    }

        /// <summary>
    /// Set the rewards resulting to a collision with an external hitbox.
    /// Use "AddReward(float)" or "SetReward(float)" to add reward goal.
    /// </summary>
    /// <param name="tag">Tag of the object colliding with the agent</param>
    /// <param name="collision">Hitbox of the colliding object</param>
    protected override void _collisionRewards(string tag, Collision collision)
    {
        if(tag == "Death")
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    /// <summary>
    /// Set the rewards resulting to a collision with an external trigger hitbox.
    /// Use "AddReward(float)" or "SetReward(float)" to add reward goal.
    /// You can set different behaviour whether the agent is inside or leaving the hitbox.
    /// Note that when an agent is inside a trigger hitbox, this function is called every frame.
    /// </summary>
    /// <param name="tag">Tag of the object colliding with the agent</param>
    /// <param name="is_inside">Set to true when the agent is inside the trigger hitbox, and to false when it is leaving.</param>
    /// <param name="collider">Hitbox of the triggered object</param>
    protected override void _triggerRewards(string tag, bool is_inside, Collider collider)
    {
        if(is_inside)
        {
            if(tag == "Target")
            {
                SetReward(1.0f);

                // When the target is reached, we switch to the next target.
                if(nav_script.activate_next_node() == 0) //No nodes left
                {
                    EndEpisode();
                }
                try{
                target = nav_script.active_target.transform.GetChild(1);
                }
                catch{
                    Debug.LogWarning("No target available");
                }
            }
            else if(tag == "Traffic_light")
            {
                current_situational = situationals.traffic_light;
                situational_object = collider.transform.parent.gameObject;
            }
        }
        else
        {
            if(tag == "Traffic_light")
            {
                current_situational = situationals.none;
                situational_object = null;
            }
        }
    }

    /// <summary>
    /// Set the action to be executed by the agent when receiving the model outputs.
    /// You can interact with the car_controller script to link the model output to actions in the environment.
    /// The model output are stored in the actionbuffers.
    /// Outputs can be retrieved using "actionBuffers.DiscreteActions[int]" or "actionBuffers.ContinuousActions[int]
    /// /// </summary>
    /// <param name="actionBuffers">Output of the neural network model.</param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //Break
        if(actionBuffers.DiscreteActions[0] == 1)
        {
            car_script.isBreaking = true;
        }
        else if(actionBuffers.DiscreteActions[0] == 0)
        {
            car_script.isBreaking = false;
        }

        //Horizontal
        if(actionBuffers.DiscreteActions[1] == 0)
        {
            car_script.horizontalInput = 0;
        }
        else if(actionBuffers.DiscreteActions[1] == 1)
        {
            car_script.horizontalInput = 1;
        }
        else if(actionBuffers.DiscreteActions[1] == 2)
        {
            car_script.horizontalInput = -1;
        }

        //Vertical
        if(actionBuffers.DiscreteActions[2] == 0)
        {
            car_script.verticalInput = 0;
        }
        else if(actionBuffers.DiscreteActions[2] == 1)
        {
            car_script.verticalInput = 1;
        }
        else if(actionBuffers.DiscreteActions[2] == 2)
        {
            car_script.verticalInput = -1;
        }

       _fixRewards();
    }

    
}

