using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ifrit : Char
{
    [SerializeField]
    private int range = 8;

    [SerializeField]
    private int atkTimes = 1;

    public override int GetAtkTimes()
    {
        return atkTimes;
    }

    public override List<XYInGrid> GetRange()
    {
        List<XYInGrid> Range = new List<XYInGrid>();
        XYInGrid temp;

        if (Direction == Dir.UP)//y-
        {

            for (int yStp = 1; yStp < range; yStp++)
            {

                temp.x = XInGrid;
                temp.y = YInGrid - yStp;
                Range.Add(temp);

            }

        }
        else if (Direction == Dir.DOWN)//y+
        {

            for (int yStp = 1; yStp < range; yStp++)
            {

                temp.x = XInGrid;
                temp.y = YInGrid + yStp;
                Range.Add(temp);

            }

        }
        else if (Direction == Dir.LEFT)//x-
        {
            for (int xStp = 1; xStp < range; xStp++)
            {


                temp.x = XInGrid - xStp;
                temp.y = YInGrid;
                Range.Add(temp);


            }
        }
        else if (Direction == Dir.RIGHT)//x+
        {
            for (int xStp = 1; xStp < range; xStp++)
            {

                temp.x = XInGrid + xStp;
                temp.y = YInGrid;
                Range.Add(temp);

            }
        }

        return Range;
    }

}
