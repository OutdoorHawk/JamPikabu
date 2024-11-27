namespace Code.Infrastructure.States.StateInfrastructure
{
  public interface IUpdateable
  {
    void Update();
  }
  
  public interface IFixedUpdateable
  {
    void FixedUpdate();
  }
}