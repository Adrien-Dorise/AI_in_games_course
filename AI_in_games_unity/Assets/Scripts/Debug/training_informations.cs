using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class training_informations : MonoBehaviour
{

    [SerializeField] GameObject car_agent;
    private car_controller car_script;
    private car_agent agent_script;
    private string debug_text;
    [SerializeField] List<float> observations = new List<float>();


    // Start is called before the first frame update
    void Start()
    {
        car_script = car_agent.GetComponent<car_controller>();
        agent_script = car_agent.GetComponent<car_agent>();
    }


    private void FixedUpdate()
    {
        debug_text = "Horizontal input = " + car_script.horizontalInput.ToString() +
                     "\nVertical input = " + car_script.verticalInput.ToString() +
                     "\nBreak = " +  car_script.isBreaking + 
                     "\nReward = " + agent_script.GetCumulativeReward().ToString() +
                     "\nStep = " + agent_script.StepCount.ToString() +
                     "\nEpisode = " + agent_script.CompletedEpisodes.ToString();

        this.GetComponent<TextMesh>().text = debug_text;
        observations.Clear();
        foreach (float obs in agent_script.GetObservations())
        {
            observations.Add(obs);                    
        }
    }
}
