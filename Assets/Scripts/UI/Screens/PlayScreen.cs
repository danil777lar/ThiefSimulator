using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using ProjectConstants;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayScreen : UIScreen
{
    [SerializeField] private PlayerActionButton actionButtonPrefab; 
    [SerializeField] private RectTransform actionButtonsRoot; 
    
    [InjectService] private ILevelManagerService _levelService;

    private List<PlayerActionButton> _actionButtons;

    protected override void OnOpen(ScreenOpenProperties screenOpenProperties)
    {
        base.OnOpen(screenOpenProperties);
        ServiceLocator.Default.InjectServicesInComponent(this);

        InputManager input = GetComponent<InputManager>();
        Character player = FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList()
            .Find(x => x.PlayerID == input.PlayerID);

        if (player != null)
        {
            player.SetInputManager(input);
            
            actionButtonsRoot.MMDestroyAllChildren();
            _actionButtons = new List<PlayerActionButton>();
            IPlayerActionSource[] actionSources = player.GetComponents<IPlayerActionSource>();
            foreach (IPlayerActionSource source in actionSources)
            {
                foreach (PlayerAction playerAction in source.Actions)
                {
                    _actionButtons.Add(Instantiate(actionButtonPrefab, actionButtonsRoot).Build(playerAction));
                }   
            }
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        GetComponentInChildren<MMTouchFollowerJoystick>().OnPointerDown(pointerData);
    }

    private void Update()
    {
        bool blockUpdate = false;
        _actionButtons.ForEach(x => blockUpdate |= x.PointerDown);
        _actionButtons.ForEach(x => x.blockUpdateUI = blockUpdate);
    }

    public class Args : ScreenOpenProperties
    {
        public Args() : base(UIScreenType.Play)
        {
            
        }
    }
}
