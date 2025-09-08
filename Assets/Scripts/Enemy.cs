using UnityEngine;

public class Enemy : MonoBehaviour
{
    CharacterController cc;

    Transform[] path_nodes;

    public Transform tower;

    int node_index = 0;

    bool final_node = false;

    bool attack = false;

    int path_index = 0;

    Vector3 current_node;

    float y_offset = 0.0f;

    int health = 15;

    float attack_interval = 1.0f;

    int enemy_damage = 5;

    public GameObject line_obj;

    LineRenderer line_renderer;

    float cool_down_timer = 0.3f;

    float speed = 2.0f;

    GameObject tower_obj;



    public void TakeDamage(int damage)
    {
        health -= damage;

        //Debug.Log("ENEMY took damage : " + damage + " current health is : " + health);

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


    void Start()
    {
        cc = GetComponent<CharacterController>();

        line_renderer = line_obj.GetComponent<LineRenderer>();

        tower_obj = GameObject.Find("Tower_Object(Clone)");

        Debug.Log("tower object is : " + tower_obj);
    }

    // Update is called once per frame
    void Update()
    {

        TrackPath();

        CheckAttackLogic();

        FireCooldown();
       
    }

    void CheckAttackLogic()
    {
        if (attack == true)
        {
            attack_interval -= Time.deltaTime;

            if (attack_interval <= 0)
            {
                AttackTower();
                attack_interval = 3.0f;
            }
        }

    }

    void TrackPath()
    {

        if (attack == false)
        {
            current_node = path_nodes[node_index].position;
            y_offset = 0.0f;
        }
        else if (attack == true)
        {
            current_node = tower_obj.transform.position;
            y_offset = 5.0f;
            speed = 0.0f;

            cc.enabled = false;

        }

        //Debug.Log(y_offset);

        transform.LookAt(current_node + new Vector3 (0.0f, y_offset, 0.0f));

        cc.Move((transform.forward * Time.deltaTime) * speed);

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

                attack = true;
            }

        
            
        }


    }

    void AttackTower()
    {
        Debug.Log("damage tower by : " + enemy_damage);

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Player");

        int picked_enemy_index = Random.Range(0, towers.Length - 1);

        for (int i = 0; i <= towers.Length - 1; i++)
        {
            if (i == picked_enemy_index)
            {
                towers[i].GetComponent<Tower>().TakeDamage(enemy_damage);
            }
        }

                

        cool_down_timer = 0.3f;

        line_renderer.startWidth = 0.7f;

        line_renderer.startColor = Color.darkRed;      
        line_renderer.endColor = Color.red;

        line_renderer.SetPosition(0, line_obj.transform.position);
        line_renderer.SetPosition(1, current_node + new Vector3(0.0f, y_offset, 0.0f));



        //Debug.Log("Damage Tower");

    }


    void FireCooldown()
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
