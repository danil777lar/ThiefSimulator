using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sellable : MonoBehaviour
{
    [SerializeField] private int cost;

    private bool _inSaleOrder;
    private bool _inSaleProcess;
    private Rigidbody _rb;
    private Collider _collider;

    public bool InSaleOrder => _inSaleOrder; 
    public bool InSaleProcess => _inSaleProcess;
    public int Cost => cost;

    public event Action EventSold;

    public void AddToSellOrder()
    {
        _inSaleOrder = true;
    }
    
    public void PrepareToSell()
    {
        _inSaleProcess = true;
        _rb.isKinematic = true;
        _collider.isTrigger = true;
    }
    
    public void StopSelling()
    {
        _inSaleOrder = false;
        _inSaleProcess = false;
        _rb.isKinematic = false;
        _collider.isTrigger = false;
    }

    public void Sell()
    {
        EventSold?.Invoke();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponentInChildren<Collider>();
    }
}
