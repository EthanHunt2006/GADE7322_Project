using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    Transform[] path;

    float spawn_interval_min = 1f;

    float spawn_interval_max = 5f;

    float spawn_time = 0;

    public GameObject Enemy;

    public void SetPath(Transform[] path_data)
    {
        path = path_data;

        spawn_time = Random.Range(spawn_interval_min, spawn_interval_max);

    }

    private void Update()
    {

        spawn_time -= Time.deltaTime;

        if (spawn_time <= 0)
        {
            SpawnEnemy();

            spawn_time = Random.Range(spawn_interval_min, spawn_interval_max);

        }
        
    }

    void SpawnEnemy()
    {
        GameObject new_enemy = Instantiate(Enemy, transform.position + new Vector3(0.0f, 3.0f, 0.0f), Quaternion.identity);

        Enemy current_enemy = new_enemy.GetComponent<Enemy>();

        current_enemy.SetPathNodes(path);

    }
}
