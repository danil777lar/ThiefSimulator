using System;
using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;
using UnityEngine.UI;

public class TestMiniGamePopup : MiniGamePopup
{
    [SerializeField] private Button completeButton;
    [SerializeField] private Button failButton;
    [SerializeField] private Button exitButton;

    private MiniGameArgs _args;
    
    protected override void OnBeforeOpen(UIObject.Args args)
    {
        if (args is MiniGameArgs miniGameArgs)
        {
            _args = miniGameArgs;
            completeButton.onClick.AddListener(Complete);
            failButton.onClick.AddListener(Fail);
            exitButton.onClick.AddListener(Exit);   
        }
    }

    private void Complete()
    {
        _args.OnComplete?.Invoke();
        Close();
    }
    
    private void Fail()
    {
        _args.OnFail?.Invoke();
        Close();
    }
    
    private void Exit()
    {
        Close();
    }

    public class Args : MiniGamePopup.MiniGameArgs
    {
        public Args(Action onComplete, Action onFail) : base(UIPopupType.TestMiniGame, onComplete, onFail)
        {
        }
    }    
}
