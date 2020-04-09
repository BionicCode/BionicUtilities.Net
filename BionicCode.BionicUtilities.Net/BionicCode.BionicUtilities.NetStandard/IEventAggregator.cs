using System;
using System.Collections.Generic;

namespace BionicCode.BionicUtilities.NetStandard
{
  public interface IEventAggregator
  {
    bool TryRegisterObservable<TEventSource>(TEventSource eventSource, IEnumerable<string> eventNames);
    bool TryRemoveObservable<TEventSource>(
      TEventSource eventSource,
      IEnumerable<string> eventNames,
      bool removeEventObservers = false);
    bool TryRemoveObservable<TEventSource>(TEventSource eventSource, bool removeObserversOfEvents = false);

    bool TryRemoveSpecificEventSource<TEventHandler>(string eventName, Type eventSourceType, TEventHandler eventHandler)
      where TEventHandler : Delegate;

    bool TryRemoveAllHandlers<TEventHandler>(string eventName);

    bool TryRemoveAllSpecificEventSources(string eventName, Type eventSourceType);
    bool TryClearEventSources(Type eventSourceType);
    bool TryRemoveHandlerFromGlobalEventSource<TEventHandler>(string eventName, TEventHandler eventHandler) where TEventHandler : Delegate;
    bool TryClearGlobalEventSource<TEventHandler>(TEventHandler eventHandler) where TEventHandler : Delegate;

    bool TryRegisterObserver<TEventHandler>(string eventName, Type eventSourceType, TEventHandler eventHandler)
     where TEventHandler : Delegate;

    bool TryRegisterObserver<TEventHandler>(string eventName, TEventHandler eventHandler)
      where TEventHandler : Delegate;
  }
}