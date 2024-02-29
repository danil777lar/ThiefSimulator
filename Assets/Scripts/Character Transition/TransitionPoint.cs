using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    [SerializeField] private Transform transitTo;

    public Transform TransitTo => transitTo;
}
