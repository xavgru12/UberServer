// Decompiled with JetBrains decompiler
// Type: Webservices.RelationshipWebService
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using UberStrike.Core.Serialization;

namespace Webservices
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
  public class RelationshipWebService : IRelationshipWebServiceContract
  {
    public RelationshipWebService() => Console.Write("Initializing RelationshipWebService...\t\t");

    public byte[] SendContactRequest(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetContactRequests(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        ListProxy<ContactRequestView>.Serialize((Stream) bytes, (ICollection<ContactRequestView>) new List<ContactRequestView>(), new ListProxy<ContactRequestView>.Serializer<ContactRequestView>(ContactRequestViewProxy.Serialize));
        return bytes.ToArray();
      }
    }

    public byte[] AcceptContactRequest(byte[] data) => new MemoryStream().ToArray();

    public byte[] DeclineContactRequest(byte[] data) => new MemoryStream().ToArray();

    public byte[] DeleteContact(byte[] data) => new MemoryStream().ToArray();

    public byte[] GetContactsByGroups(byte[] data) => new MemoryStream().ToArray();

    public byte[] MoveContactToGroup(byte[] data) => new byte[0];
  }
}
