using DalApi;
using DO;
using System;
using System.Collections.Generic;
namespace Dal;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Call? Read(Func<Call, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Call item)
    {
        throw new NotImplementedException();
    }
}
