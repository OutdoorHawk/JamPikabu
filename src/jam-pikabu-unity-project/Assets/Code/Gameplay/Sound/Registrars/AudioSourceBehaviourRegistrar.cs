using Code.Gameplay.Sound.Behaviours;
using Code.Infrastructure.View.Registrars;

namespace Code.Gameplay.Sound.Registrars
{
    public class AudioSourceBehaviourRegistrar : EntityComponentRegistrar
    {
        public AudioSourceBehaviour audioSourceBehaviour;

        public override void RegisterComponents()
        {
           // Entity.AddAudioSourceBehaviour(audioSourceBehaviour);
        }

        public override void UnregisterComponents()
        {
            /*if (Entity.hasAudioSourceBehaviour)
                Entity.RemoveAudioSourceBehaviour();*/
        }
    }
}