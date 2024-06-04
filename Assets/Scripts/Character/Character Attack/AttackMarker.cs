using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Networking;

public class AttackMarker : MonoBehaviour
{
    [SerializeField] private float showHideAnimDuration;
    [SerializeField] private LayerMask mask;

    private float _distance;
    private float _angle;
    private Vector3 _lastPosition;
    private Quaternion _lastRotation;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private CharacterAttack _attacker;
    private CharacterAttack _target;

    public void Init(float distance, float angle, Vector3 direction, CharacterAttack attacker, CharacterAttack target)
    {
        _distance = distance;
        _angle = angle;
        _attacker = attacker;
        _target = target;
        
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
     
        transform.localPosition = transform.localPosition.MMSetY(0.05f);
        transform.localRotation = Quaternion.LookRotation(direction);
        transform.localScale = Vector3.zero;
        
        this.DOKill();
        transform.DOScale(Vector3.one, showHideAnimDuration)
            .SetTarget(this)
            .SetEase(Ease.OutBack);
    }
    
    public void Remove()
    {
        this.DOKill();
        transform.DOScale(Vector3.zero, showHideAnimDuration)
            .SetTarget(this)
            .SetEase(Ease.InQuad)
            .OnComplete(() => Destroy(gameObject));
    }

    private void Update()
    {
        UpdateMesh();
        UpdateProgress();
    }

    private void UpdateMesh()
    {
        FovMeshBuilder.Input input = new FovMeshBuilder.Input
        {
            angle = _angle,
            directionRotate = 180f,
            raysPerDeg = 1f,
            distance = _distance,
            meshFilter = _meshFilter,
            raycastMask = mask,
        };
        FovMeshBuilder.BuildMesh(input);
    }

    private void UpdateProgress()
    {
        float targetPercent = _attacker.Target == _target ? _attacker.AttackProgress : 0f;
        float currentPercent = _meshRenderer.material.GetFloat("_FillPercent");
        targetPercent = Mathf.Lerp(currentPercent, targetPercent, Time.deltaTime * 10f);
        _meshRenderer.material.SetFloat("_FillPercent", targetPercent);
    }
}
