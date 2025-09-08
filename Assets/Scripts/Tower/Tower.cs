using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
public class Tower : MonoBehaviour
{
    int current_health = 100;

    int tower_damage = 5;

    float attack_interval = 3f;

    float max_attack_interval = 5f;

    public GameObject line_obj;

    LineRenderer line_renderer;

    bool cooldown = false;

    float cool_down_timer = 0.3f;

    public GameObject health_bar_obj;

    HealthBar health_bar;

    public bool is_defender = false;

    private void Awake()
    {
        line_renderer = line_obj.GetComponent<LineRenderer>();
    }

    private void Start()
    {
        health_bar = health_bar_obj.GetComponent<HealthBar>();

        health_bar.MaxHealth(current_health);
    }

    public void TakeDamage(int damage)
    {
        current_health -= damage;

        health_bar = health_bar_obj.GetComponent<HealthBar>();

        health_bar.CurrentHealth(current_health);

        //Debug.Log("TOWER took damage : " + damage + " current health is : " + current_health);

        

    }

    private void Update()
    {
        Debug.Log("Tower Health : " + current_health);

        CheckHealth();

        attack_interval -= Time.deltaTime;

        if (attack_interval <= 0) 
        {
            AttackEnemies();

            attack_interval = Random.Range(0, max_attack_interval);
        
        }

        FireCooldown();


    }

    void AttackEnemies()
    {

        //Debug.Log("Attack Enemy");

        GameObject[] enemies_array = GameObject.FindGameObjectsWithTag("Enemy");
        int picked_enemy_index = Random.Range(0, enemies_array.Length - 1);

        //Debug.Log("enemies length : " + enemies_array.Length);

        //Debug.Log("picked enemy inded : " + picked_enemy_index);

        if (enemies_array.Length > 0) 
        {    

            for (int i = 0; i <= enemies_array.Length - 1; i++)
            {
                if (i == picked_enemy_index)
                {
                    

                    cool_down_timer = 0.3f;

                    enemies_array[i].GetComponent<Enemy>().TakeDamage(tower_damage);

                    //Debug.Log("Shoot enemy");

                    line_renderer.startWidth = 0.7f;

                    line_renderer.startColor = Color.darkBlue;      // Blood at the tip.
                    line_renderer.endColor = Color.cyan;

                    line_renderer.SetPosition(0, line_obj.transform.position);
                    line_renderer.SetPosition(1, enemies_array[i].transform.position);

                }
            }
        }

       
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

    void CheckHealth()
    {
        if (current_health <= 0)
        {
            Die();

            current_health = 0;
        }
    }

    void Die()
    {
        Debug.Log("Tower is dead");

        if (is_defender == false)
        {
            SceneManager.LoadScene("SampleScene");
        }
        else if (is_defender == true) 
        {
            Destroy(gameObject);
        }

            

    }

}
