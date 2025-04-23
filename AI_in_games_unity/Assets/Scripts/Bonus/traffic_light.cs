using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class traffic_light : MonoBehaviour
{
    private Material[] material;
    public enum traffic_states {red, orange, green};
    public traffic_states current_state;
    private IDictionary<string, Color> traffic_colors;

    private float switch_time, last_time;

    // Start is called before the first frame update
    void Start()
    {
        switch_time = 5f;

        traffic_colors = new Dictionary<string, Color>(){
            {"green", Color.green},
            {"orange", new Color(1f,0.5f,0f,1f)},
            {"red", Color.red},
        };
        material = new Material[2] {
            this.transform.GetChild(0).GetComponent<Renderer>().material,
            this.transform.GetChild(1).GetComponent<Renderer>().material
            };
        current_state = traffic_states.green;
        foreach(Material mat in material)
        {
            mat.color = traffic_colors["green"];
        }
        last_time = Time.time;
    }

    void state_machine()
    {
        switch(current_state)
        {
            case traffic_states.green:
               foreach(Material mat in material)
                {
                    mat.color = traffic_colors["orange"];
                }
                current_state = traffic_states.orange;
                break;

            case traffic_states.orange:
                foreach(Material mat in material)
                {
                    mat.color = traffic_colors["red"];
                }
                current_state = traffic_states.red;
                break;

            case traffic_states.red:
                foreach(Material mat in material)
                {
                    mat.color = traffic_colors["green"];
                }
                current_state = traffic_states.green;
                break;

            default:
                foreach(Material mat in material)
                {
                    mat.color = traffic_colors["green"];
                }
                current_state = traffic_states.green;
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - last_time >= switch_time)
        {
            last_time = Time.time;
            state_machine();
        } 
    }
}
