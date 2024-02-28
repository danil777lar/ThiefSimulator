using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sellable : MonoBehaviour
{
    [SerializeField] private int cost;

    public int Cost => cost;
}
