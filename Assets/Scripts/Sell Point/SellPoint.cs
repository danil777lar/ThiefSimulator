using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Dreamteck.Splines;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class SellPoint : MonoBehaviour, ILevelEventHandler
{
    [SerializeField] private float sellDelay;
    [SerializeField] private float sellAnimDuration;
    [SerializeField] private Image sellProgressUi;
    [SerializeField] private SplineComputer trajectory;

    [InjectService] private ICurrencyService _currencyService;
        
    private float _currentTime;
    private List<Sellable> _objectsToSell;
    
    public void OnLevelEvent(LevelEvent levelEvent)
    {
        if (levelEvent is LevelEventProgressComplete { Type: LevelEventProgressComplete.ProgressType.Full })
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        
        _objectsToSell = new List<Sellable>();
    }

    private void Update()
    {
        if (_objectsToSell.Count > 0)
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
            _currencyService.AddCurrency(CurrencyType.Coins, CurrencyPlacementType.Level, sellable.Cost);
            DOTween.To(() => 0f, 
                    x => sellable.transform.position =
                        EvaluateTrajectory(startPoint, x), 
                    1f, sellAnimDuration)
                .OnComplete(() =>
                {
                    Destroy(sellable.gameObject);
                });
        }
    }

    private Vector3 EvaluateTrajectory(Vector3 startPoint, float t)
    {
        return Vector3.Lerp(startPoint, trajectory.EvaluatePosition(t), t * 10f);
    }
}
