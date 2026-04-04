
using DG.Tweening;
using Larje.Character;
using Larje.Core;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Transform barRoot;
    [SerializeField] private Slider mainSlider;
    [SerializeField] private Slider secondarySlider;
    [Space]
    [SerializeField] private float lifetime = 1f;

    [InjectService] private IPlayerProviderService _playerProvider;

    private float _currentLifetime;

    private Health _playerHealth;
    private Camera _mainCamera;

    private void Awake()
    {
        DIContainer.InjectTo(this);

        _mainCamera = Camera.main;
        barRoot.gameObject.SetActive(false);

        if (_playerProvider.TryGetPlayer(out Character player))
        {
            _playerHealth = player.Health;
            _playerHealth.EventDamage += OnDamageTaken;
        }

        mainSlider.value = 1f;
        secondarySlider.value = 1f;
    }

    private void Update()
    {
        if (_currentLifetime > 0)
        {
            secondarySlider.value = Mathf.Lerp(secondarySlider.value, _playerHealth.HealthPercent, Time.deltaTime * 2f);

            _currentLifetime -= Time.deltaTime;
            if (_currentLifetime <= 0)
            {
                barRoot.gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetPositon = _mainCamera.WorldToScreenPoint(_playerHealth.transform.position + Vector3.up * -1f);
        barRoot.transform.position = Vector3.Lerp(barRoot.transform.position, targetPositon, Time.deltaTime * 10f);
    }

    private void OnDamageTaken(DamageData damage)
    {
        barRoot.gameObject.SetActive(true);
        mainSlider.value = _playerHealth.HealthPercent;

        if (_currentLifetime <= 0f)
        {
            this.DOKill();

            barRoot.transform.localScale = new Vector3(0f, 0f, 1f);

            Sequence seq = DOTween.Sequence().SetTarget(this);
            seq.Append(barRoot.transform.DOScaleY(1f, 0.2f).SetEase(Ease.InQuad).SetTarget(this));
            seq.Join(barRoot.transform.DOScaleX(1f, 0.4f).SetEase(Ease.OutBack).SetTarget(this));
            seq.AppendCallback(() => _currentLifetime = lifetime);
        }
        else
        {
            _currentLifetime = lifetime;
        }
    }
}
