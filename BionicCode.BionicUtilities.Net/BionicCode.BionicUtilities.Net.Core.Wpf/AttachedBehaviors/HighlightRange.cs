namespace BionicCode.BionicUtilities.Net.Core.Wpf.AttachedBehaviors
{
  public struct HighlightRange
  {
    public HighlightRange(int startIndex, int endIndex)
    {
      this.StartIndex = startIndex;
      this.EndIndex = endIndex;
    }

    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
  }
}
