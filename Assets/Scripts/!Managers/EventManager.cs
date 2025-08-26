using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    public delegate void Callback();
    public delegate void CallbackWithParameters(params object[] parameters);
    private readonly Dictionary<string, Callback> _subscribers = new();
    private readonly Dictionary<string, CallbackWithParameters> _subscribersWithParameters = new();
    
    
    #region EventManager
    public void Subscribe(string eventId, Callback callback)
    {
        if (!_subscribers.ContainsKey(eventId)) _subscribers.Add(eventId, callback);
        else _subscribers[eventId] += callback;
    }
    
    public void Subscribe(string eventId, CallbackWithParameters callbackWithParameters)
    {
        if (!_subscribersWithParameters.ContainsKey(eventId)) _subscribersWithParameters.Add(eventId, callbackWithParameters);
        else _subscribersWithParameters[eventId] += callbackWithParameters;
    }

    public void Unsubscribe(string eventId, Callback callback)
    {
        if (_subscribers.ContainsKey(eventId)) _subscribers[eventId] -= callback;
    }
    
    public void Unsubscribe(string eventId, CallbackWithParameters callbackWithParameters)
    {
        if (_subscribersWithParameters.ContainsKey(eventId)) _subscribersWithParameters[eventId] -= callbackWithParameters;
    }

    public void Trigger(string eventId, params object[] parameters)
    {
        if (_subscribers.ContainsKey(eventId)) _subscribers[eventId]?.Invoke();
        if (_subscribersWithParameters.ContainsKey(eventId)) _subscribersWithParameters[eventId]?.Invoke(parameters);
    }
    #endregion
}