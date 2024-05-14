using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

public class ItemAccessory : MonoBehaviour
{
    [field: SerializeField] public ItemType ItemType { get; private set; }
    [field: SerializeField] public string Key { get; private set; }
}
