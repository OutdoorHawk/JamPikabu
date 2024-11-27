namespace SkyCastleSkillbox.Input
{
    public class PlayerInput
    {
        private readonly InputActions _inputActions;

        public PlayerInput()
        {
            _inputActions = new InputActions();
        }

        public void EnableAllInput()
        {
            _inputActions.Player.Enable();
        }
    }
}