#region Info
// //  
// BionicUtilities.NetStandard
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BionicUtilities.NetStandard
{
  public class EventAggregator : IEventAggregator
  {
    public EventAggregator()
    {
      this.EventHandlerTable = new ConcurrentDictionary<Type, ConcurrentDictionary<string, List<Delegate>>>();
      this.EventPublisherTable = new ConcurrentDictionary<string, Delegate>();
    }

    #region Implementation of IEventAggregator<TEventSource>

    /// <inheritdoc />
    public bool TryRegisterObservable<TEventSource>(TEventSource eventSource, IEnumerable<string> eventNames)
    {
      if (eventSource == null)
      {
        return false;
      }
      foreach (string eventName in eventNames)
      {
        EventInfo eventInfo = eventSource.GetType()
          .GetEvent(
            eventName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if (eventInfo == null)
        {
          throw new ArgumentException($"The event {eventName} was not found on the event source {eventSource.GetType().Name} or on its declaring base type.");
        }

        var fullyQualifiedEventIdOfGlobalSource =
          CreateFullyQualifiedEventIdOfGlobalSource(eventInfo.EventHandlerType, eventName);
        var fullyQualifiedEventIdOfSpecificSource =
          CreateFullyQualifiedEventIdOfSpecificSource(eventSource.GetType(), eventName);

        IEnumerable<string> interfaceEventIds = eventSource.GetType().GetInterfaces().Select(
          interfaceType => CreateFullyQualifiedEventIdOfSpecificSource(interfaceType, eventName));
        var eventIds = new List<string>(interfaceEventIds) { fullyQualifiedEventIdOfSpecificSource, fullyQualifiedEventIdOfGlobalSource };

        (Type EventHandlerType, IEnumerable<string> EventIds) eventIdArg = (eventInfo.EventHandlerType, eventIds);

        var eventPublisher = new Action<object, object>((sender, args) => HandleEvent(eventIdArg, sender, args));
        if (!this.EventPublisherTable.TryAdd(fullyQualifiedEventIdOfSpecificSource, eventPublisher))
        {
          return false;
        }
        Delegate genericHandler = Delegate.CreateDelegate(
          eventInfo.EventHandlerType,
          eventPublisher.Target,
          eventPublisher.Method);

        eventInfo.AddEventHandler(eventSource, genericHandler);
      }

      return true;
    }

    /// <inheritdoc />
    public bool TryRemoveObservable<TEventSource>(TEventSource eventSource, IEnumerable<string> eventNames, bool removeEventObservers = false)
    {
      bool result = false;
      foreach (string eventName in eventNames)
      {
        string fullyQualifiedEventIdOfSpecificSource =
          CreateFullyQualifiedEventIdOfSpecificSource(eventSource.GetType(), eventName);
        bool isRemoveSuccessful = this.EventPublisherTable.TryRemove(fullyQualifiedEventIdOfSpecificSource, out Delegate handler);
        result |= isRemoveSuccessful;

        if (isRemoveSuccessful)
        {
          EventInfo eventInfo = eventSource.GetType()
            .GetEvent(
              eventName,
              BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
          eventInfo?.RemoveEventHandler(eventSource, handler);
        }

        if (removeEventObservers)
        {
          TryRemoveAllSpecificEventSources(eventName, eventSource.GetType());
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryRemoveObservable<TEventSource>(TEventSource eventSource, bool removeObserversOfEvents = false)
    {
      bool result = false;
      string fullyQualifiedEventIdOfSpecificSourcePrefix =
        CreateFullyQualifiedEventIdOfSpecificSource(eventSource.GetType(), string.Empty);
      foreach (KeyValuePair<string, Delegate> publisherEntry in this.EventPublisherTable.Where(entry => entry.Key.StartsWith(fullyQualifiedEventIdOfSpecificSourcePrefix)))
      {
        bool isRemoveSuccessful = this.EventPublisherTable.TryRemove(publisherEntry.Key, out Delegate handler);
        result |= isRemoveSuccessful;
        if (!isRemoveSuccessful || !removeObserversOfEvents)
        {
          continue;
        }

        TryRemoveAllSpecificEventSources(publisherEntry.Key, eventSource.GetType());
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryRemoveSpecificEventSource<TEventHandler>(string eventName, Type eventSourceType, TEventHandler eventHandler) where TEventHandler : Delegate
    {
      bool result = false;
      string fullyQualifiedEventIdOfSpecificSource =
        CreateFullyQualifiedEventIdOfSpecificSource(eventSourceType, eventName);
      for (var index = 0; index < this.EventHandlerTable.Count; index++)
      {
        KeyValuePair<Type, ConcurrentDictionary<string, List<Delegate>>> entry = this.EventHandlerTable.ElementAt(index);
        bool hasRemovedEntry = entry.Value.TryGetValue(fullyQualifiedEventIdOfSpecificSource, out List<Delegate> handlers) &&
            handlers.Remove(eventHandler);
        result |= hasRemovedEntry;
        if (hasRemovedEntry && entry.Value.IsEmpty)
        {
          this.EventHandlerTable.TryRemove(entry.Key, out ConcurrentDictionary<string, List<Delegate>> eventNameToHandlersMap);
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryRemoveAllSpecificEventSources(string eventName, Type eventSourceType)
    {
      bool result = false;
      string fullyQualifiedEventIdOfSpecificSource =
        CreateFullyQualifiedEventIdOfSpecificSource(eventSourceType, eventName);
      for (var index = 0; index < this.EventHandlerTable.Count; index++)
      {
        KeyValuePair<Type, ConcurrentDictionary<string, List<Delegate>>> entry = this.EventHandlerTable.ElementAt(index);
        bool hasRemovedEntry = entry.Value.TryRemove(fullyQualifiedEventIdOfSpecificSource,
          out List<Delegate> handlers);
        result |= hasRemovedEntry;
        if (hasRemovedEntry && entry.Value.IsEmpty)
        {
          this.EventHandlerTable.TryRemove(entry.Key, out ConcurrentDictionary<string, List<Delegate>> eventNameToHandlersMap);
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryClearEventSources(Type eventSourceType)
    {
      bool result = false;
      string fullyQualifiedEventIdOfSpecificSourcePrefix =
        CreateFullyQualifiedEventIdOfSpecificSource(eventSourceType, string.Empty);
      for (var index = 0; index < this.EventHandlerTable.Count; index++)
      {
        KeyValuePair<Type, ConcurrentDictionary<string, List<Delegate>>> entry = this.EventHandlerTable.ElementAt(index);
        for (var index2 = 0; index2 < entry.Value.Count; index2++)
        {
          KeyValuePair<string, List<Delegate>> handlersEntry = entry.Value.ElementAt(index2);
          if (handlersEntry.Key.StartsWith(fullyQualifiedEventIdOfSpecificSourcePrefix))
          {
            result |= entry.Value.TryRemove(handlersEntry.Key, out List<Delegate> handlers);
          }
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryRemoveHandlerFromGlobalEventSource<TEventHandler>(string eventName, TEventHandler eventHandler) where TEventHandler : Delegate
    {
      bool result = false;
      string fullyQualifiedEventIdOfGlobalSource =
        CreateFullyQualifiedEventIdOfGlobalSource(typeof(TEventHandler), eventName);
      for (var index = 0; index < this.EventHandlerTable.Count; index++)
      {
        KeyValuePair<Type, ConcurrentDictionary<string, List<Delegate>>> entry = this.EventHandlerTable.ElementAt(index);
        bool hasRemovedEntry = entry.Value.TryGetValue(fullyQualifiedEventIdOfGlobalSource, out List<Delegate> handlers) && handlers.Remove(eventHandler);
        result |= hasRemovedEntry;
        if (hasRemovedEntry && entry.Value.IsEmpty)
        {
          this.EventHandlerTable.TryRemove(entry.Key, out ConcurrentDictionary<string, List<Delegate>> eventNameToHandlersMap);
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryClearGlobalEventSource<TEventHandler>(TEventHandler eventHandler) where TEventHandler : Delegate
    {
      bool result = false;
      string fullyQualifiedEventIdOfSpecificSourcePrefix =
        CreateFullyQualifiedEventIdOfGlobalSource(typeof(TEventHandler), string.Empty);
      for (var index = 0; index < this.EventHandlerTable.Count; index++)
      {
        KeyValuePair<Type, ConcurrentDictionary<string, List<Delegate>>> entry = this.EventHandlerTable.ElementAt(index);
        for (var index2 = 0; index2 < entry.Value.Count; index2++)
        {
          KeyValuePair<string, List<Delegate>> handlersEntry = entry.Value.ElementAt(index2);
          if (handlersEntry.Key.StartsWith(fullyQualifiedEventIdOfSpecificSourcePrefix))
          {
            result |= entry.Value.TryGetValue(handlersEntry.Key, out List<Delegate> handlers) && handlers.Remove(eventHandler);
          }
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryRemoveAllHandlers<TEventHandler>(string eventName)
    {
      bool result = false;
      string fullyQualifiedEventIdSuffix = eventName;
      for (var index = 0; index < this.EventHandlerTable.Count; index++)
      {
        KeyValuePair<Type, ConcurrentDictionary<string, List<Delegate>>> entry = this.EventHandlerTable.ElementAt(index);
        for (var index2 = 0; index2 < entry.Value.Count; index2++)
        {
          KeyValuePair<string, List<Delegate>> handlersEntry = entry.Value.ElementAt(index2);
          if (handlersEntry.Key.EndsWith(fullyQualifiedEventIdSuffix))
          {
            result |= entry.Value.TryRemove(handlersEntry.Key, out List<Delegate> handlers);
          }
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryRegisterObserver<TEventHandler>(string eventName, TEventHandler eventHandler)
      where TEventHandler : Delegate
    {
      var fullyQualifiedEventName = CreateFullyQualifiedEventIdOfGlobalSource(typeof(TEventHandler), eventName);
      RegisterObserver(eventHandler, fullyQualifiedEventName);

      return true;
    }

    /// <inheritdoc />
    public bool TryRegisterObserver<TEventHandler>(string eventName, Type eventSourceType, TEventHandler eventHandler)
      where TEventHandler : Delegate
    {
      var fullyQualifiedEventName = CreateFullyQualifiedEventIdOfSpecificSource(eventSourceType, eventName);
      RegisterObserver(eventHandler, fullyQualifiedEventName);

      return true;
    }

    #endregion

    private void RegisterObserver<TEventHandler>(
      TEventHandler eventHandler,
      string fullyQualifiedEventName) where TEventHandler : Delegate
    {
      Type eventHandlerType = typeof(TEventHandler);
      if (this.EventHandlerTable.TryGetValue(
        eventHandlerType,
        out ConcurrentDictionary<string, List<Delegate>> eventNameToHandlersMap))
      {
        if (eventNameToHandlersMap.TryGetValue(fullyQualifiedEventName, out List<Delegate> handlers))
        {
          handlers.Add(eventHandler);
        }
        else
        {
          eventNameToHandlersMap.TryAdd(fullyQualifiedEventName, new List<Delegate>() { eventHandler });
        }
      }
      else
      {
        var nameToHandlersMap = new ConcurrentDictionary<string, List<Delegate>>();
        nameToHandlersMap.TryAdd(fullyQualifiedEventName, new List<Delegate>() { eventHandler });
        this.EventHandlerTable.TryAdd(eventHandlerType, nameToHandlersMap);
      }
    }

    private string CreateFullyQualifiedEventIdOfGlobalSource(Type eventHandlerType, string eventName) => eventHandlerType.FullName.ToLowerInvariant() + "." + eventName;

    private string CreateFullyQualifiedEventIdOfSpecificSource(Type eventSource, string eventName) => eventSource.AssemblyQualifiedName.ToLowerInvariant() + "." + eventName;

    private void HandleEvent((Type EventHandlerType, IEnumerable<string> EventIds) eventInfo, object sender, object args)
    {
      if (!this.EventHandlerTable.TryGetValue(
        eventInfo.EventHandlerType,
        out ConcurrentDictionary<string, List<Delegate>> handlerInfos))
      {
        return;
      }

      IEnumerable<Delegate> handlers = eventInfo.EventIds
        .SelectMany(
          eventId => handlerInfos.TryGetValue(eventId, out List<Delegate> delegates)
            ? delegates
            : new List<Delegate>());

      foreach (Delegate handler in handlers)
      {
        Type handlerEventArgsType = handler.GetType()
          .GetMethod("Invoke")?
          .GetParameters()
          .ElementAt(1).ParameterType;
        if (!args.GetType().IsSubclassOf(handlerEventArgsType)
            && handlerEventArgsType != args.GetType())
        {
          throw new InvalidOperationException(
            $"The event handler {handler.Method.Name} has the wrong signature. Expected event args type: {args.GetType()}. But found type {handlerEventArgsType}.");
        }

        handler.DynamicInvoke(sender, args);
      }
    }

    private ConcurrentDictionary<Type, ConcurrentDictionary<string, List<Delegate>>> EventHandlerTable { get; set; }
    private ConcurrentDictionary<string, Delegate> EventPublisherTable { get; }
  }
}