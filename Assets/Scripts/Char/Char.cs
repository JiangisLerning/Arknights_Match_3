using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char : MonoBehaviour
{
    public enum Name
    {
        NONE,
        SilverAsh,
        Ifrit,
        COUNT,
    };

    public enum Dir
    {
        NONE,
        UP,
        LEFT,
        DOWN,
        RIGHT,
    };

    public struct XYInGrid
    {
        public int x;
        public int y;
    };
    public bool IsAttackting { get; set; } = false;
    public bool IsBeingCleared { get; set; } = false;
    public int XInGrid { get; set; }
    public int YInGrid { get; set; }
    public bool IsPlaced { get; set; } = false;
    public Dir Direction { get; set; }
    public Name NameOfChar { get; set; }
    public CharManager CharManager { get; set; }
    public CharController CharController { get; private set; }

    private void Awake()
    {
        CharController = GetComponent<CharController>();
    }

    public void init(Name _name, CharManager _charManager)
    {
        NameOfChar = _name;
        CharManager = _charManager;
    }

    public virtual List<XYInGrid> GetRange()
    {
        List<XYInGrid> Range = new List<XYInGrid>();
        return Range;
    }

    public virtual int GetAtkTimes()
    {
        return 0;
    }

}
