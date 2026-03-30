using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Dreamteck.Splines;
using Larje.Character;
using Larje.Core;
using Larje.Core.Services;
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
    [SerializeField] private Transform currencySpawnPoint;
    [Space]
    [SerializeField] private OffscreenMarker markerPrefab;
    [SerializeField] private Image sellProgressUi;

    [InjectService] private ICurrencyService _currencyService;
    [InjectService] private IPlayerProviderService _playerProviderService;
    
    private bool _triggerActive;
    private float _currentTime;
    private Character _player;
    private CharacterCarry3D _playerCarry;
    private OffscreenMarker _marker;
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
        DIContainer.InjectTo(this);

        _objectsToSell = new List<Sellable>();
        if (_playerProviderService.TryGetPlayer(out _player))
        {
            _playerCarry = _player.FindAbility<CharacterCarry3D>();
            _marker = Instantiate(markerPrefab).Init(transform, _playerCarry.transform, IsMarkerActive);
        }
        else
        {
            UnityEngine.Debug.LogError("Player not found", this);
        }
    }

    private void OnDestroy()
    {
        if (_marker != null)
        {
            Destroy(_marker.gameObject);
        }
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
        if (other.gameObject == _player.gameObject)
        {
            List<Carryable> carryable = _playerCarry.DropAll();
            foreach (Carryable c in carryable)
            {
                Sellable sellable = c.GetComponent<Sellable>();
                if (sellable != null)
                {
                    c.SetInteractable(false);
                    _objectsToSell.Add(sellable);
                }
            }
        }
    }

    private void Sell(Sellable sellable)
    {
        if (_objectsToSell.Contains(sellable))
        {
            Vector3 startPoint = sellable.transform.position;
            _objectsToSell.Remove(sellable);
            sellable.PrepareToSell();
            
            DOTween.To(() => 0f, 
                    x => sellable.transform.position = EvaluateTrajectory(startPoint, x), 
                    1f, sellAnimDuration)
                .SetEase(sellEase)
                .OnComplete(() =>
                {
                    int price = Mathf.RoundToInt(sellable.Cost * GetSellPriceMultiplier());
                    _currencyService.AddCurrency(new CurrencyOperationData
                    {
                        Currency = CurrencyType.Coins,
                        Placement = CurrencyPlacementType.Level,
                        Amount = price,
                        UsePosition = true,
                        WorldPosition = currencySpawnPoint.position
                    });

                    sellable.Sell();
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

    private bool IsMarkerActive()
    {
        return _playerCarry.HasCarryable;
    }
}
