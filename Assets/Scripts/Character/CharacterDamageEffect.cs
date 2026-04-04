using DG.Tweening;
using Larje.Character;
using UnityEngine;

public class CharacterDamageEffect : MonoBehaviour
{
    [Header("Material")]
    [SerializeField] private bool changeMaterial = true;
    [SerializeField] private float materialChangeDuration = 0.2f;
    [SerializeField] private Material damageMaterial;

    [Header("Shake")]
    [SerializeField] private bool useCameraShake = false;

    [Header("Sound")]
    [SerializeField] private SoundSettings damageSound;

    private Character _character;
    private MaterialSetter _materialSetter;

    private void Start()
    {
        _character = GetComponentInParent<Character>();
        _materialSetter = _character.GetComponentInChildren<MaterialSetter>();

        _character.Health.EventDamage += OnDamageTaken;
    }

    private void OnDamageTaken(DamageData damage)
    {
        if (changeMaterial)
        {
            _materialSetter.AddMaterialOverride(this, damageMaterial);
            DOVirtual.DelayedCall(materialChangeDuration, () => _materialSetter.RemoveMaterialOverride(this));
        }

        if (useCameraShake)
        {
        }

        damageSound.Play();
    }
}
