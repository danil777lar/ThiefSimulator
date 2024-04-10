using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Larje.Core.Tools.TopDownEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.UI;

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
    private CoreCharacterMovement _movement;
    private TransitionPoint _nearestTransition;
    private Transform _nearestTransitionPoint;

    public PlayerAction[] Actions { get; private set; }

    protected override void Initialization()
    {
        base.Initialization();
        _movement = _character.FindAbility<CoreCharacterMovement>();
        BuildActions();
    }

    public override void ProcessAbility()
    {
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
        Dictionary<TransitionPoint, Collider> transitions = PhysicsUtility.FindObjectsInRange<TransitionPoint>
            (transform.position, findDistance, transitionMask, _controller3D.ObstaclesLayerMask);

        if (transitions.Count > 0)
        {
            _nearestTransition = transitions.Keys.OrderBy(x => 
                Vector3.Distance(transform.position, x.transform.position)).First();
            _nearestTransitionPoint = transitions[_nearestTransition].transform;
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
            TransitionPoint transit = _nearestTransition;
            Transform point = _nearestTransitionPoint;
            
            if (transit.TryGetStartAndEndPercent(point, out float start, out float end))
            {
                _character.MovementState.ChangeState(CharacterStates.MovementStates.Transition);
                _inTransition = true;
                this.DOKill();
                DOTween.To(() => start,
                        (v) =>
                        {
                            _character.CharacterModel.transform.position = transit.EvaluatePosition(v);
                        }, 
                        end, transit.Duration)
                    .SetTarget(this)
                    .SetUpdate(UpdateType.Fixed)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        transform.position = _character.CharacterModel.transform.position;
                        _character.CharacterModel.transform.localPosition = Vector3.zero;
                        _inTransition = false;
                        _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
                    });
            }
        }
    }
}
