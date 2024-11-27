using Code.Gameplay.Cheats.Cheats.Abstract;
using Code.Infrastructure.DI.Installers;
using Code.Infrastructure.States.GameStateHandler;
using Code.Progress.Writer;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Cheats.Cheats
{
    [Injectable(typeof(ICheatAction))]
    public class WipeCheat : BaseCheat, ICheatActionBasic
    {
        private IProgressReadWrite _readWrite;
        public string CheatLabel => "Delete all progress";

        public OrderType Order => OrderType.Last;

        [Inject]
        private void Construct
        (
            IProgressReadWrite readWrite
        )
        {
            _readWrite = readWrite;
        }

        public void Execute()
        {
            _readWrite.DeleteProgress();
            
            Application.Quit();

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
        }
    }
}