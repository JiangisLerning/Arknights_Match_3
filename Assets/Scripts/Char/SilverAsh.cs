using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilverAsh : Char
{
    [SerializeField]
    private int range = 4;

    [SerializeField]
    private int atkTimes = 1;

    public override int GetAtkTimes ()
    {
        return atkTimes;
    }

    public override List<XYInGrid> GetRange()
    {
        List<XYInGrid> Range = new List<XYInGrid>();
        XYInGrid temp;

        if (Direction==Dir.UP)//y-
        {
            for (int xStp = 1 - range; xStp < range; xStp++)
            {
                for (int yStp = 0; yStp < range; yStp++)
                {
                    int steps = Mathf.Abs(xStp) + Mathf.Abs(yStp);
                    if (steps!=0 && steps < range)
                    {
                        temp.x = XInGrid + xStp;
                        temp.y = YInGrid - yStp;
                        Range.Add(temp);
                    }
                }
            }
        }
        else if (Direction==Dir.DOWN)//y+
        {
            for (int xStp = 1 - range; xStp < range; xStp++)
            {
                for (int yStp = 0; yStp < range; yStp++)
                {
                    int steps = Mathf.Abs(xStp) + Mathf.Abs(yStp);
                    if (steps != 0 && steps < range)
                    {
                        temp.x = XInGrid + xStp;
                        temp.y = YInGrid + yStp;
                        Range.Add(temp);
                    }
                }
            }
        }
        else if (Direction==Dir.LEFT)//x-
        {
            for (int xStp = 0; xStp < range; xStp++)
            {
                for (int yStp = 1 - range; yStp < range; yStp++)
                {
                    int steps = Mathf.Abs(xStp) + Mathf.Abs(yStp);
                    if (steps != 0 && steps < range)
                    {
                        temp.x = XInGrid - xStp;
                        temp.y = YInGrid + yStp;
                        Range.Add(temp);
                    }
                }
            }
        }
        else if (Direction==Dir.RIGHT)//x+
        {
            for (int xStp = 0; xStp < range; xStp++)
            {
                for (int yStp = 1 - range; yStp < range; yStp++)
                {
                    int steps = Mathf.Abs(xStp) + Mathf.Abs(yStp);
                    if (steps != 0 && steps < range)
                    {
                        temp.x = XInGrid + xStp;
                        temp.y = YInGrid + yStp;
                        Range.Add(temp);
                    }
                }
            }
        }

        return Range;

    }

}
