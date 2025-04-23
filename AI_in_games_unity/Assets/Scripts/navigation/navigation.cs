using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class navigation : MonoBehaviour
{
    // Gives a visualisation of the path found by the algorithm
    [SerializeField] private bool debug_calculate_navigation;

    // Graph objects
    public GameObject start_point, end_point;
    public List<GameObject> path;
    private NavigationGraph game_graph;
    [HideInInspector] public List<GameObject> roads;
    [HideInInspector] public GameObject active_target;
    private bool is_init_done;
 
    // Start is called before the first frame update
    void Start()
    {
        // Initialisation
        debug_calculate_navigation = false;
        path = new List<GameObject>();
        is_init_done = false;
        active_target = null;

        // Get all road objects in the scene
        foreach(Transform child in this.transform.parent.GetComponentsInChildren<Transform>())
        {
            if(child.tag == "Road")
            {
                roads.Add(child.gameObject);
            }
        }
        start_point = end_point = roads[0];
    }

    
    private class Node
    {
        public int ID; // Node ID
        public GameObject road; // GameObject symbolising the node
        public Node previous_node, next_node; 
        public List<GameObject> edges; // List symbolising the edges (connected roads) of the graph
        public bool is_start; // Is this node the root node?
        public bool is_target; // Is this node the goal node?
        public bool already_visited; // Was this node visited?

        /// <summary>
        /// Node constructor.
        /// The parameters are initialised
        /// Set the roads connected to this node.
        /// </summary>
        /// <param name="ID">ID of this node</param>
        /// <param name="previous_node">Parent node of this node</param>
        /// <param name="road">Road gameObject linked to this node</param>
        /// <param name="start_point">Start node of the graph</param>
        /// <param name="end_point">Target node of the graph</param>
        public Node(int ID ,Node previous_node, GameObject road, GameObject start_point, GameObject end_point)
        {
            this.ID = ID;
            this.road = road;
            this.previous_node = previous_node;
            this.next_node = null;
            this.is_start = ReferenceEquals(this.road, start_point);
            this.is_target = ReferenceEquals(this.road, end_point);
            this.already_visited = false;
            this.edges = new List<GameObject>();
            
            road_node road_script = road.GetComponentInChildren<road_node>();
            foreach(GameObject connect in road_script.connected_road)
            {
                edges.Add(connect.transform.parent.gameObject);
            }

        }

        /// <summary>
        /// Set the "already_visited" parameter to True.
        /// </summary>
        public void set_visited()
        {
            this.already_visited = true;
        }
    }


    private class NavigationGraph
    {
        public List<Node> graph, path;
        public Node current_node;
        public int nodes_left;
        private GameObject start_point, end_point;
        private bool found_path;

        /// <summary>
        /// Navigation graph constructor.
        /// Initialised the parameters.
        /// </summary>
        /// <param name="start_point">Start road object</param>
        /// <param name="end_point">Target road object</param>
        public NavigationGraph(List<GameObject> roads, GameObject start_point, GameObject end_point)
        {
            this.graph = new List<Node>();
            this.start_point = start_point;
            this.end_point = end_point;
            this.nodes_left = 0;
            this.found_path = false;


            int id = 0;
            foreach(GameObject r in roads)
            {
                graph.Add(new Node(id, null, r, start_point, end_point));
                id++;
            }          
            
        }     

        /// <summary>
        /// Call the pathfinding algorithm between the start node and the target node.
        /// The found path is assigned the "path" intern variable. 
        /// The number of nodes in the path is also assigned to the "node_left" variable
        /// </summary>
        /// <returns>Bool: True if a path was found. False otherwise</returns>
        public bool findPath()
        {
            List<Node> nodes = graph;
            List<int[]> edges = new List<int[]>();

            //Creation of the edge object
            foreach(Node n in nodes)
            {
                foreach(GameObject e in n.edges)
                {
                    foreach(Node node_road in nodes)
                    {
                        if(ReferenceEquals(e, node_road.road))
                        {
                            edges.Add(new int[2]{n.ID, node_road.ID});
                            break;
                        }
                    }
                }
            }

            path = pathfinding_algorithm(nodes, edges);
            for(int i=0; i<(path.Count-1); i++) 
            {
                //We set the next nodes for each nodes of the path as it is required to iterate the path.
                path[i].next_node = path[i+1];
            }
            current_node = path[0];
            nodes_left = path.Count;
            found_path = path.Count > 0;
            return this.found_path;
        }
        

        /// <summary>
        /// Implement the pathfinding algorithm from a root vertex to a goal vertex.
        /// Check the is_start and is_target flags in the nodes.
        /// You can use the ID parameter of the nodes to store track
        /// </summary>
        /// <param name="nodes">List of the nodes in the graph. 
        /// It is represented as {Node0, Node1, Node2, ...}</param>
        /// <param name="edges">List of the edges in the graph.
        /// It is represented as as list of arrays:
        /// { [Node0,Node1], [Node0,Node2], [Node1,Node2] }</param>
        /// <returns>Return a list of ordered nodes representing the path between the root and goal vertices.</returns>
        protected List<Node> pathfinding_algorithm(List<Node> nodes, List<int[]> edges)
        {
            List<Node> optimal_path = new List<Node>();
            Node current_node = null;
            foreach(Node n in nodes)
            {
                if (n.is_start)
                {
                    current_node = n;
                    break;
                }
            }

            // TODO
            // From the list of search algorithms seen during the course,
            // implement one so that you can create the navigation system for the car agent.
            // Don't forget to use Node.ID to track visited nodes. 

            return optimal_path;
        }

        /// <summary>
        /// Find all connected nodes of a given node.
        /// </summary>
        /// <param name="id">ID of the root node</param>
        /// <param name="edges">Edges of the entire graph</param>
        /// <returns>Returns a list of all connected nodes from the root node</returns>
        private List<int> get_neighbors(int id, List<int[]> edges)
        {
            List<int> neighbors = new List<int>();
            foreach(int[] e in edges)
            {
                if(e[0] == id)
                {
                    neighbors.Add(e[1]);
                }
            }
            return neighbors;
        }

        /// <summary>
        /// Verify if a node was already visited
        /// </summary>
        /// <param name="visited">List of visited node ID</param>
        /// <param name="id">ID of the node to verify</param>
        /// <returns>True if already visited, false otherwise</returns>
        private bool was_visited(List<int> visited, int id)
        {
            foreach(int i in visited)
            {
                if(i == id)
                {
                    return true;
                }
            }
            return false;
        }
    }

    
    /// <summary>
    /// Start the navigation system in game using "game_graph" global variable.
    /// A navigationGraph object is created, and the find_path() method is called to create the path.
    /// </summary>
    /// <param name="start_point">Root road GameObject</param>
    /// <param name="end_point">Goal road GameObject</param>
    /// <returns>Return 0 if a path is found, -1 otherwise</returns>
    public int activate_navigation(GameObject start_point, GameObject end_point)
    {
        if(!is_init_done)
        {return -1;}
        game_graph = new NavigationGraph(roads,start_point,end_point);
        if(game_graph.findPath())
        {
            //path is used to visualise the path created
            path.Clear();
            foreach(Node n in game_graph.path)
            {
                path.Add(n.road.gameObject);
            }
            active_target = game_graph.current_node.road.transform.Find("Target Road").gameObject;
            activate_only_current_target();
            return 0;
        }
        return -1;
    }

    /// <summary>
    /// Activate the next node on the route.
    /// </summary>
    /// <returns> Returns the number of node left in the route </returns>
    public int activate_next_node()
    {
        if(!is_init_done)
        {
            return -1;
        }

        if(game_graph.current_node.next_node != null)
        {
            game_graph.current_node = game_graph.current_node.next_node;
            active_target = game_graph.current_node.road.transform.Find("Target Road").gameObject;
            activate_only_current_target();
        }
        if(game_graph.nodes_left>0)
        {
            game_graph.nodes_left--;
        }
        
        return game_graph.nodes_left;
    }

    /// <summary>
    /// Coroutine that animates the path animation.
    /// This allows to see the road taken from a start point to a target point.
    /// The target object of each roads are activated sequencelly following the path.
    /// </summary>
    /// <param name="path">List of roads in the used in the navigation</param>
    /// <returns></returns>
    IEnumerator path_animation(List<GameObject> path)
    {
        GameObject car_agent = null;
        try{ car_agent = GameObject.Find("Car Agent Discrete Navigation"); }
        catch{ Debug.LogWarning("Car agent not found"); }

        foreach(GameObject road in roads)
        {
            road.transform.Find("Target Road").gameObject.SetActive(false);  
        }
        car_agent.SetActive(false);

        foreach(GameObject road in path)
        {
            road.transform.Find("Target Road").gameObject.SetActive(true);  
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(3f);

        foreach(GameObject road in path)
        {
            road.transform.Find("Target Road").gameObject.SetActive(false);
        }

        car_agent.SetActive(true);
    }

    /// <summary>
    /// Deactivate all target object in the roads GameObject.
    /// Then only the current target given to the agent is activated.
    /// It is used to update checkpoints when the agent reach a destination.
    /// </summary>
    private void activate_only_current_target()
    {
        foreach(GameObject r in roads)
        { 
            bool is_to_activate = Object.ReferenceEquals(game_graph.current_node.road, r);
            r.transform.Find("Target Road").gameObject.SetActive(is_to_activate);
        }
    }

    /// <summary>
    /// Randomly select a start road GameObject and a target road GameObject.
    /// By doing so, it is possible to randomly start a new navigation.
    /// </summary>
    public void set_random_trip()
    {
        int safeguard = 0;
        int start_indice, target_indice;
        start_indice = Random.Range(0,roads.Count);
        target_indice = Random.Range(0,roads.Count);
        while(target_indice == start_indice)
        {
            target_indice = Random.Range(0,roads.Count);

            if(safeguard > 100){break;}
            safeguard++;
        }
        start_point = roads[start_indice];
        end_point = roads[target_indice];
    }

    /// <summary>
    /// Calculate a path from a random navigation, and then display it with "path_animation" coroutine.
    /// This function can be activated by setting the bool "debug_calculate_navigation" in the inspector.
    /// </summary>
    private void debug_display_random_trip()
    {
        set_random_trip();
        game_graph = new NavigationGraph(roads, start_point,end_point);
        if(game_graph.findPath())
        {
            debug_calculate_navigation = false;
            path = new List<GameObject>();
            
            foreach(Node n in game_graph.path)
            {
                path.Add(n.road.gameObject);
            }
            StartCoroutine(path_animation(path));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!is_init_done && start_point.GetComponentInChildren<road_node>().connected_road.Count>0)
        {
            is_init_done = true;
        }

        if(debug_calculate_navigation && is_init_done)
        {
            debug_display_random_trip();
        }
    }
}
