using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    private Unit unit;
    private IEnumerator moveCoroutine;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public void Move(int newX, int newY, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        unit.X = newX;
        unit.Y = newY;

        Vector3 startPos = transform.position;
        Vector3 endPos = unit.GridRef.GetWorldPosition(newX, newY);

        for (float t = 0; t <= 1*time; t+= Time.deltaTime)
        {
            unit.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        unit.transform.position = endPos;
    }

}
