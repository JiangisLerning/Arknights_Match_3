using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirSelectUI : MonoBehaviour
{
    private Char _char;

    private void Start()
    {
        _char = GetComponentInParent<Char>();
    }
    private void OnMouseDown()
    {
        _char.CharController.Towards(gameObject);
    }

    private void OnMouseEnter()
    {
        transform.GetComponent<SpriteRenderer>().material.color=new Color(1, 0.6870695f, 0.4103774f,1);
    }

    private void OnMouseExit()
    {
        transform.GetComponent<SpriteRenderer>().material.color = Color.white;
    }
}
