using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using UnityEngine;
using UnityEngine.Rendering;

public class ThiefLevelBaker : MonoBehaviour
{
    [SerializeField] private LayerMask mask;
    
    private MB3_MeshBaker _baker;
    
    private void Start()
    {
        List<GameObject> objectsToBake = new List<GameObject>();
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            if (mask.HasLayer(meshRenderer.gameObject.layer))
            {
                objectsToBake.Add(meshRenderer.gameObject);
            }   
        }
        
        StartCoroutine(BakeDelayed(objectsToBake));
    }
    
    private void Bake(List<GameObject> objectsToBake)
    {
        _baker = GetComponent<MB3_MeshBaker>();
        if (_baker == null)
        {
            _baker = gameObject.AddComponent<MB3_MeshBaker>();
        }

        _baker.ClearMesh();
        _baker.meshCombiner.renderType = MB_RenderType.skinnedMeshRenderer;
        _baker.meshCombiner.doUV = false;
        _baker.meshCombiner.doTan = false;
        _baker.meshCombiner.doCol = true;
        _baker.meshCombiner.optimizeAfterBake = true;

        if (objectsToBake.Count > 0)
        {
            _baker.AddDeleteGameObjects(objectsToBake.ToArray(), null, true);
            _baker.Apply();

            //baker.meshCombiner.targetRenderer.enabled = false;
            if ((SkinnedMeshRenderer)_baker.meshCombiner.targetRenderer)
            {
                ((SkinnedMeshRenderer)_baker.meshCombiner.targetRenderer).shadowCastingMode = ShadowCastingMode.Off;
                ((SkinnedMeshRenderer)_baker.meshCombiner.targetRenderer).localBounds =
                    new Bounds(transform.position, new Vector3(100, 50, 100));
            }
            
            _baker.meshCombiner.targetRenderer.transform.parent.SetParent(transform);
        }
    }

    private IEnumerator BakeDelayed(List<GameObject> objectsToBake)
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        
        Bake(objectsToBake);
    }
}
