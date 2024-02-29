using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core.Services.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerActionButton : MonoBehaviour
{
    [HideInInspector] public bool blockUpdateUI;
    
    [SerializeField] private Image progress; 
    [SerializeField] private Image icon;
    [SerializeField] private float scaleOnTouch;

    private bool _pointerDown;
    private float _time;
    
    private PlayerAction _action;
    private RectTransformEvents _events;
    private LayoutElement _layoutElement;
    private CanvasGroup _canvasGroup;

    public bool PointerDown => _pointerDown;

    public PlayerActionButton Build(PlayerAction action)
    {
        _action = action;

        icon.sprite = _action.Icon;

        _events = GetComponent<RectTransformEvents>();
        _layoutElement = GetComponent<LayoutElement>();
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _events.EventPointerDown += OnPointerDown; 
        _events.EventPointerUp += OnPointerUp; 
        
        UpdateUI();
        
        return this;
    }

    private void Update()
    {
        UpdateUI();
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

    private void UpdateUI()
    {
        if (!blockUpdateUI)
        {
            _layoutElement.ignoreLayout = !_action.Enabled();
            _canvasGroup.alpha = _action.Enabled() ? 1f : 0f;
            _canvasGroup.blocksRaycasts = _action.Enabled();
        }
    }
}
