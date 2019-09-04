namespace BionicUtilities.NetStandard.Generic
{
  public interface INullObject<out TObject>
  {
    bool IsNull { get; }
    TObject NullObject { get; }
  }
}
