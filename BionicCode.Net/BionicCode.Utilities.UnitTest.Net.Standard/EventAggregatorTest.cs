using System;
using BionicCode.Utilities.Net.Standard;
using BionicCode.Utilities.UnitTest.Net.Standard.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BionicCode.Utilities.UnitTest.Net.Standard
{
  
  [TestClass]
  public class EventAggregatorTest
  {

    [TestInitialize]
    public void Initialize()
    {
      this.NonGenericEventInvocationCount = 0;
      this.GenericEventInvocationCount = 0;
      this.EventManager = new EventAggregator();
      this.EventSource1 = new TestEventSource();
      this.EventSource2 = new TestEventSource2();

      this.EventManager.TryRegisterObservable(this.EventSource1, new[] { nameof(this.EventSource1.TestEvent), nameof(this.EventSource1.GenericTestEvent) });

      this.EventManager.TryRegisterObservable(this.EventSource2, new[] { nameof(this.EventSource2.TestEvent), nameof(this.EventSource2.GenericTestEvent) });
    }

    [TestMethod]
    public void Handle4EventsOfSpecificEventOfKnownSource()
    {
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource2.TestEvent), this.EventSource2.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), this.EventSource2.GetType(), OnGenericTestEvent);

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(2, this.NonGenericEventInvocationCount);
      Assert.AreEqual(2, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void Handle4EventsOfSpecificEventOfUnknownSource()
    {
      this.EventManager.TryRegisterGlobalObserver<EventArgs>(nameof(this.EventSource1.TestEvent), OnTestEvent);
      this.EventManager.TryRegisterGlobalObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), OnGenericTestEvent);

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(2, this.NonGenericEventInvocationCount);
      Assert.AreEqual(2, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void Handle4EventsOfUnspecificEventOfUnknownSourceButSpecificHandler()
    {
      this.EventManager.TryRegisterGlobalObserver<EventArgs>(OnTestEvent);
      this.EventManager.TryRegisterGlobalObserver<TestEventArgs>(OnGenericTestEvent);

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(2, this.NonGenericEventInvocationCount);
      Assert.AreEqual(2, this.GenericEventInvocationCount); 
    }

    [TestMethod]
    public void Handle2EventsAfterUnsubscribingFromAllSpecificEventsOfKnownSource()
    {
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);

      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource2.TestEvent), this.EventSource2.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), this.EventSource2.GetType(), OnGenericTestEvent);

      this.EventManager.TryRemoveObserver<EventArgs>(
        nameof(this.EventSource1.TestEvent),
        this.EventSource1.GetType(),
        OnTestEvent);
      this.EventManager.TryRemoveObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(1, this.NonGenericEventInvocationCount);
      Assert.AreEqual(1, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void HandleNoEventsAfterUnsubscribingFromSpecificEventsOfAllUnknownSource()
    {
      this.EventManager.TryRegisterGlobalObserver<EventArgs>(nameof(this.EventSource1.TestEvent), OnTestEvent);
      this.EventManager.TryRegisterGlobalObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), OnGenericTestEvent);

      this.EventManager.TryRegisterGlobalObserver<EventArgs>(nameof(this.EventSource2.TestEvent), OnTestEvent);
      this.EventManager.TryRegisterGlobalObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), OnGenericTestEvent);

      this.EventManager.TryRemoveGlobalObserver<EventArgs>(
        nameof(this.EventSource1.TestEvent),
        OnTestEvent);
      this.EventManager.TryRemoveGlobalObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), OnGenericTestEvent);

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(0, this.NonGenericEventInvocationCount);
      Assert.AreEqual(0, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void HandleNoEventsAfterUnsubscribingFromUnspecificEventOfAllUnknownSourcesButSpecificHandlers()
    {
      this.EventManager.TryRegisterGlobalObserver<EventArgs>(nameof(this.EventSource1.TestEvent), OnTestEvent);
      this.EventManager.TryRegisterGlobalObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), OnGenericTestEvent);

      this.EventManager.TryRegisterGlobalObserver<EventArgs>(nameof(this.EventSource2.TestEvent), OnTestEvent);
      this.EventManager.TryRegisterGlobalObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), OnGenericTestEvent);

      this.EventManager.TryRemoveGlobalObserver<EventArgs>(
        OnTestEvent);
      this.EventManager.TryRemoveGlobalObserver<TestEventArgs>(OnGenericTestEvent);

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(0, this.NonGenericEventInvocationCount);
      Assert.AreEqual(0, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void Handle2EventsAfterUnsubscribingFromAllUnspecificEventsOfKnownSource()
    {
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType(),  OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);

      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource2.TestEvent), this.EventSource2.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), this.EventSource2.GetType(), OnGenericTestEvent);

      this.EventManager.TryRemoveAllObservers(this.EventSource1.GetType());

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(1, this.NonGenericEventInvocationCount);
      Assert.AreEqual(1, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void Handle2EventsAfterUnsubscribingFromAllSpecificEventsOfAllUnknownSources()
    {
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);

      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource2.TestEvent), this.EventSource2.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), this.EventSource2.GetType(), OnGenericTestEvent);

      this.EventManager.TryRemoveAllObservers(nameof(this.EventSource1.TestEvent));

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(0, this.NonGenericEventInvocationCount);
      Assert.AreEqual(2, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void Handle3EventAfterUnsubscribingFromSpecificEventOfKnownSource()
    {
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);

      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource2.TestEvent), this.EventSource2.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), this.EventSource2.GetType(), OnGenericTestEvent);

      this.EventManager.TryRemoveAllObservers(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType());

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(1, this.NonGenericEventInvocationCount);
      Assert.AreEqual(2, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void Handle3EventsAfterUnregister1KnownSource1SpecificEvent()
    {
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);

      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource2.TestEvent), this.EventSource2.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), this.EventSource2.GetType(), OnGenericTestEvent);

      this.EventManager.TryRemoveObservable(this.EventSource1, new []{ nameof(this.EventSource1.TestEvent) });

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(1, this.NonGenericEventInvocationCount);
      Assert.AreEqual(2, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void Handle2EventsAfterUnregister1KnownCompleteSource()
    {
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);

      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource2.TestEvent), this.EventSource2.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), this.EventSource2.GetType(), OnGenericTestEvent);

      this.EventManager.TryRemoveObservable(this.EventSource1);

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(1, this.NonGenericEventInvocationCount);
      Assert.AreEqual(1, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void Handle2EventsAfterUnregister1KnownSourceAllEvents()
    {
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);

      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource2.TestEvent), this.EventSource2.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), this.EventSource2.GetType(), OnGenericTestEvent);

      this.EventManager.TryRemoveObservable(this.EventSource1, true);

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(1, this.NonGenericEventInvocationCount);
      Assert.AreEqual(1, this.GenericEventInvocationCount);
    }

    [TestMethod]
    public void Handle3EventsAfterUnregister1KnownSource1Event()
    {
      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource1.TestEvent), this.EventSource1.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource1.GenericTestEvent), this.EventSource1.GetType(), OnGenericTestEvent);

      this.EventManager.TryRegisterObserver<EventArgs>(nameof(this.EventSource2.TestEvent), this.EventSource2.GetType(), OnTestEvent);
      this.EventManager.TryRegisterObserver<TestEventArgs>(nameof(this.EventSource2.GenericTestEvent), this.EventSource2.GetType(), OnGenericTestEvent);

      this.EventManager.TryRemoveObservable(this.EventSource1, new []{ nameof(this.EventSource1.TestEvent)}, true);

      this.SenderType = this.EventSource1.GetType();
      this.EventSource1.RaiseAll();

      this.SenderType = this.EventSource2.GetType();
      this.EventSource2.RaiseAll();

      Assert.AreEqual(1, this.NonGenericEventInvocationCount);
      Assert.AreEqual(2, this.GenericEventInvocationCount);
    }

    private void OnTestEvent(object sender, EventArgs e)
    {
      this.NonGenericEventInvocationCount++;

      Assert.IsInstanceOfType(sender, this.SenderType, $"Sender is not {this.SenderType}");
    }

    private void OnGenericTestEvent(object sender, TestEventArgs e)
    {
      this.GenericEventInvocationCount++;

      Assert.IsInstanceOfType(sender, this.SenderType, $"Sender is not {this.SenderType}");
    }

    delegate void TestEventHandler(object sender, EventArgs e);
    public int NonGenericEventInvocationCount { get; set; }
    public int GenericEventInvocationCount { get; set; }
    public TestEventSource EventSource1 { get; set; }
    public TestEventSource2 EventSource2 { get; set; }
    public Type SenderType { get; set; }

    public IEventAggregator EventManager { get; set; }
  }
}
