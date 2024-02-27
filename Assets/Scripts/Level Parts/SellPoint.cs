using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;

public class SellPoint : MonoBehaviour
{
    [SerializeField] private float sellDelay;

    [InjectService] private ICurrencyService _currencyService;
        
    private float _currentTime;
    private List<Sellable> _objectsToSell;

    private void Start()
    {
        ServiceLocator.Default.InjectServicesInComponent(this);
        
        _objectsToSell = new List<Sellable>();
    }
    
    private void Update()
    {
        if (_objectsToSell.Count > 0)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= sellDelay)
            {
                Sell(_objectsToSell.First());
            }
        }
        else
        {
            _currentTime = 0f;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Sellable sellable = other.GetComponentInParent<Sellable>();
        if (sellable != null && !_objectsToSell.Contains(sellable))
        {
            _objectsToSell.Add(sellable);            
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Sellable sellable = other.GetComponentInParent<Sellable>();
        if (sellable != null && _objectsToSell.Contains(sellable))
        {
            _objectsToSell.Remove(sellable);            
        }
    }

    private void Sell(Sellable sellable)
    {
        if (_objectsToSell.Contains(sellable))
        {
            _objectsToSell.Remove(sellable);
            _currencyService.AddCurrency(CurrencyType.Coins, CurrencyPlacementType.Level, sellable.Cost);
            Destroy(sellable.gameObject);
        }
    }
}
