using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform bar;

    private Settlement healthSystem;

    private void Start()
    {
        bar = transform.Find("Bar");
    }

    public void Setup(Settlement healthSystem)
    {
        this.healthSystem = healthSystem;

        healthSystem.OnValueChanged += HealthSystem_OnValueChanged; 
    }

    private void HealthSystem_OnValueChanged(object sender, System.EventArgs e)
    {
        transform.Find("Bar").localScale = new Vector3(healthSystem.GetPercent(), 1f);
    }

}
