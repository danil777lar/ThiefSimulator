using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class SellPoint : MonoBehaviour, ILevelEventHandler
{
    [SerializeField] private float sellDelay;
    [SerializeField] private float sellAnimDuration;
    [SerializeField] private Image sellProgressUi;
    [Header("Trajectory")] 
    [SerializeField] private Transform middlePoint;
    [SerializeField] private Transform finishPoint;
    [Header("Gizmos")] 
    [SerializeField] private Color gizmosColor;
    [SerializeField, Min(0.05f)] private float gizmosStep;

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

    private void OnDrawGizmos()
    {
        if (middlePoint != null && finishPoint != null)
        {
            Gizmos.color = gizmosColor;
            Vector3 lastPoint = transform.position;
            float step = Mathf.Max(0.05f, gizmosStep);
            for (float t = 0f; t <= 1f; t += step)
            {
                Vector3 point = EvaluateTrajectory(transform.position, middlePoint.position, finishPoint.position, t);
                Gizmos.DrawLine(lastPoint, point);
                lastPoint = point;
            }
        }
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
                        EvaluateTrajectory(startPoint, middlePoint.position, finishPoint.position, x), 
                    1f, sellAnimDuration)
                .OnComplete(() =>
                {
                    Destroy(sellable.gameObject);
                });
        }
    }

    private Vector3 EvaluateTrajectory(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(ab, bc, t);
    }
}
