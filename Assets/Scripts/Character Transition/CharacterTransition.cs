using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterTransition : CharacterAbility, IPlayerActionSource
{
    [SerializeField] private float findDistance;
    [SerializeField] private LayerMask transitionMask;
    [Header("Gizmos")]
    [SerializeField] private bool drawGizmos;
    [SerializeField] private Color gizmosColor;
    [Header("Action Icons")] 
    [SerializeField] private Sprite transitIcon;

    private bool _inTransition;
    private ActualSpeedCharacterMovement _movement;
    private TransitionPoint _nearestTransition;

    public PlayerAction[] Actions { get; private set; }

    protected override void Initialization()
    {
        base.Initialization();
        _movement = _character.FindAbility<ActualSpeedCharacterMovement>();
        BuildActions();
    }

    public override void ProcessAbility()
    {
        base.ProcessAbility();
        TryFindTransition();
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawWireSphere(transform.position, findDistance);
        }   
    }

    private void BuildActions()
    {
        List<PlayerAction> actions = new List<PlayerAction>();
        actions.Add(new PlayerAction(TryTransit, CanTransit, () => 0.2f, transitIcon));
        Actions = actions.ToArray();
    }

    private void TryFindTransition()
    {
        _nearestTransition = null;
        List<TransitionPoint> transitions = new List<TransitionPoint>();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, findDistance, transitionMask);
        foreach (Collider cargoCollider in  colliders)
        {
            if (cargoCollider.attachedRigidbody != null 
                && cargoCollider.attachedRigidbody.TryGetComponent(out TransitionPoint cargo))
            {
                transitions.Add(cargo);
            }
        }

        if (transitions.Count > 0)
        {
            _nearestTransition = transitions.OrderBy(x => 
                Vector3.Distance(transform.position, x.transform.position)).First();
        }
    }

    private bool CanTransit()
    {
        return _nearestTransition != null && !_inTransition;
    }

    private void TryTransit()
    {
        if (CanTransit())
        {
            _inTransition = true;
            this.DOKill();
            _character.CharacterModel.transform
                .DOMove(_nearestTransition.TransitTo.position, 0.5f)
                .SetTarget(this)
                .OnComplete(() =>
                {
                    transform.position = _character.CharacterModel.transform.position;
                    _character.CharacterModel.transform.localPosition = Vector3.zero;
                    _inTransition = false;
                });
            
        }
    }
}
