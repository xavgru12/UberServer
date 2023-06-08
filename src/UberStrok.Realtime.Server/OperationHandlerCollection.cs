using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UberStrok.Realtime.Server;

public class OperationHandlerCollection : IEnumerable<OperationHandler>, IEnumerable
{
    private readonly ConcurrentDictionary<byte, OperationHandler> _opHandlers;

    public OperationHandler this[byte id]
    {
        get
        {
            _opHandlers.TryGetValue(id, out var value);
            return value;
        }
    }

    public int Count => _opHandlers.Count;

    public OperationHandlerCollection()
    {
        _opHandlers = new ConcurrentDictionary<byte, OperationHandler>();
    }

    public void Add(OperationHandler handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException("handler");
        }
        if (!_opHandlers.TryAdd(handler.Id, handler))
        {
            throw new InvalidOperationException("Already contains an OperationHandler with the same ID");
        }
    }

    public bool Remove(byte id)
    {
        OperationHandler value;
        return _opHandlers.TryRemove(id, out value);
    }

    public IEnumerator<OperationHandler> GetEnumerator()
    {
        return _opHandlers.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _opHandlers.Values.GetEnumerator();
    }
}