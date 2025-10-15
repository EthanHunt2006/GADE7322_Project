using UnityEngine;

//Technical advise on inheritence in c# [2]
public class BombEnemy : Enemy
{
    public GameObject bomb;

    public GameObject BOOM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        SetupEnemy();

        range_reached_scalar = 1;

        kill_scalar = 1;

        health = 20;

        speed = 4.0f;

    }

    protected override void BeforeDeath()
    {
        Instantiate(bomb, transform.position, Quaternion.identity);

        Instantiate(BOOM, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

        TrackPath();

        CheckAttackLogic();

        FireCooldown();

        if (reached_final_node == true)
        {
            Instantiate(bomb, transform.position, Quaternion.identity);

            Instantiate(BOOM, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

       
    }
}
