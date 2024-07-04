using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private RawImage mapImageStatic;
    [SerializeField] private RawImage mapImageDynamic;
    
    private void Start()
    {
        mapImageStatic.texture = MiniMapCamera.Instance.StaticTexture;
        mapImageDynamic.texture = MiniMapCamera.Instance.DynamicTexture;
    }
}
