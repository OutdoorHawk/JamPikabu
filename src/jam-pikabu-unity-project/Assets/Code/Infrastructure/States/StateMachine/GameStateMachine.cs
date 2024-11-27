using Code.Common.Logger.Service;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.StateInfrastructure;
using RSG;
using Zenject;

namespace Code.Infrastructure.States.StateMachine
{
    public class GameStateMachine : IGameStateMachine, ITickable, IFixedTickable
    {
        private readonly IStateFactory _stateFactory;
        private readonly ILoggerService _logger;

        private IExitableState _activeState;

        public GameStateMachine(IStateFactory stateFactory, ILoggerService logger)
        {
            _stateFactory = stateFactory;
            _logger = logger;
        }

        public IExitableState ActiveState => _activeState;

        public void Tick()
        {
            if (ActiveState is IUpdateable updateableState)
                updateableState.Update();
        }

        public void FixedTick()
        {
            if (ActiveState is IFixedUpdateable updateableState)
                updateableState.FixedUpdate();
        }

        public void Enter<TState>() where TState : class, IState
        {
            RequestEnter<TState>()
                .Done();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            RequestEnter<TState, TPayload>(payload)
                .Done();
        }

        private IPromise<TState> RequestEnter<TState>() where TState : class, IState
        {
            return RequestChangeState<TState>()
                .Then(EnterState);
        }

        private IPromise<TState> RequestEnter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            return RequestChangeState<TState>()
                .Then(state => EnterPayloadState(state, payload));
        }

        private TState EnterState<TState>(TState state) where TState : class, IState
        {
            _logger.Log($"<b>[GameState]</b> EnterState: {typeof(TState).Name}");
            _activeState = state;

            state.Enter();
            return state;
        }

        private TState EnterPayloadState<TState, TPayload>(TState state, TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            _activeState = state;

            state.Enter(payload);
            return state;
        }

        private IPromise<TState> RequestChangeState<TState>() where TState : class, IExitableState
        {
            if (ActiveState != null)
            {
                _logger.Log($"<b>[GameState]</b> Exit state: {ActiveState.GetType().Name}");
                return ActiveState
                    .BeginExit()
                    .Then(ActiveState.EndExit)
                    .Then(ChangeState<TState>);
            }
            
            return ChangeState<TState>();
        }


        private IPromise<TState> ChangeState<TState>() where TState : class, IExitableState
        {
            TState state = _stateFactory.GetState<TState>();
            _logger.Log($"<b>[GameState]</b> Entering state: {typeof(TState).Name}");
            
            return Promise<TState>.Resolved(state);
        }
    }
}