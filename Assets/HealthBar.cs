using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

  

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    public void MaxHealth(int max_health)
    {
        slider.maxValue = max_health;
        slider.value = max_health;
    }

    public void CurrentHealth(int current_health)
    {
        //Debug.Log("Adjust health to : " + current_health);

        slider.value = current_health;
    }
}
