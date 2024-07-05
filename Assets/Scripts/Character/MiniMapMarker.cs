using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class MiniMapMarker : MonoBehaviour
{
    [SerializeField] private GameObject defaultMarker;
    [SerializeField] private GameObject deadMarker;

    private CharacterHealth _health;
    
    private void Start()
    {
        Character character = GetComponentInParent<Character>();
        _health = character.CharacterHealth as CharacterHealth;
    }
    
    private void Update()
    {
        defaultMarker.SetActive(_health.CurrentHealth > 0);        
        deadMarker.SetActive(_health.CurrentHealth <= 0);        
    }
}
