using UnityEngine;

//Technical advise on inheritence in c# [2]
public class MeleeEnemy : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        SetupEnemy();

        range_reached_scalar = 1;

        kill_scalar = 1;

        enemy_damage = 10;

        
        
    }

    // Update is called once per frame
    void Update()
    {

        TrackPath();

        CheckAttackLogic();

        FireCooldown();


        if (reached_final_node == true)
        {
            attack = true;

            cc.enabled = false;
        }
    }
}
