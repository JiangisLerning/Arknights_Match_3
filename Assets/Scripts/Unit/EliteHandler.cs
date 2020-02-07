using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteHandler : MonoBehaviour
{
    public Transform pfHealthBar;
    private Settlement healthSystem;
    [SerializeField] private int healthMax;

    public Settlement HealthSystem { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        HealthSystem = new Settlement(healthMax, healthMax);

        Transform healthBarTf = Instantiate(pfHealthBar, position: new Vector3(0f, -0.55f) + transform.position, 
            rotation: Quaternion.identity, parent: transform);
        HealthBar healthBar = healthBarTf.GetComponent<HealthBar>();
        healthBar.Setup(HealthSystem);
    }

}
