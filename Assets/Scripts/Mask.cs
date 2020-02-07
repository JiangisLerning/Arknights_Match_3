using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour
{
    private GridManager grid;
    public bool IsMasking { get; set; }

    private Vector3 wideVision = new Vector3(140, 140, 1);
    private Vector3 narrowVision = new Vector3(10, 10, 1);
    private float time = 0.5f;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        grid = FindObjectOfType<GridManager>();
    }

    public IEnumerator FollowMouse()
    {
        IsMasking = true;

        StartCoroutine(SchrinkVision());
        
        while (grid.NumScout > 0)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            //Debug.Log(transform.position.x + "," + transform.position.y + "," + transform.position.z);
            yield return 0;
        }
        
        StopCoroutine(SchrinkVision());
        transform.localScale = wideVision;
        transform.position = Vector3.zero;
        IsMasking = false;
    }

    public IEnumerator SchrinkVision()
    {
        while (IsMasking && transform.localScale != narrowVision)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, narrowVision, ref velocity, time);
            yield return new WaitForEndOfFrame();
        }
    }
}
