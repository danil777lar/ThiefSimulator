using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ThiefTrapLaser : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform root;
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private GameObject colliders;
    [SerializeField] private LineRenderer[] lasers;

    [Header("Damage")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float damageInterval = 0.5f;

    [Header("Commands")]
    [SerializeField, SerializeReference] private List<Command> commands = new List<Command>();

    private bool _processing;
    private bool _isActive = true;
    private bool _inAPoint = true;
    private int _currentCommandIndex;
    private float _damageCooldown;

    private Dictionary<Collider, IDamageTarget> _damageTargets = new Dictionary<Collider, IDamageTarget>();

    private void Start()
    {
        _isActive = true;
        SetState(_isActive);

        _inAPoint = true;
        root.position = pointA.position;
    }

    private void Update()
    {
        if (_damageCooldown > 0f)
        {
            _damageCooldown -= Time.deltaTime;
        }

        if (commands.Count == 0 || _processing)
        {
            return;
        }

        _processing = true;
        ProcessCommand(commands[_currentCommandIndex]); 
        _currentCommandIndex = (_currentCommandIndex + 1) % commands.Count;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_isActive || _damageCooldown > 0f)
        {
            return;
        }

        IDamageTarget target = null;
        if (_damageTargets.TryGetValue(other, out IDamageTarget damageTarget))
        {
            target = damageTarget;
        }
        else if (other.TryGetComponent(out damageTarget))
        {
            _damageTargets[other] = damageTarget;
            target = damageTarget;
        }

        if (target != null)
        {
            Vector3 targetLocalPos = transform.InverseTransformPoint(target.gameObject.transform.position);
            Vector3 dir = transform.forward * (targetLocalPos.z > 0 ? 1 : -1);

            _damageCooldown = damageInterval;
            target.SendDamage(new DamageData
            {
                damage = this.damage,
                hitDirection = dir,
            });
        }
    }

    private void OnDrawGizmos()
    {
        if (commands.Find(command => command is MoveCommand) != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }

    private void OnValidate()
    {
        foreach (Command command in commands)
        {
            command?.Validate();
        }
    }

    private void ProcessCommand(Command command)
    {
        if (command is SwitchStateCommand)
        {
            _isActive = !_isActive;
            SetState(_isActive);
            _processing = false;
        }
        else if (command is WaitCommand waitCommand)
        {
            DOVirtual.DelayedCall(waitCommand.Delay, () => _processing = false);
        }
        else if (command is MoveCommand moveCommand)
        {
            _inAPoint = !_inAPoint;
            float distance = Vector3.Distance(pointA.position, pointB.position);
            root.DOMove(_inAPoint ? pointB.position : pointA.position, distance / moveCommand.Speed)
                .SetEase(moveCommand.Ease)
                .OnComplete(() => _processing = false);
        }
    }

    private void SetState(bool active)
    {
        colliders.SetActive(active);
        foreach (LineRenderer laser in lasers)
        {
            laser.enabled = active;
        }
    }

    [ContextMenu("Add Command Switch")]
    private void AddCommandSwitch()
    {
        commands.Add(new SwitchStateCommand());
    }

    [ContextMenu("Add Command Wait")]
    private void AddCommandWait()
    {
        commands.Add(new WaitCommand());
    }

    [ContextMenu("Add Command Move")]
    private void AddCommandMove()
    {
        commands.Add(new MoveCommand());
    }

    [Serializable]
    private abstract class Command
    {
        public abstract void Validate();
    }

    [Serializable]
    private class SwitchStateCommand : Command
    {
        [SerializeField, HideInInspector] public string inspectorName;

        public override void Validate()
        {
            inspectorName = "Switch";
        }
    }

    [Serializable]
    private class WaitCommand : Command
    {
        [SerializeField, HideInInspector] public string inspectorName;

        [field: SerializeField] public float Delay { get; private set; }

        public override void Validate()
        {
            inspectorName = $"Wait {Delay}s";
        }
    }

    [Serializable]
    private class MoveCommand : Command
    {
        [SerializeField, HideInInspector] public string inspectorName;

        [field: SerializeField] public float Speed { get; private set; } = 2f;
        [field: SerializeField] public Ease Ease { get; private set; } = Ease.InOutQuad;

        public override void Validate()
        {
            inspectorName = $"Move (Speed: {Speed}, Ease: {Ease})";
        }
    }
}
