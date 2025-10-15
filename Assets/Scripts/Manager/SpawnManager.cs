
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SpawnManager : MonoBehaviour
{
    Vector3 tower_positon = new Vector3(0.0f, 50.0f, 0.0f);

    public Transform spawn_1;

    public Transform spawn_2;

    public Transform spawn_3;

    public GameObject tower;

    public GameObject enemy_spawn;

    RaycastHit rit;

    public Transform[] path_1;

    public Transform[] path_2;

    public Transform[] path_3;

    public GameObject defender;

    public GameObject hover_defender;

    public GameObject BOOM_defender;

    public Transform[] defender_locations;

    [SerializeField]
    List<int> spawned = new List<int>();

    

    int attempt = 0;

    Grid spawn_grid;

    public int grid_size = 16;

    public GameObject grid_node_visual;

    float spawn_time = 5f;

    float spawn_interval_min = 10f;

    float spawn_interval_max = 15f;

    EnemySpawner[] all_spawns;

    public int node_distance = 4;

    [SerializeField]
    float nuke_countdown = 180f;

    public GameObject nuke;

    bool victory = false;

    float victory_screen_timer = 10f;

    public GameObject victory_ui;

    int count_defender = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupTower();

        SetupGrid();

        PlaceSpawns();

    }

    private void Update()
    {
        SpawnInterval();

        NukeWin();


    }

    void NukeWin()
    {
        if (victory == true)
        {
            VictoryScreen();
        }

        if (victory == false)
        {
            nuke_countdown -= Time.deltaTime;

            if (nuke_countdown <= 0)
            {
                Instantiate(nuke, tower_positon, Quaternion.identity);

                nuke_countdown = 0;

                victory = true;
            }

        }

       
    }

    void VictoryScreen()
    {
        victory_screen_timer -= Time.deltaTime;

        victory_ui.SetActive(true);

        if (victory_screen_timer <= 0)
        {

            SceneManager.LoadScene("SampleScene");

        }
    }

    bool CheckProbability(int success_percentage)
    {
        int probability = Random.Range(0, 100);

        if (probability <= success_percentage)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    void UpdateSpawns ()
    {
        all_spawns = GetSpawners();

       
    }

    void SpawnInterval ()
    {
        spawn_time -= Time.deltaTime;

        if (spawn_time <= 0)
        {
            //Debug.Log("Initate Spawning");

            foreach(EnemySpawner spawn in all_spawns)
            {
                spawn.SpawnLogic();

                //Debug.Log("called spawn logic");
            }

            spawn_time = Random.Range(spawn_interval_min, spawn_interval_max);

        }

    }

    


    void SetupTower()
    {
        Ray raytower = new Ray(tower_positon, -Vector3.up);
        Debug.DrawRay(tower_positon, -Vector3.up);
        Physics.Raycast(raytower, out rit);
        tower_positon.y = rit.point.y;

        tower_positon.y -= 0.5f;

        Instantiate(tower, tower_positon, Quaternion.identity);

    }

    void SetupGrid()
    {
        spawn_grid = new Grid(grid_size, node_distance);
        //Debug.Log("Setup Grid");
    }

    void PlaceSpawns()
    {
        //Debug.Log("Placing spawns...");


        for (int x = 0; x < grid_size; x++)
        {
            for (int y = 0; y < grid_size; y++)
            {

                GridPoint current_node = spawn_grid.GetNode(x, y);

                Ray ray = new Ray(current_node.GetLocation(), -Vector3.up);

                Physics.Raycast(ray, out rit);

                current_node.SetNodeHeight(rit.point.y, 0.0f);

                GameObject node = Instantiate(enemy_spawn, current_node.GetLocation(), Quaternion.identity);

                EnemySpawner spawner = node.GetComponent<EnemySpawner>();

                switch (PickPath(current_node.GetLocation()))
                {
                    case "path_1": 
                        spawner.SetPath(path_1);
                        //Debug.Log("gave path info 1");
                        break;

                    case "path_2":
                        spawner.SetPath(path_2);
                        //Debug.Log("gave path info 2");
                        break;

                    case "path_3":
                        spawner.SetPath(path_3);
                        //Debug.Log("gave path info 3");
                        break;
                }

                spawner.SetSpawnProbability(current_node.GetPerlinWeightValue());

                //node.name = string.Format("node ({0},{1}) : weight = {2}", current_node.GetLocation().x, current_node.GetLocation().y, current_node.GetPerlinWeightValue());

                

            }
        }

        UpdateSpawns();

    }

    string PickPath(Vector3 spawn_location)
    {
        Vector3 path_1_location = new Vector3(path_1[0].position.x, 0.0f, path_1[0].position.z);

        Vector3 path_2_location = new Vector3(path_2[0].position.x, 0.0f, path_2[0].position.z);

        Vector3 path_3_location = new Vector3(path_3[0].position.x, 0.0f, path_3[0].position.z);

        Vector3 leveled_spawn_location = new Vector3(spawn_location.x, 0.0f, spawn_location.z);

        float path_1_distance = Vector3.Distance(leveled_spawn_location, path_1_location);
        float path_2_distance = Vector3.Distance(leveled_spawn_location, path_2_location);
        float path_3_distance = Vector3.Distance(leveled_spawn_location, path_3_location);


        float[] path_distances = { path_1_distance, path_2_distance, path_3_distance };

        int minIndex = 0;
        for (int i = 1; i < path_distances.Length; i++)
        {
            if (path_distances[i] < path_distances[minIndex])
            {
                minIndex = i;
            }
        }

        return "path_" + (minIndex + 1);
    }

    float range_distance(float a, float b)
    {
        return a - b;
    }

    float FindSmallestFloat(float[] floatArray)
    {
        float smallest = floatArray[0]; 

        for (int i = 1; i < floatArray.Length; i++) 
        {
            if (floatArray[i] < smallest) 
            {
                smallest = floatArray[i];
            }
        }

        return smallest; 
    }

    public void RemoveSpawnFromIndex(int index)
    {
        /*
        foreach (int spawn in spawned)
        {
            if (spawn == index)
            {
                spawned.RemoveAt(index);
            }
        }
        */
    }

    EnemySpawner[] GetSpawners()
    {
        GameObject[] spawn_objects = GameObject.FindGameObjectsWithTag("Spawn");

        EnemySpawner[] spawns = new EnemySpawner[spawn_objects.Length];

        for (int i = 0; i < spawn_objects.Length; i++)
        {
            spawns[i] = spawn_objects[i].GetComponent<EnemySpawner>();
        }

        return spawns;
    }

    void DefencePlacementChanged()
    {
        UpdateSpawns();

        foreach(EnemySpawner spawn in all_spawns)
        {
            spawn.UpdateDefencePlacement();
        }
    }

    public void SpawnDefender()
    {
        if (spawned.Count == defender_locations.Length)
        {
            return;
        }

        int picked_enemy_index = Random.Range(0, defender_locations.Length);

        foreach (int spawn in spawned)
        {
            if (spawn == picked_enemy_index)
            {
                SpawnDefender();
                return;
                
            }
        }

        for (int i = 0; i < defender_locations.Length; i++)
        {

            if (i == picked_enemy_index)
            {

                spawned.Add(i);

                Debug.Log(defender_locations.Length);

                Debug.Log(picked_enemy_index);

                switch (count_defender)
                {
                    case 0:
                        Instantiate(defender, defender_locations[i].position, Quaternion.identity);
                        break;

                    case 1:
                        Instantiate(hover_defender, defender_locations[i].position, Quaternion.identity);
                        break;

                    case 2:
                        Instantiate(BOOM_defender, defender_locations[i].position, Quaternion.identity);
                        break;
                }

                count_defender++;

                //GameObject spawned_defender = Instantiate(defender, defender_locations[i].position, Quaternion.identity);

                //Tower tower_defender = spawned_defender.GetComponent<Tower>();

                //tower_defender.SetSpawnIndex(i);

            }

        }

        DefencePlacementChanged();

    }


    void GenerateSpawns ()
    {
        spawn_1.position = new Vector3(spawn_1.position.x, 50.0f, spawn_1.position.z);

        spawn_2.position = new Vector3(spawn_2.position.x, 50.0f, spawn_2.position.z);

        spawn_3.position = new Vector3(spawn_3.position.x, 50.0f, spawn_3.position.z);

        spawn_1.position += new Vector3(Random.Range(-5f, 5f), 0.0f, Random.Range(-5f, 5f));

        spawn_2.position += new Vector3(Random.Range(-5f, 5f), 0.0f, Random.Range(-5f, 5f));

        spawn_3.position += new Vector3(Random.Range(-5f, 5f), 0.0f, Random.Range(-5f, 5f));

        Ray ray1 = new Ray(spawn_1.position, -Vector3.up);
        Debug.DrawRay(spawn_1.position, -Vector3.up);
        Physics.Raycast(ray1, out rit);
        spawn_1.position = rit.point;

        Ray ray2 = new Ray(spawn_2.position  , -Vector3.up);
        Debug.DrawRay(spawn_2.position, -Vector3.up);
        Physics.Raycast(ray2, out rit);
        spawn_2.position = rit.point;


        Ray ray3 = new Ray(spawn_3.position, -Vector3.up);
        Debug.DrawRay(spawn_3.position, -Vector3.up);
        Physics.Raycast(ray3, out rit);
        spawn_3.position = rit.point;


        Ray raytower = new Ray(tower_positon, -Vector3.up);
        Debug.DrawRay(tower_positon, -Vector3.up);
        Physics.Raycast(raytower, out rit);
        tower_positon.y = rit.point.y;

        tower_positon.y -= 0.5f;



        InstantiateSpawns();

    }

    void InstantiateSpawns ()
    {
        Instantiate(tower, tower_positon, Quaternion.identity);

        EnemySpawner enemy_spawn_1 = Instantiate(enemy_spawn, spawn_1.position, Quaternion.identity).GetComponent<EnemySpawner>();

        enemy_spawn_1.SetPath(path_1);

        EnemySpawner enemy_spawn_2 = Instantiate(enemy_spawn, spawn_2.position, Quaternion.identity).GetComponent<EnemySpawner>();

        enemy_spawn_2.SetPath(path_2);

        EnemySpawner enemy_spawn_3 = Instantiate(enemy_spawn, spawn_3.position, Quaternion.identity).GetComponent<EnemySpawner>();

        enemy_spawn_3.SetPath(path_3);


    }

}

class Grid
{
    GridPoint[,] grid_nodes;

    int grid_size;

    int node_distance;

    public Grid(int size, int distance)
    {
        grid_size = size;

        node_distance = distance;

        grid_nodes = new GridPoint[size,size];

        PopulateGrid();
    }


    //Technical advice in perlin noise in unity [1]
    void PopulateGrid()
    {
        
        for (int x = 0; x < grid_size; x++)
        {
            for (int y = 0; y < grid_size; y++)
            {
                float x_perlin_input = (float)x / (float)grid_size;

                float y_perlin_input = (float)y / (float)grid_size;

                float perlin_weight = Mathf.PerlinNoise(x_perlin_input, y_perlin_input);

                //Debug.Log(perlin_weight);

                float grid_length = (float)node_distance * (float)grid_size;

                float half_grid_lenth = grid_length * 0.5f;

                float x_location = (float)x * (float)node_distance;

                float y_location = y * node_distance;

                //Debug.Log(string.Format("X = {0} : Y = {1}", x_location, y_location));

                Vector3 node_location = new Vector3((x_location - half_grid_lenth) + (node_distance / 2), 50.0f, (y_location - half_grid_lenth) + (node_distance / 2));

                GridPoint created_grid_node = new GridPoint(node_location, perlin_weight);

                grid_nodes[x, y] = created_grid_node;
            }
        }
    }

    public GridPoint[,] GetGridNodes()
    {
        return grid_nodes;
    }

    public GridPoint GetNode(int x, int y)
    {
        return grid_nodes[x, y];
    }

}

struct GridPoint
{
    Vector3 location_local;

    float perlin_weight_value_local;

    public GridPoint(Vector3 location, float perlin_weight_value)
    {
        location_local = location;

        perlin_weight_value_local = perlin_weight_value;

    }

    public Vector3 GetLocation()
    {
        return location_local;
    }

    public void SetNodeHeight(float ground_height, float height_offset)
    {

        location_local.y = ground_height + height_offset;

    }

    public float GetPerlinWeightValue()
    {
        return perlin_weight_value_local;
    }

}
