using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClearLine : UnitClear
{
    public bool isRaw;

    public override void Clear()
    {
        base.Clear();

        if (isRaw)
        {
            // clear raw
            unit.GridRef.ClearRaw(unit.Y);
        }
        else
        {
            // clear column
            unit.GridRef.ClearColumn(unit.X);
        }
    }
}
