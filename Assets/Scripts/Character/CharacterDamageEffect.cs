using DG.Tweening;
using Larje.Character;
using Unity.Cinemachine;
using UnityEngine;

public class CharacterDamageEffect : MonoBehaviour
{
    [Header("Material")]
    [SerializeField] private bool changeMaterial = true;
    [SerializeField] private float materialChangeDuration = 0.2f;
    [SerializeField] private Material damageMaterial;

    [Header("Shake")]
    [SerializeField] private bool useCameraShake = false;
    [SerializeField] private float shakeIntensity = 1f;

    [Header("Sound")]
    [SerializeField] private SoundSettings damageSound;

    private Character _character;
    private MaterialSetter _materialSetter;
    private CinemachineImpulseSource _impulseSource;

    private void Start()
    {
        _character = GetComponentInParent<Character>();
        _materialSetter = _character.GetComponentInChildren<MaterialSetter>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();

        _character.Health.EventDamage += OnDamageTaken;
    }

    private void OnDamageTaken(DamageData damage)
    {
        if (changeMaterial && _materialSetter != null)
        {
            _materialSetter.AddMaterialOverride(this, damageMaterial);
            DOVirtual.DelayedCall(materialChangeDuration, () => _materialSetter.RemoveMaterialOverride(this));
        }

        if (useCameraShake && _impulseSource != null)
        {
            Vector2 impulse = Vector2.zero;
            impulse.x = Random.Range(-1f, 1f);
            impulse.y = Random.Range(-1f, 1f);
            impulse *= shakeIntensity;

            _impulseSource.GenerateImpulse(impulse);
        }

        damageSound.Play();
    }
}
