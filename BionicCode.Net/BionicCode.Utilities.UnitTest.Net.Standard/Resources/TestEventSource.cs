using System;

namespace BionicCode.Utilities.UnitTest.Net.Standard.Resources
{
  public class TestEventArgs : EventArgs
  {
    
  }

  public class TestEventSource
  {
    public event EventHandler TestEvent;
    public event EventHandler<TestEventArgs> GenericTestEvent;

    public virtual void OnTestEvent()
    {
      this.TestEvent?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnGenericTestEvent()
    {
      this.GenericTestEvent?.Invoke(this, new TestEventArgs());
    }

    public virtual void RaiseAll()
    {
      OnTestEvent();
      OnGenericTestEvent();
    }
  }

  public class TestEventSource2
  {
    public event EventHandler TestEvent;
    public event EventHandler<TestEventArgs> GenericTestEvent;

    public virtual void OnTestEvent()
    {
      this.TestEvent?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnGenericTestEvent()
    {
      this.GenericTestEvent?.Invoke(this, new TestEventArgs());
    }

    public virtual void RaiseAll()
    {
      OnTestEvent();
      OnGenericTestEvent();
    }
  }
}
