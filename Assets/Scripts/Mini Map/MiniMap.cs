using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private RawImage mapImage;
    
    private void Start()
    {
        mapImage.texture = MiniMapCamera.Instance.OutTexture;
    }
}
