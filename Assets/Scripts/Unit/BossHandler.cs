using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandler : MonoBehaviour
{
    public Transform pfHealthBar;
    private Settlement healthSystem;
    private GridManager gridManager;

    [SerializeField] private int healthMax;
    [SerializeField] private float blockSpawnCD;
    

    public Settlement HealthSystem { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        HealthSystem = new Settlement(healthMax, healthMax);

        Transform healthBarTf = Instantiate(pfHealthBar, position: new Vector3(-0.1f, -1.2f) + transform.position,
            rotation: Quaternion.identity, parent: transform);
        HealthBar healthBar = healthBarTf.GetComponent<HealthBar>();
        healthBar.Setup(HealthSystem);
    }

    public void Startup(GridManager grid)
    {
        gridManager = grid;
        StartCoroutine(SpawnBlocks());
    }

    public IEnumerator SpawnBlocks()
    {
        float[] NumProbs = { 20, 50, 10 };//该轮生成0，1，2个Block的概率

        yield return new WaitForSeconds(blockSpawnCD);
        while (true)
        {
            int count = ProbChoose.Choose(NumProbs);
            for (int i = 0; i < count; i++)
            {
                while (true)
                {
                    int x = Random.Range(0, gridManager.xDim);
                    int y = Random.Range(1, gridManager.yDim);
                    Unit unit = gridManager.GetUnit(x, y);

                    if (unit.Type >= GridManager.UnitType.NORMAL
                        && unit.Type != GridManager.UnitType.SP_Char
                        && unit.Type != GridManager.UnitType.BLOCK)
                    {
                        if (unit == gridManager.PressedUnit)
                        {
                            gridManager.PressedUnit = null;
                        }

                        if (unit == gridManager.EnteredUnit)
                        {
                            gridManager.EnteredUnit = null;
                        }

                        Destroy(unit.gameObject);

                        gridManager.SpawnNewUnit(x, y, GridManager.UnitType.BLOCK);

                        break;
                    }
                }

            }

            yield return new WaitForSeconds(blockSpawnCD);
        }
    }
}
