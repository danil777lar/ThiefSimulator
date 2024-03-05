using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sellable : MonoBehaviour
{
    [SerializeField] private int cost;

    private bool _inSaleProcess;
    private Rigidbody _rb;
    
    public bool InSaleProcess => _inSaleProcess;
    public int Cost => cost;

    public void PrepareToSell()
    {
        _inSaleProcess = true;
        _rb.isKinematic = true;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
}
