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
    [InjectService] private PlayerInputService _inputService;

    private List<PlayerActionButton> _actionButtons;

    protected override void OnBeforeOpen(UIObject.Args args)
    {
        ServiceLocator.Default.InjectServicesInComponent(this);
        
        _inputService.ConnectPlayer();
        Character player = FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList()
            .Find(x => x.CharacterType == Character.CharacterTypes.Player);
        if (player != null)
        {
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
    }

    private void Update()
    {
        bool blockUpdate = false;
        _actionButtons.ForEach(x => blockUpdate |= x.PointerDown);
        _actionButtons.ForEach(x => x.blockUpdateUI = blockUpdate);
    }

    public class Args : UIScreen.Args
    {
        public Args() : base(UIScreenType.Play)
        {
            
        }
    }
}
