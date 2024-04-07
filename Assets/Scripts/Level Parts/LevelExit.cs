using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using ProjectConstants;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour, ILevelEventHandler, ILevelStartHandler, ILevelEndHandler
{
    [SerializeField] private float delay;
    [SerializeField] private Image progressUi;
    [SerializeField] private LayerMask mask;
        
    [InjectService] private ILevelManagerService _levelService;

    private bool _levelPlaying;
    private float _currentTime;
    private GameObject _player;
    
    public void OnLevelStarted(LevelProcessor.StartData data)
    {
        _levelPlaying = true;
    }

    public void OnLevelEnded(LevelProcessor.StopData data)
    {
        _levelPlaying = false;
    }
    
    public void OnLevelEvent(LevelEvent levelEvent)
    {
        if (levelEvent is LevelEventProgressComplete)
        {
            Debug.Log("LevelEventProgressComplete");
            gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        ServiceLocator.Instance.InjectServicesInComponent(this);
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (_player != null && _levelPlaying)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= delay)
            {
                Win();
                _currentTime = Mathf.Min(_currentTime, delay);
            }
        }
        else
        {
            _currentTime = 0f;
        }

        progressUi.fillAmount = _currentTime / delay;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_player == null && mask.HasLayer(other.gameObject.layer))
        {
            _player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_player != null && other.gameObject == _player)
        {
            _player = null;
        }
    }

    private void Win()
    {
        _levelService.TryStopCurrentLevel(new LevelProcessor.StopData(LevelStopType.Win));
    }
}
