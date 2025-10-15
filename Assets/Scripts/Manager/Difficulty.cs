using UnityEngine;

public class Difficulty : MonoBehaviour
{

    [SerializeField]
    int performance_scalar = 0;

    [SerializeField]
    int attack_range_reached_scalar = 0;

    [SerializeField]
    int kills_per_minute_scalar = 0;

    [SerializeField]
    int damage_taken_per_minute_scalar = 0;

    [SerializeField]
    int damage_dealt_per_minute_scalar = 0;

    bool ranged_reached_update;

    bool kills_per_minute_update;

    bool damage_taken_update;

    bool damaged_dealt_update;

    EnemySpawner[] all_spawns;

    float performance_window = 30f;

    float current_performance_window = 30f;



    // Update is called once per frame
    void Update()
    {
        DeterminePerformance();
    }

    private void Start()
    {
        all_spawns = GetSpawners();
    }

    void DeterminePerformance ()
    {
        current_performance_window -= Time.deltaTime;

        if (current_performance_window <= 0)
        {
            damage_taken_per_minute_scalar = damage_taken_per_minute_scalar / (int)performance_window;

            damage_dealt_per_minute_scalar = damage_dealt_per_minute_scalar / (int)performance_window;

            current_performance_window = performance_window;

            UpdatePerformance();

            performance_scalar = 0;
        }
    }

    void UpdatePerformance ()
    {

        all_spawns = GetSpawners();

        foreach(EnemySpawner spawn in all_spawns)
        {
            //damage_dealt_per_minute_scalar /= 10;

            if (damage_taken_per_minute_scalar < 1)
            {
                damage_taken_per_minute_scalar = 1;
            }

            if (damage_dealt_per_minute_scalar < 1)
            {
                damage_dealt_per_minute_scalar = 1;
            }

            if (damage_taken_per_minute_scalar >= 4)
            {
                damage_taken_per_minute_scalar = 4;
            }

            if (damage_dealt_per_minute_scalar >= 2)
            {
                damage_dealt_per_minute_scalar = 2;
            }

            Debug.Log(string.Format("Damage Taken Scalar : ({0}), Damage Dealt Scalar : ({1})", damage_taken_per_minute_scalar, damage_dealt_per_minute_scalar));

            spawn.UpdatePerformance(damage_taken_per_minute_scalar, damage_dealt_per_minute_scalar);
        }

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

    void PerformanceScalar(string value_type, int update_value)
    {

        switch (value_type)
        {
            case "attack_range_reached":
                //performance_scalar += update_value;
                break;

            case "kills_per_minute":
                //performance_scalar += update_value;
                break;

            case "damage_taken_per_minute":
                //performance_scalar += update_value;
                break;

            case "damage_dealt_per_minute":
                //performance_scalar += update_value;
                break;

        }


    }

    public void TrackAttackRangeReached(int input_scalar)
    {
        attack_range_reached_scalar += input_scalar;


        PerformanceScalar("attack_range_reached", input_scalar);


    }

    public void TrackKillsPerMinute(int input_scalar)
    {
        kills_per_minute_scalar += input_scalar;


        PerformanceScalar("kills_per_minute", input_scalar);
    }

    public void TrackDamageTakenPerMinute(int input_scalar)
    {
        damage_taken_per_minute_scalar += input_scalar;


        PerformanceScalar("damage_taken_per_minute", input_scalar);
    }

    public void TrackDamageDeltPerMinute(int input_scalar)
    {
        damage_dealt_per_minute_scalar += input_scalar;


        PerformanceScalar("damage_dealt_per_minute", input_scalar);
    }

}
