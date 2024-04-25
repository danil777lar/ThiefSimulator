using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sellable : MonoBehaviour
{
    [SerializeField] private int cost;

    private bool _inSaleProcess;
    private Rigidbody _rb;
    private Collider _collider;
    
    public bool InSaleProcess => _inSaleProcess;
    public int Cost => cost;

    public void PrepareToSell()
    {
        _inSaleProcess = true;
        _rb.isKinematic = true;
        _collider.isTrigger = true;
    }
    
    public void StopSelling()
    {
        _inSaleProcess = false;
        _rb.isKinematic = false;
        _collider.isTrigger = false;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
}
