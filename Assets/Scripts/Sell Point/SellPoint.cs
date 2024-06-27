using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Dreamteck.Splines;
using Larje.Core.Services;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class SellPoint : MonoBehaviour
{
    [SerializeField] private float sellDelay;
    [SerializeField] private float sellAnimDuration;
    [SerializeField] private Ease sellEase;
    [SerializeField] private SplineComputer trajectory;
    [SerializeField] private GameObject content;
    [Space]
    [SerializeField] private Image sellProgressUi;

    [InjectService] private ICurrencyService _currencyService;
    
    private bool _triggerActive;
    private float _currentTime;
    private CharacterCarry3D _playerCarry;
    private List<Sellable> _objectsToSell;

    private List<Func<float>> _sellPriceMultipliers = new List<Func<float>>();
    
    public bool TriggerActive => _triggerActive;
    
    public void AddSellPriceMultiplier(Func<float> multiplier)
    {
        if (!_sellPriceMultipliers.Contains(multiplier))
        {
            _sellPriceMultipliers.Add(multiplier);
        }
    }
    
    public void RemoveSellPriceMultiplier(Func<float> multiplier)
    {
        if (_sellPriceMultipliers.Contains(multiplier))
        {
            _sellPriceMultipliers.Remove(multiplier);
        }
    }

    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        Character player = FindObjectsOfType<Character>().ToList()
            .Find(x => x.CharacterType == Character.CharacterTypes.Player);
        _playerCarry = player.GetComponentInChildren<CharacterCarry3D>();
        
        _objectsToSell = new List<Sellable>();
    }

    private void Update()
    {
        CheckIsActive();
        
        if (_triggerActive && _objectsToSell.Count > 0)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= sellDelay)
            {
                _currentTime = 0f;
                Sell(_objectsToSell.First());
            }
        }
        else
        {
            _currentTime = 0f;
        }

        sellProgressUi.fillAmount = _currentTime / sellDelay;
    }

    private void OnTriggerEnter(Collider other)
    {
        Sellable sellable = other.GetComponentInParent<Sellable>();
        if (sellable != null && !sellable.InSaleProcess && !_objectsToSell.Contains(sellable))
        {
            _objectsToSell.Add(sellable);            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Sellable sellable = other.GetComponentInParent<Sellable>();
        if (sellable != null && _objectsToSell.Contains(sellable))
        {
            sellable.StopSelling();
            _objectsToSell.Remove(sellable);            
        }
    }

    private void Sell(Sellable sellable)
    {
        if (_objectsToSell.Contains(sellable))
        {
            Vector3 startPoint = sellable.transform.position;
            _objectsToSell.Remove(sellable);
            sellable.PrepareToSell();

            int price = Mathf.RoundToInt(sellable.Cost * GetSellPriceMultiplier());
            _currencyService.AddCurrency(CurrencyType.Coins, CurrencyPlacementType.Level, price);
            
            DOTween.To(() => 0f, 
                    x => sellable.transform.position =
                        EvaluateTrajectory(startPoint, x), 
                    1f, sellAnimDuration)
                .SetEase(sellEase)
                .OnComplete(() =>
                {
                    Destroy(sellable.gameObject);
                });
        }
    }

    private void CheckIsActive()
    {
        _triggerActive = _objectsToSell.Count > 0 || _playerCarry.HasCarryable;
        
        content.SetActive(_triggerActive);
    }

    private Vector3 EvaluateTrajectory(Vector3 startPoint, float t)
    {
        return Vector3.Lerp(startPoint, trajectory.EvaluatePosition(t), t * 10f);
    }

    private float GetSellPriceMultiplier()
    {
        float result = 1f;
        foreach (Func<float> multiplier in _sellPriceMultipliers)
        {
            result *= multiplier();
        }

        return result;
    }
}
