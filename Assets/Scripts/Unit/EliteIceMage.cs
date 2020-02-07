using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteIceMage : MonoBehaviour
{
    [SerializeField]
    private float bufferTime;
    public Transform pfIceBlock;
    private Unit unit;

    private void Start()
    {
        unit = GetComponent<Unit>();
        StartCoroutine(RandomMove());
    }

    private IEnumerator RandomMove()
    {
        while (true)
        {
            yield return new WaitForSeconds(bufferTime);

            int x = unit.X;
            int y = unit.Y;
            GridManager grid = unit.GridRef;
            int xDim = unit.GridRef.xDim;
            int yDim = unit.GridRef.yDim;

            int xR = Random.Range(x - 1, x + 2);
            int yR = Random.Range(y - 1, y + 2);

            if (xR >= 0 && xR < xDim && yR >= 0 && yR < yDim && (xR != x || yR != y ))
            {
                Unit unitRandom = grid.GetUnit(xR, yR);
                if (!unitRandom.IsFrozen() 
                    && !unitRandom.IsElite()
                    && grid.GetMatch(unitRandom, x, y) == null)
                {
                    grid.SwapUnits(unitRandom, unit);
                    FreezeUnit(unitRandom);
                }
            }

        }

    }

    public void FreezeUnit(Unit unit)
    {
        Transform iceBlockTf = Instantiate(pfIceBlock, unit.transform, false);
        unit.MovableComponent.enabled = false;
        unit.Collider2D.enabled = false;
    }
}
