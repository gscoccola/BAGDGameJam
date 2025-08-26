using System;
using System.Collections.Generic;

public class Pool<T>
{
    private readonly List<T> _currentStock;
    private readonly Func<T> _createFunction;
    private readonly Action<T> _enableFunction;
    private readonly Action<T> _disableFunction;
    private readonly bool _canCreateMoreObjects;

    
    #region Pool
    public Pool(Func<T> createFunction, Action<T> enableFunction, Action<T> disableFunction, int initialStock, bool canCreateMoreObjects = true)
    {
        _createFunction = createFunction;
        _enableFunction = enableFunction;
        _disableFunction = disableFunction;
        _canCreateMoreObjects = canCreateMoreObjects;
        
        _currentStock = new List<T>();
        for (var i = 0; i < initialStock; i++)
        {
            T obj = _createFunction();
            _disableFunction(obj);
            _currentStock.Add(obj);
        }
    }
    
    public T GetObject()
    {
        T obj = default(T);

        if (_currentStock.Count > 0)
        {
            obj = _currentStock[0];
            _currentStock.Remove(obj);
        }
        else if(_canCreateMoreObjects) obj = _createFunction();
        
        _enableFunction(obj);
        return obj;
    }
    
    public void ReturnObject(T obj)
    {
        _disableFunction(obj);
        _currentStock.Add(obj);
    }
    
    public void RemoveObject(T obj)
    {
        if (_currentStock.Contains(obj)) _currentStock.Remove(obj);
    }
    #endregion
}