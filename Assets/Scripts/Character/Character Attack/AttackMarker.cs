using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class AttackMarker : MonoBehaviour
{
    [SerializeField] private float showHideAnimDuration;
    [SerializeField] private LayerMask mask;

    private float _distance;
    private float _angle;
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
        FovMeshBuilder.Input input = new FovMeshBuilder.Input
        {
            angle = _angle,
            directionRotate = 180f,
            raysPerAngle = 1f,
            distance = _distance,
            meshFilter = _meshFilter,
            raycastMask = mask,
        };
        FovMeshBuilder.Output output = FovMeshBuilder.BuildMesh(input);

        float percent = _attacker.Target == _target ? _attacker.AttackProgress : 0f;
        _meshRenderer.material.SetFloat("_FillPercent", percent);
    }
}
