namespace BionicCode.Utilities.NetStandard.Generic
{
  public interface IFactory<out TCreate>
  {
    TCreate Create();
    TCreate Create(params object[] args);
  }
}
