using UnityEngine;

//Technical advise on inheritence in c# [2]
public class RangeEnemy : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        SetupEnemy();

        health = 5;

        speed = 1;

        enemy_damage = 5;

    }

    // Update is called once per frame
    void Update()
    {

        TrackPath();

        CheckAttackLogic();

        FireCooldown();


        if (final_node == true)
        {

            attack = true;

            cc.enabled = false;
        }
    }
}
