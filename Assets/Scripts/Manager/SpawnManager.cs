
using UnityEngine;
using System.Collections.Generic;


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

    public Transform[] defender_locations;

    List<int> spawned = new List<int>();

    

    int attempt = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateSpawns();



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

        for (int i = 0; i <= defender_locations.Length - 1; i++)
        {

            if (i == picked_enemy_index)
            {

                spawned.Add(i);

                Instantiate(defender, defender_locations[i].position, Quaternion.identity);

            }

        }

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
