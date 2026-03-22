using System.Collections;
using System.Collections.Generic;
using Larje.Character;
using UnityEngine;

public class MiniMapMarker : MonoBehaviour
{
    [SerializeField] private GameObject defaultMarker;
    [SerializeField] private GameObject deadMarker;

    private Health _health;
    
    private void Start()
    {
        Character character = GetComponentInParent<Character>();
        _health = character.GetComponentInChildren<Health>();
    }
    
    private void Update()
    {
        defaultMarker.SetActive(_health.CurrentHealth > 0);        
        deadMarker.SetActive(_health.CurrentHealth <= 0);        
    }
}
