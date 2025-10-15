using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    Transform[] path;

    float spawn_interval_min = 2f;

    float spawn_interval_max = 8f;

    float spawn_time = 0;

    public GameObject EnemyMelee;

    public int melee_spawn_chance = 50;

    public GameObject EnemyRanged;

    public int ranged_spawn_chance = 50;

    public GameObject EnemyBomber;

    public int bomber_spawn_chance = 65;

    [SerializeField]
    int spawn_probability = 0;

    [SerializeField]
    int base_probability = 0;

    float blockout_distance = 10f;

    bool block_spawning = false;


    List<EnemySpawnSlot> enemies = new List<EnemySpawnSlot> ();

    public void UpdatePerformance(int damage_taken, int damage_dealt)
    {

       

        //Debug.Log(string.Format("Damage Taken = {0}", damage_taken));

        //Debug.Log(string.Format("Damage Dealt = {0}", damage_dealt));

        if (damage_taken > damage_dealt)
            spawn_probability = base_probability / damage_taken;

        if (damage_dealt > damage_taken)
            spawn_probability = base_probability * damage_dealt;

        if (damage_taken == damage_dealt)
            spawn_probability = base_probability;

            //Debug.Log(string.Format("Spawn Probability = {0}", spawn_probability));
            /*
            foreach (EnemySpawnSlot slot in enemies)
            {

            switch (slot.GetTypeName())
            {
                case"melee":
                    if (damage_taken > damage_dealt)
                        slot.SetSpawnChance(melee_spawn_chance / damage_taken);
                    if (damage_dealt > damage_taken)
                        slot.SetSpawnChance(melee_spawn_chance * damage_dealt);
                    if (damage_taken == damage_dealt)
                        slot.SetSpawnChance(melee_spawn_chance);
                    break;

                case "ranged":
                    if (damage_taken > damage_dealt)
                        slot.SetSpawnChance(ranged_spawn_chance / damage_taken);
                    if (damage_dealt > damage_taken)
                        slot.SetSpawnChance(ranged_spawn_chance * damage_dealt);
                    if (damage_taken == damage_dealt)
                        slot.SetSpawnChance(ranged_spawn_chance);
                    break;
            }

            

            //Debug.Log(string.Format("Slot Spawn Chance = {0}", slot.GetSpawnChance()));

            if (slot.GetSpawnChance() >= 100)
            {
                slot.SetSpawnChance(100);
            }
        }
            */

        if (spawn_probability >= 100)
        {
            spawn_probability = 100;
        }


    }

    public void SetSpawnProbability(float spawn_weight)
    {
        float initial_probability = spawn_weight * 100;

        spawn_probability = (int)initial_probability;

    }

    public void UpdateDefencePlacement()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Player");

        Blockout(towers);

    }

    void Blockout(GameObject[] obsticles)
    {

        foreach(GameObject obsticle in obsticles)
        {
            float distance_to_obsticle = Vector3.Distance(transform.position, obsticle.transform.position);

            //distance to obsticle is smaller than blockout distance, therefore its ti close needs to be blocked out
            if (blockout_distance >= distance_to_obsticle)
            {

                block_spawning = true;

            }
            else
            {
                block_spawning = false;
            }

        }
        


    }

    public void SetPath(Transform[] path_data)
    {
        path = path_data;

        spawn_time = Random.Range(spawn_interval_min, spawn_interval_max);

    }

    private void Start()
    {
        UpdateDefencePlacement();

        enemies.Add(new EnemySpawnSlot(EnemyMelee, melee_spawn_chance, "melee"));

        enemies.Add(new EnemySpawnSlot(EnemyRanged, ranged_spawn_chance, "ranged"));

        enemies.Add(new EnemySpawnSlot(EnemyBomber, bomber_spawn_chance, "bomber"));

        base_probability = spawn_probability;

    }


    public void SpawnLogic()
    {
        if (block_spawning == false)
        {
            foreach (EnemySpawnSlot enemy_slot in enemies)
            {
                if (DetermineSpawn(enemy_slot) == true)
                {
                    return;
                }

            }
        }

    }

    bool CheckProbability(int success_percentage)
    {
        int probability = Random.Range(0, 100);

        if(probability <= success_percentage)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    void SpawnEnemy(GameObject enemy_type)
    {

        GameObject new_enemy = Instantiate(enemy_type, transform.position + new Vector3(0.0f, 3.0f, 0.0f), Quaternion.identity);

        Enemy current_enemy = new_enemy.GetComponent<Enemy>();

        current_enemy.SetPathNodes(path);

    }

    bool DetermineSpawn(EnemySpawnSlot current_slot)
    {

        //We have decided to apply for spawning
        bool has_spawned = CheckProbability(spawn_probability);

        if (has_spawned == true)
        {
            //Debug.Log(string.Format("current slot spawn range ({0}). spawn probability ({1})", current_slot.GetSpawnChance(), spawn_probability));

            //if out enemy spawning range is less than or equal to our spawn probability
            if (current_slot.GetSpawnChance() <= spawn_probability)
            {
                //we can spawn the enemy
                SpawnEnemy(current_slot.GetEnemyType());
            }

        }

        return has_spawned;

    }

}

public struct EnemySpawnSlot
{
    GameObject enemy_type_local;

    int spawn_chance_local;

    string type_name_local;


    public EnemySpawnSlot(GameObject enemy_type, int spawn_chance, string type_name)
    {
        enemy_type_local = enemy_type;

        spawn_chance_local = spawn_chance;

        type_name_local = type_name;
    }

    public string GetTypeName()
    {
        return type_name_local;
    }

    public GameObject GetEnemyType()
    {
        return enemy_type_local;
    }

    public void SetEnemyType (GameObject enemy_type)
    {
        enemy_type_local = enemy_type;
    }

    public int GetSpawnChance()
    {
        return spawn_chance_local;
    }

    public void SetSpawnChance(int spawn_chance)
    {
        spawn_chance_local = spawn_chance;
    }


}
