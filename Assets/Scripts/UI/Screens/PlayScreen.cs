using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;

public class PlayScreen : UIScreen
{
    [InjectService] private ILevelManagerService _levelService;

    protected override void OnOpen(ScreenOpenProperties screenOpenProperties)
    {
        base.OnOpen(screenOpenProperties);
        ServiceLocator.Default.InjectServicesInComponent(this);

        InputManager input = GetComponent<InputManager>();
        foreach (Character character in FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (character.PlayerID == input.PlayerID)
            {
                character.SetInputManager(input);       
            }
        }
    }

    public class Args : ScreenOpenProperties
    {
        public Args() : base(UIScreenType.Play)
        {
            
        }
    }
}
