using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Character;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    private Dictionary<Renderer, List<Material>> _renderers;
    private Dictionary<object, Material> _overrides = new Dictionary<object, Material>();

    private void Start()
    {
        GrabRenderers();
    }

    private void GrabRenderers()
    {
        Character character = GetComponentInParent<Character>();

        List<Renderer> renderers = new List<Renderer>();
        renderers.AddRange(character.GetComponentsInChildren<MeshRenderer>(true));
        renderers.AddRange(character.GetComponentsInChildren<SkinnedMeshRenderer>(true));
        
        _renderers = new Dictionary<Renderer, List<Material>>();
        foreach (Renderer renderer in renderers)
        {
            _renderers.Add(renderer, renderer.materials.ToList());
        }
    }

    public void AddMaterialOverride(object source, Material material)
    {
        if (_overrides.ContainsKey(source))
        {
            _overrides.Remove(source);
        }
        
        _overrides.Add(source, material);
        ApplyMaterial();
    }
    
    public void RemoveMaterialOverride(object source)
    {
        if (_overrides.ContainsKey(source))
        {
            _overrides.Remove(source);
        }
        
        ApplyMaterial();
    }

    private void ApplyMaterial()
    {
        if (_overrides.Count > 0)
        {
            _renderers.Keys.ToList().ForEach(renderer =>
            {
                List<Material> materials = new List<Material>();
                for (int i = 0; i < _renderers[renderer].Count; i++)
                {
                    materials.Add(_overrides.Last().Value);
                }
                renderer.materials = materials.ToArray();
            });
        }
        else
        {
            _renderers.Keys.ToList().ForEach(renderer =>
            {
                renderer.materials = _renderers[renderer].ToArray();
            });
        }
    }
}
