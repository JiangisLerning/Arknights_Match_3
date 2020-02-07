using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClearCostRecover : UnitClear
{
    public override void Clear()
    {
        base.Clear();

        StartCoroutine(MoveCostIconCoroutine());
    }

    private IEnumerator MoveCostIconCoroutine()
    {
        Transform currencyUITf = unit.GridRef.Canvas.transform.Find("Currency");
        Transform costIconTf = transform.Find("costIcon");
        Color color = costIconTf.GetComponent<SpriteRenderer>().material.color;
        Vector3 startPos = costIconTf.position;
        Vector3 uiInWld = Camera.main.ScreenToWorldPoint(currencyUITf.position);
        Vector3 endPos = new Vector3(uiInWld.x, uiInWld.y, 0f);

        for (float t = 0; t <= 0.5f; t += Time.deltaTime)
        {
            color = new Color(1, 1, 1, 1 - t / 0.5f);
            costIconTf.position = Vector3.Lerp(startPos, endPos, t / 0.5f);
            yield return 0;
        }

        
    }
}
