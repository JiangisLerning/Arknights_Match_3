using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClear : MonoBehaviour
{
    public AnimationClip clearAnimation;

    public bool IsBeingCleared { get; private set; } = false;

    protected Unit unit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public virtual void Clear()
    {
        IsBeingCleared = true;
        if (unit.Type == GridManager.UnitType.ELITE_SCOUT)
        {
            unit.GridRef.NumScout -= 1;
            
        }

        if (unit.IsElite())
        {
            unit.GridRef.Boss.HealthSystem.Expense(5);
        }
        else
        {
            unit.GridRef.Boss.HealthSystem.Expense(1);
        }

        StartCoroutine(ClearCoroutine());
        
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.Play(clearAnimation.name);

            yield return new WaitForSeconds(clearAnimation.length);

            Destroy(gameObject);
        }
    }
}
