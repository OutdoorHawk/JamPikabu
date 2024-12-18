using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Common.Extensions.Animations;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows.Service;
using Code.Meta.Features.MainMenu.Windows;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Meta.Features.LootCollection.Behaviours
{
    public class IngredientUnlockBehaviour : MonoBehaviour
    {
        public Button UnlockButton;
        public Image IngredientIcon;
        public UIShiny IngredientShiny;
        public TMP_Text ReadyToUnlockText;
        public TMP_Text LockedText;
        public Animator UnlockIngredientAnimator;
        public TMP_Text TimerText;

        private LootTypeId _unlocksIngredient;

        private IStaticDataService _staticData;
        private IWindowService _windowService;

        [Inject]
        private void Construct(IStaticDataService staticData, IWindowService windowService)
        {
            _windowService = windowService;
            _staticData = staticData;
        }

        private void Awake()
        {
            gameObject.DisableElement();
        }

        private void Start()
        {
            UnlockButton.onClick.AddListener(ProceedUnlock);
        }

        private void OnDestroy()
        {
            UnlockButton.onClick.RemoveAllListeners();
        }

        public void InitReadyToCollect(LootTypeId unlocksIngredient)
        {
            UnlockButton.interactable = true;
            LockedText.DisableElement();
            ReadyToUnlockText.EnableElement();
            UnlockIngredientAnimator.SetBehaviorEnabled();
            IngredientShiny.SetBehaviorEnabled();
            Init(unlocksIngredient);
        }

        public void InitLocked(LootTypeId unlocksIngredient)
        {
            UnlockButton.interactable = false;
            UnlockIngredientAnimator.SetBehaviorDisabled();
            LockedText.EnableElement();
            ReadyToUnlockText.DisableElement();
            IngredientShiny.SetBehaviorDisabled();
            Init(unlocksIngredient);
        }

        private void Init(LootTypeId unlocksIngredient)
        {
            gameObject.EnableElement();
            _unlocksIngredient = unlocksIngredient;
            var lootSettings = _staticData.GetStaticData<LootSettingsStaticData>();
            IngredientIcon.sprite = lootSettings.GetConfig(_unlocksIngredient).Icon;
        }

        private void ProceedUnlock()
        {
            if (_unlocksIngredient is LootTypeId.Unknown)
                return;

            UnlockButton.interactable = false;
            UnlockIngredient();
            CollectNewIngredient().Forget();
        }

        private void UnlockIngredient()
        {
            CreateMetaEntity.Empty()
                .With(x => x.isUnlockLootRequest = true)
                .AddLootTypeId(_unlocksIngredient);
        }

        private async UniTaskVoid CollectNewIngredient()
        {
            await UnlockIngredientAnimator.WaitForAnimationCompleteAsync(AnimationParameter.Collect.AsHash(), destroyCancellationToken);

            if (_windowService.TryGetWindow(out MainMenuWindow mainMenuWindow) == false)
                return;
            
            Transform shopButton = mainMenuWindow.ShopButton.transform;
            
           await IngredientIcon.transform
                .DOJump(shopButton.position, 2, 1, 1)
                .SetLink(IngredientIcon.gameObject)
                .AsyncWaitForCompletion()
                ;
           
           gameObject.DisableElement();
        }
    }
}