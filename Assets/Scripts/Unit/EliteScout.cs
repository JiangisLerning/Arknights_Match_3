using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteScout : MonoBehaviour
{
    [SerializeField]
    private float bufferTime;

    private Unit unit;

    // Start is called before the first frame update
    private void Start()
    {
        unit = GetComponent<Unit>();
        unit.GridRef.NumScout += 1;
        
        StartCoroutine(GetMask());
    }

    private IEnumerator GetMask()
    {
            yield return new WaitForSeconds(bufferTime);
            StartCoroutine(unit.GridRef.Mask.FollowMouse());

    }
}
