namespace Hell.LogDown.Settings
{
  public class DataChangedEventArgs<TData> : DataChangedEventArgs
  {
    public DataChangedEventArgs()
    { }

    public DataChangedEventArgs(TData data)
    {
      this.OldData = default(TData);
      this.NewData = data;
    }

    public DataChangedEventArgs(TData oldOldData, TData newData)
    {
      this.OldData = oldOldData;
      this.NewData = newData;
    }

    public TData NewData { get; set; }
    public TData OldData { get; set; }
  }
}