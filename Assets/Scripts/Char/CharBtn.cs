using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject charPrefab;
    [SerializeField]
    private int cost;
    [SerializeField]
    private Char.Name charName;

    public GameObject CharPrefab { get => charPrefab; }
    public bool IsUsed { get; set; } = false;
    public int Cost { get => cost; }
    public Char.Name CharName { get => charName; private set => charName = value; }

    private CharManager charManager;

    private void Awake()
    {
        charManager = FindObjectOfType<CharManager>();
    }

    public bool IsAvailable()
    {
        if (!IsUsed && charManager.Currency.GetValue() >= Cost)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
