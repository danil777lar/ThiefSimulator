using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreen : UIScreen
{
    [Space(40f)] 
    [SerializeField] private Button exitButton;

    [Header("Preview")] 
    [SerializeField] private Camera previewCamera;
    [SerializeField] private RawImage previewImage;

    private RenderTexture _previewTexture;
    
    protected override void OnBeforeOpen(UIObject.Args screenOpenProperties)
    {
        exitButton.onClick.AddListener(OnExitButtonClicked);
        SetPreview();
    }

    private void SetPreview()
    {
        _previewTexture = new RenderTexture((int)previewImage.rectTransform.rect.width, 
            (int)previewImage.rectTransform.rect.height, 24);
        previewImage.texture = _previewTexture;
        previewCamera.targetTexture = _previewTexture;
    }
    
    private void OnExitButtonClicked()
    {
        Back();
    }
    
    public new class Args : UIScreen.Args
    {
        public Args() : base(UIScreenType.Shop)
        {
        }
    }
}
