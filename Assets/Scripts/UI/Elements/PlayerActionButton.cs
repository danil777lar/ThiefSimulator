using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core.Services.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerActionButton : MonoBehaviour
{
    [SerializeField] private Image progress; 
    [SerializeField] private Image icon;
    [SerializeField] private float scaleOnTouch;

    private bool _pointerDown;
    private float _time;
    private RectTransformEvents _events;
    private PlayerAction _action;
    
    public PlayerActionButton Build(PlayerAction action)
    {
        _action = action;

        icon.sprite = _action.Icon;

        _events = GetComponent<RectTransformEvents>();
        _events.EventPointerDown += OnPointerDown; 
        _events.EventPointerUp += OnPointerUp; 
        
        return this;
    }

    private void Update()
    {
        if (_pointerDown && _action.Enabled())
        {
            _time += Time.deltaTime;
            if (_time >= _action.Duration())
            {
                _time = 0f;
                _action.Action.Invoke();
            }
        }
        else
        {
            _time = 0f;
        }

        progress.fillAmount = _time / _action.Duration();
    }

    private void OnPointerDown(PointerEventData data)
    {
        _pointerDown = true;

        this.DOKill();
        transform.DOScale(scaleOnTouch, 0.1f)
            .SetEase(Ease.Linear)
            .SetTarget(this);
    }
    
    private void OnPointerUp(PointerEventData data)
    {
        _pointerDown = false;
        
        this.DOKill();
        transform.DOScale(1f, 0.25f)
            .SetEase(Ease.OutBack)
            .SetTarget(this);
    }
}
