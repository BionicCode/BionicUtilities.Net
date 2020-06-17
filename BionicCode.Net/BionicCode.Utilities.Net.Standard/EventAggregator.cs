#region Info
// //  
// BionicUtilities.Net.Standard
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BionicCode.Utilities.Net.Standard
{
  public class EventAggregator : IEventAggregator
  {
    public EventAggregator()
    {
      this.EventHandlerTable = new ConcurrentDictionary<string, List<Delegate>>();
      this.EventPublisherTable = new ConcurrentDictionary<object, List<(EventInfo EventInfo, Delegate Handler)>>();
    }

    #region Implementation of IEventAggregator


    /// <inheritdoc />
    public bool TryRegisterObservable<TEventSource>(TEventSource eventSource, IEnumerable<string> eventNames)
    {
      if (EqualityComparer<TEventSource>.Default.Equals(eventSource, default))
      {
        return false;
      }
      foreach (string eventName in eventNames.Distinct())
      {
        EventInfo eventInfo = eventSource.GetType()
          .GetEvent(
            eventName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if (eventInfo == null)
        {
          throw new ArgumentException($"The event {eventName} was not found on the event source {eventSource.GetType().Name} or on its declaring base type.");
        }

        Type normalizedEventHandlerType = NormalizeEventHandlerType(eventInfo.EventHandlerType);

        var fullyQualifiedEventIdOfGlobalEvent =
          CreateFullyQualifiedEventIdOfGlobalSource(normalizedEventHandlerType, eventName);
        var fullyQualifiedEventIdOfUnknownGlobalEvent =
          CreateFullyQualifiedEventIdOfGlobalSource(normalizedEventHandlerType, string.Empty);
        var fullyQualifiedEventIdOfSpecificEvent =
          CreateFullyQualifiedEventIdOfSpecificSource(eventSource.GetType(), eventName);

        IEnumerable<string> interfaceEventIds = eventSource.GetType().GetInterfaces().Select(
          interfaceType => CreateFullyQualifiedEventIdOfSpecificSource(interfaceType, eventName));

        var eventIds = new List<string>(interfaceEventIds) { fullyQualifiedEventIdOfSpecificEvent, fullyQualifiedEventIdOfGlobalEvent, fullyQualifiedEventIdOfUnknownGlobalEvent };

        (Type eventHandlerType, List<string> eventIds) eventIdArg = (normalizedEventHandlerType, eventIds);
        //DynamicMethod handler =
        //  new DynamicMethod("",
        //    null,
        //    eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(pi => pi.ParameterType).ToArray(),
        //    GetType());

        //ILGenerator ilgen = handler.GetILGenerator();

        //Type[] showParameters = { typeof(String) };
        //MethodInfo simpleShow =
        //  typeof(MessageBox).GetMethod("Show", showParameters);

        //ilgen.Emit(OpCodes.Ldstr,
        //  "This event handler was constructed at run time.");
        //ilgen.Emit(OpCodes.Call, simpleShow);
        //ilgen.Emit(OpCodes.Pop);
        //ilgen.Emit(OpCodes.Ret);

        Action<object, object> clientHandlerInvocator = (sender, args) => HandleEvent(eventIdArg, sender,args);
        var eventSourceHandler = Delegate.CreateDelegate(
          eventInfo.EventHandlerType,
          clientHandlerInvocator.Target,
          clientHandlerInvocator.Method);
        if (this.EventPublisherTable.TryGetValue(eventSource, out List<(EventInfo, Delegate)> publishers))
        {
          publishers.Add((eventInfo, eventSourceHandler));
        }
        else if (!this.EventPublisherTable.TryAdd(eventSource, new List<(EventInfo, Delegate)> { (eventInfo, eventSourceHandler) }))
        {
          return false;
        }

        eventInfo.AddEventHandler(eventSource, eventSourceHandler);
      }

      return true;
    }

    /// <inheritdoc />
    public bool TryRemoveObservable(object eventSource, IEnumerable<string> eventNames, bool removeEventObservers = false)
    {
      bool hasRemovedObservable = false;
      if (!this.EventPublisherTable.TryGetValue(eventSource, out List<(EventInfo EventInfo, Delegate Handler)> publisherHandlerInfos))
      {
        return false;
      }
      foreach (string eventName in eventNames)
      {
        (EventInfo EventInfo, Delegate Handler) publisherHandlerInfo = publisherHandlerInfos.FirstOrDefault(
          handlerInfo => handlerInfo.EventInfo.Name.Equals(eventName, StringComparison.Ordinal));

        publisherHandlerInfo.EventInfo?.RemoveEventHandler(eventSource, publisherHandlerInfo.Handler);
        hasRemovedObservable = publisherHandlerInfos.Remove(publisherHandlerInfo); 

        if (removeEventObservers)
        {
          TryRemoveAllObservers(eventName, eventSource.GetType());
        }
      }

      if (!publisherHandlerInfos.Any())
      {
        this.EventPublisherTable.TryRemove(eventSource, out List<(EventInfo EventInfo, Delegate Handler)> _);
      }

      return hasRemovedObservable;
    }

    /// <inheritdoc />
    public bool TryRemoveObservable(object eventSource, bool removeObserversOfEvents = false)
    {
      bool hasRemovedObservable = false;

      if (this.EventPublisherTable.TryRemove(eventSource, out List<(EventInfo EventInfo, Delegate Handler)> handlerInfo))
      {
        handlerInfo.ForEach(publisherHandlerInfo => publisherHandlerInfo.EventInfo.RemoveEventHandler(eventSource, publisherHandlerInfo.Handler));
        hasRemovedObservable = true;
      }

      if (removeObserversOfEvents)
      {
        TryRemoveAllObservers(eventSource.GetType());
      }

      return hasRemovedObservable;
    }

    /// <inheritdoc />
    public bool TryRegisterObserver<TEventArgs>(string eventName, Type eventSourceType, EventHandler<TEventArgs> eventHandler)
    {
      var fullyQualifiedEventName = CreateFullyQualifiedEventIdOfSpecificSource(eventSourceType, eventName);
      return TryRegisterObserverInternal(eventHandler, fullyQualifiedEventName);
    }

    /// <inheritdoc />
    public bool TryRegisterGlobalObserver<TEventArgs>(string eventName, EventHandler<TEventArgs> eventHandler)
    {
      Type normalizedEventHandlerType = NormalizeEventHandlerType(eventHandler.GetType());
      var fullyQualifiedEventName = CreateFullyQualifiedEventIdOfGlobalSource(normalizedEventHandlerType, eventName);
      return TryRegisterObserverInternal(eventHandler, fullyQualifiedEventName);
    }

    /// <inheritdoc />
    public bool TryRegisterGlobalObserver<TEventArgs>(EventHandler<TEventArgs> eventHandler)
    {
      Type normalizedEventHandlerType = NormalizeEventHandlerType(eventHandler.GetType());
      var fullyQualifiedEventName = CreateFullyQualifiedEventIdOfGlobalSource(normalizedEventHandlerType, string.Empty);
      return TryRegisterObserverInternal(eventHandler, fullyQualifiedEventName);
    }

    /// <inheritdoc />
    public bool TryRemoveObserver<TEventArgs>(string eventName, Type eventSourceType, EventHandler<TEventArgs> eventHandler)
    {
      string fullyQualifiedEventIdOfSpecificSource =
        CreateFullyQualifiedEventIdOfSpecificSource(eventSourceType, eventName);
      return this.EventHandlerTable.TryRemove(fullyQualifiedEventIdOfSpecificSource, out List<Delegate> _);
    }

    /// <inheritdoc />
    public bool TryRemoveGlobalObserver<TEventArgs>(string eventName, EventHandler<TEventArgs> eventHandler)
    {
      Type normalizedEventHandlerType = NormalizeEventHandlerType(eventHandler.GetType());

      string fullyQualifiedEventIdOfGlobalSource =
        CreateFullyQualifiedEventIdOfGlobalSource(normalizedEventHandlerType, eventName);
      return this.EventHandlerTable.TryRemove(fullyQualifiedEventIdOfGlobalSource, out List<Delegate> _);
    }

    /// <inheritdoc />
    public bool TryRemoveGlobalObserver<TEventArgs>(EventHandler<TEventArgs> eventHandler)
    {
      bool result = false;
      Type normalizedEventHandlerType = NormalizeEventHandlerType(eventHandler.GetType());

      string fullyQualifiedEventIdOfGlobalSourcePrefix =
        CreateFullyQualifiedEventIdOfGlobalSource(normalizedEventHandlerType, string.Empty);
      for (var index = this.EventHandlerTable.Count - 1; index >= 0; index--)
      {
        KeyValuePair<string, List<Delegate>> handlersEntry = this.EventHandlerTable.ElementAt(index);
        if (handlersEntry.Key.StartsWith(fullyQualifiedEventIdOfGlobalSourcePrefix, StringComparison.Ordinal))
        {
          result |= this.EventHandlerTable.TryRemove(handlersEntry.Key, out List<Delegate> _);
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryRemoveAllObservers(string eventName, Type eventSourceType)
    {
      string fullyQualifiedEventIdOfSpecificSource =
        CreateFullyQualifiedEventIdOfSpecificSource(eventSourceType, eventName);
      return this.EventHandlerTable.TryRemove(fullyQualifiedEventIdOfSpecificSource,
        out List<Delegate> _);
    }

    /// <inheritdoc />
    public bool TryRemoveAllObservers(Type eventSourceType)
    {
      bool result = false;
      string fullyQualifiedEventIdOfSpecificSourcePrefix =
        CreateFullyQualifiedEventIdOfSpecificSource(eventSourceType, string.Empty);
      for (var index = this.EventHandlerTable.Count - 1; index >= 0; index--)
      {
        KeyValuePair<string, List<Delegate>> handlersEntry = this.EventHandlerTable.ElementAt(index);
        if (handlersEntry.Key.StartsWith(fullyQualifiedEventIdOfSpecificSourcePrefix, StringComparison.Ordinal))
        {
          result |= this.EventHandlerTable.TryRemove(handlersEntry.Key, out _);
        }
      }

      return result;
    }

    /// <inheritdoc />
    public bool TryRemoveAllObservers(string eventName)
    {
      bool result = false;
      string fullyQualifiedEventIdSuffix = $".{eventName}";
      for (var index = this.EventHandlerTable.Count - 1; index >= 0; index--)
      {
        KeyValuePair<string, List<Delegate>> handlersEntry = this.EventHandlerTable.ElementAt(index);
        if (handlersEntry.Key.EndsWith(fullyQualifiedEventIdSuffix, StringComparison.Ordinal))
        {
          result |= this.EventHandlerTable.TryRemove(handlersEntry.Key, out List<Delegate> _);
        }
      }

      return result;
    }

    #endregion Implementation of IEventAggregator

    private bool TryRegisterObserverInternal<TEventArgs>(
      EventHandler<TEventArgs> eventHandler,
      string fullyQualifiedEventName)
    {
      if (this.EventHandlerTable.TryGetValue(fullyQualifiedEventName, out List<Delegate> handlers))
      {
        handlers.Add(eventHandler);
        return true;
      }

      return this.EventHandlerTable.TryAdd(fullyQualifiedEventName, new List<Delegate>() { eventHandler });
    }

    private void HandleEvent((Type EventHandlerType, IEnumerable<string> EventIds) eventInfo, object sender, object args)
    {
      IEnumerable<Delegate> handlers = eventInfo.EventIds
        .SelectMany(
          eventId => this.EventHandlerTable.TryGetValue(eventId, out List<Delegate> delegates)
            ? delegates
            : new List<Delegate>());

      foreach (Delegate handler in handlers)
      {
        Type handlerEventArgsType = handler.GetType()
          .GetMethod("Invoke")?
          .GetParameters()
          .ElementAt(1).ParameterType;
        if (handlerEventArgsType == null || !args.GetType().IsSubclassOf(handlerEventArgsType)
            && handlerEventArgsType != args.GetType())
        {
          throw new InvalidOperationException(
            $"The event handler {handler.Method.Name} has the wrong signature. Expected event args type: {args.GetType()}. But found type {handlerEventArgsType}.");
        }

        handler.DynamicInvoke(sender, args);
      }
    }

    private Type NormalizeEventHandlerType(Type eventHandlerType)
    {
      Type normalizedEventHandlerType = eventHandlerType == typeof(EventHandler)
        ? typeof(EventHandler<EventArgs>)
        : eventHandlerType;
      return normalizedEventHandlerType;
    }

    private string CreateFullyQualifiedEventIdOfGlobalSource(Type eventHandlerType, string eventName) => eventHandlerType.FullName.ToLowerInvariant() + "." + eventName;

    private string CreateFullyQualifiedEventIdOfSpecificSource(Type eventSource, string eventName) => eventSource.AssemblyQualifiedName.ToLowerInvariant() + "." + eventSource.FullName.ToLowerInvariant() + "." + eventName;

    private ConcurrentDictionary<string, List<Delegate>> EventHandlerTable { get; set; }
    private ConcurrentDictionary<object, List<(EventInfo EventInfo,  Delegate Handler)>> EventPublisherTable { get; }
  }
}