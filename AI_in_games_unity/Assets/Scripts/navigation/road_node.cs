using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class road_node : MonoBehaviour
{
    //connected_road variable is to set in the editor
    public List<GameObject> connected_road;

    // Start is called before the first frame update
    void Start()
    {
        connected_road = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Navigation_node")
        {
            connected_road.Add(other.gameObject.transform.parent.gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
