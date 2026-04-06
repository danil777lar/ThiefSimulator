using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : UIScreen
{
        [Space] 
        [SerializeField] private Slider loadingProgress;
        [SerializeField] private TextMeshProUGUI loadingPercent;

        private Args _args;

        private void Awake()
        {
            DIContainer.InjectTo(this);
        }

        protected override void OnBeforeOpen(UIObject.Args rawArgs)
        {
            if (rawArgs is Args args)
            {
                _args = args;
            }
        }

        private void Update()
        {
            if (loadingProgress != null)
            {
                loadingProgress.value = _args.GetProgress();
            }

            if (loadingPercent != null)
            {
                loadingPercent.text = $"{Mathf.Floor(Mathf.Lerp(0, 100, _args.GetProgress()))}%";
            }
        }

        public class Args : UIScreen.Args
        {
            public readonly Func<float> GetProgress;

            public Args(Func<float> getProgress) : base(UIScreenType.Loading)
            {
                GetProgress = getProgress;
            }
        }
}
