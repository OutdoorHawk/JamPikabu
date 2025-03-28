using RSG;

namespace Code.Infrastructure.States.StateInfrastructure
{
  public class EndOfFrameExitState : IState, IUpdateable, IFixedUpdateable
  {
    private Promise _exitPromise;

    protected bool ExitWasRequested =>
      _exitPromise != null;

    public virtual void Enter()
    {
    }

    IPromise IExitableState.BeginExit()
    {
      _exitPromise = new Promise();
      return _exitPromise;
    }

    void IExitableState.EndExit()
    {
      ExitOnEndOfFrame();
      ClearExitPromise();
    }

    void IUpdateable.Update()
    {
      // THIS IS WRONG, since it will exit on the next frame instead of the end of current frame
      // if (!IsExitRequested)
      //   OnUpdate();
      // else
      //   ResolveExitPromise();

      if (!ExitWasRequested)
        OnUpdate();
      
      if (ExitWasRequested) 
        ResolveExitPromise();
    }

    void IFixedUpdateable.FixedUpdate()
    {
      if (!ExitWasRequested)
        OnFixedUpdate();
    }

    protected virtual void ExitOnEndOfFrame()
    {
      
    }

    protected virtual void OnUpdate(){}
    protected virtual void OnFixedUpdate(){}

    private void ClearExitPromise() =>
      _exitPromise = null;

    private void ResolveExitPromise() =>
      _exitPromise?.Resolve();
  }
}