using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement
{
    public event EventHandler OnValueChanged;
    protected int value;
    protected int valueMax;
    protected bool hasUplimit;
    protected bool isAutoGrowth;
    protected float growthRate;

    public Settlement() { }

    public Settlement(int valueMax, int valueInit = 0, bool isAutoGrowth = false, float growthRate = 1)
    {
        this.valueMax = valueMax;
        hasUplimit = true;
        value = valueInit;
        this.isAutoGrowth = isAutoGrowth;
        this.growthRate = growthRate;
      
    }

    public Settlement(int valueInit, bool isAutoGrowth = false)
    {
        hasUplimit = false;
        value = valueInit;
        this.isAutoGrowth = isAutoGrowth;
    }

    public int GetValue()
    {
        return value;
    }

    public float GetPercent()
    {
        if(hasUplimit)
        {
            return (float)value / valueMax;
        }

        return 0;
    }

    public void Expense(int expenseAmount)
    {
        value -= expenseAmount;
        if (value < 0) value = 0;
        OnValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Earn(int earnAmount)
    {
        value += earnAmount;
        if (value > valueMax) value = valueMax;
        OnValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerator AutoGrowth()
    {
        while (isAutoGrowth)
        {
            Earn(1);
            yield return new WaitForSecondsRealtime(growthRate);
        }
        
    }
}
