using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private int x;
    private int y;

    public int X
    {
        get { return x; }
        set
        {
            if (IsMovable())
            {
                x = value;
            }
        }
    }
    public int Y
    {
        get { return y; }
        set
        {
            if (IsMovable())
            {
                y = value;
            }
        }
    }
    public GridManager.UnitType Type { get; private set; }
    public GridManager GridRef { get; private set; }
    public UnitMove MovableComponent { get; private set; }
    public UnitColor ColorComponent { get; private set; }
    public UnitClear ClearableComponent { get; private set; }
    public EliteHandler EliteHandler { get; private set; }
    public Collider2D Collider2D { get; private set; }
    public Animator Animator { get; private set; }

    private void Awake()
    {
        MovableComponent = GetComponent<UnitMove>();
        ColorComponent = GetComponent<UnitColor>();
        ClearableComponent = GetComponent<UnitClear>();
        EliteHandler = GetComponent<EliteHandler>();
        Collider2D = GetComponent<Collider2D>();
        Animator = GetComponent<Animator>();
    }

    public void init(int _x, int _y, GridManager _grid, GridManager.UnitType _type)
    {
        X = _x;
        Y = _y;
        GridRef = _grid;
        Type = _type;
    }

    public bool IsMovable()
    {
        return MovableComponent != null;
    }

    public bool IsColored()
    {
        return ColorComponent != null;
    }

    public bool IsClearable()
    {
        if (IsElite())
        {
            return ClearableComponent != null && EliteHandler.HealthSystem.GetValue() <= 0;
        }
        else
        {
            return ClearableComponent != null;
        }
        
    }

    public bool IsElite()
    {
        return EliteHandler != null;
    }

    public bool IsFrozen() => !MovableComponent.enabled && !Collider2D.enabled;
}
