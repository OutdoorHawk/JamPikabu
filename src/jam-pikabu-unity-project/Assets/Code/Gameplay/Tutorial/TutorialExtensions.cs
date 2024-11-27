using System.Threading;
using Code.Gameplay.Tutorial.Window;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Code.Gameplay.Tutorial
{
    public static class TutorialExtensions
    {
        public static async UniTask SetArrowOnButtonAndWaitForClick
        (
            this TutorialWindow tutorialWindow,
            Button button,
            CancellationToken token,
            ArrowRotation rotation = ArrowRotation.Top,
            float xOffset = 0,
            float yOffset = 150
        )
        {
            tutorialWindow.ShowArrow(button.transform, xOffset, yOffset, rotation);
            await button.OnClickAsync(token);
        }
    }
}