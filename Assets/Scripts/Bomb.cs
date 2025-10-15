using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Bomb : MonoBehaviour
{
    List<Tower> towers = new List<Tower>();

    List<Enemy> enemies = new List<Enemy>();

    GameObject[] tower_objects;

    GameObject[] enemy_objects;

    public float explosion_range = 5f;

    public int explosion_damage = 25;

    public bool damage_player = true;

    public bool damage_enemy = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (damage_player == true) 
        {
            GetTowers();
        }

        if (damage_enemy == true)
        {
            GetEnemies();
        }
        

        DamageAOE();
        
    }

    void GetTowers()
    {
        tower_objects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject tower in tower_objects)
        {
            if (Vector3.Distance(transform.position, tower.transform.position) <= explosion_range)
            {
                towers.Add(tower.GetComponent<Tower>());
            }

            

        }
    }

    void GetEnemies()
    {
        enemy_objects = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemy_objects)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= explosion_range)
            {
                enemies.Add(enemy.GetComponent<Enemy>());
            }



        }


    }

    void DamageAOE()
    {
        if (damage_player == true)
        {

            foreach (Tower tower in towers)
            {
                tower.TakeDamage(explosion_damage);

            }
        }

        if (damage_enemy == true)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.TakeDamage(explosion_damage);

            }
        }

        Destroy(gameObject);

    }

    
}
