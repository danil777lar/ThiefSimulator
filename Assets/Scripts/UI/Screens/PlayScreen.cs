using System.Collections;
using System.Collections.Generic;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

public class PlayScreen : UIScreen
{
    [InjectService] private ILevelManagerService _levelService;

    protected override void OnOpen(ScreenOpenProperties screenOpenProperties)
    {
        base.OnOpen(screenOpenProperties);
        ServiceLocator.Default.InjectServicesInComponent(this);
    }

    public class Args : ScreenOpenProperties
    {
        public Args() : base(UIScreenType.Play)
        {
            
        }
    }
}
