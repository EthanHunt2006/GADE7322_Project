using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected CharacterController cc;

    Transform[] path_nodes;

    protected int node_index = 0;

    protected bool final_node = false;

    protected bool attack = false;

    protected int path_index = 0;

    protected Vector3 current_node;

    protected float y_offset = 0.0f;

    protected int health = 15;

    protected float attack_interval = 1.0f;

    protected int enemy_damage = 5;

    public GameObject line_obj;

    protected LineRenderer line_renderer;

    protected float cool_down_timer = 0.3f;

    protected float speed = 2.0f;

    protected GameObject tower_obj;

    protected bool reached_final_node = false;

    public Animator enemy_animation;

    public GameObject death_particle;

    GameObject game_manager;

    Difficulty difficulty;

    protected int range_reached_scalar = 1;

    protected int kill_scalar = 1;

    bool attack_trigger = false;

    protected bool death;

    public GameObject health_bar_obj;

    HealthBar health_bar;

    protected virtual void BeforeDeath()
    {

    }


    protected void PlayAnimation ()
    {

        enemy_animation.SetBool("Attack", true);
    }

    protected void StopAnimation()
    {
        enemy_animation.SetBool("Attack", false);
    }



    public void TakeDamage(int damage)
    {
        health -= damage;

        //Debug.Log("ENEMY took damage : " + damage + " current health is : " + health);

        health_bar = health_bar_obj.GetComponent<HealthBar>();

        health_bar.CurrentHealth(health);

        if (difficulty != null)
        {
            difficulty.TrackDamageDeltPerMinute(damage);
        }

        

        CheckHealth();

    }

    void CheckHealth()
    {

        if (health <= 0)
        {
            Die();

            health = 0;
        }
    }

    void Die()
    {

        BeforeDeath();

        Instantiate(death_particle, transform.position, Quaternion.identity);

        difficulty.TrackKillsPerMinute(kill_scalar);

        Destroy(gameObject);
    }

    public void SetPathIndex(int index)
    {
        path_index = index;
    }

    public void SetPathNodes(Transform[] nodes)
    {


        path_nodes = nodes;

        //Debug.Log("recieved path information");
    }

    protected void SetupEnemy()
    {

        health_bar = health_bar_obj.GetComponent<HealthBar>();

        health_bar.MaxHealth(health);

        cc = GetComponent<CharacterController>();

        line_renderer = line_obj.GetComponent<LineRenderer>();

        tower_obj = GameObject.Find("Tower_Object(Clone)");

        //Debug.Log("tower object is : " + tower_obj);

        game_manager = GameObject.Find("GameManager");

        difficulty = game_manager.GetComponent<Difficulty>();

    }

    protected void CheckAttackLogic()
    {
        if (attack == true)
        {
           ActivateAttacking();

            Attack_Trigger();

        }

    }

    void Attack_Trigger()
    {
        if (attack_trigger == true) 
        {
            return;
        }
        else
        {
            InitiateAttack();
            attack_trigger = true;
        }
    }

    protected void InitiateAttack()
    {

        difficulty.TrackAttackRangeReached(range_reached_scalar);

    }

    protected void ActivateAttacking ()
    {
        attack_interval -= Time.deltaTime;

        if (attack_interval <= 0)
        {
            AttackTower();
            attack_interval = 3.0f;
            PlayAnimation();
        }else
        {
            StopAnimation();
        }

    }

    protected void TrackPath()
    {

        if (attack == false)
        {
            //Debug.Log(node_index);

            current_node = path_nodes[node_index].position;
            y_offset = 0.0f;
        }
        else if (attack == true)
        {
            current_node = tower_obj.transform.position;
            y_offset = 5.0f;
            speed = 0.0f;

            //cc.enabled = false;

        }

        //Debug.Log(y_offset);

        transform.LookAt(current_node + new Vector3 (0.0f, y_offset, 0.0f));

        if (cc.enabled == true)
        {
            cc.Move((transform.forward * Time.deltaTime) * speed);
        }

        

        if (node_index == path_nodes.Length - 1)
        {
            //Debug.Log("Final Node");

            final_node = true;
        }

        if (Vector3.Distance(transform.position, current_node) <= 1f)
        {

            if (final_node == false)
            {
                //Debug.Log("Advance To Next Node!");
                node_index += 1;
            }

            if (final_node == true)
            {
                reached_final_node = true;
            }


        }


    }

    void AttackTower()
    {
        //Debug.Log("damage tower by : " + enemy_damage);

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Player");

        

        int picked_enemy_index = Random.Range(0, towers.Length);

        for (int i = 0; i <= towers.Length; i++)
        {
            if (i == picked_enemy_index)
            {
                towers[i].GetComponent<Tower>().TakeDamage(enemy_damage);

                line_renderer.startWidth = 0.7f;

                line_renderer.startColor = Color.darkRed;
                line_renderer.endColor = Color.red;

                line_renderer.SetPosition(0, line_obj.transform.position);
                line_renderer.SetPosition(1, towers[i].transform.position + new Vector3(0.0f, y_offset, 0.0f));
            }
        }

                

        cool_down_timer = 0.3f;

        



        //Debug.Log("Damage Tower");

    }


    protected void FireCooldown()
    {

        cool_down_timer -= Time.deltaTime;

        if (cool_down_timer <= 0)
        {
           

            line_renderer.SetPosition(0, line_obj.transform.position);
            line_renderer.SetPosition(1, line_obj.transform.position);

            cool_down_timer = 0;


        }
       

    }
}
