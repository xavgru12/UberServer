// Decompiled with JetBrains decompiler
// Type: Webservices.PrivateMessageWebService
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
  internal class PrivateMessageWebService : IPrivateMessageWebServiceContract
  {
    public PrivateMessageWebService() => Console.Write("Initializing PrivateMessageWebService...\t");

    public byte[] GetAllMessageThreadsForUser(byte[] data) => new byte[0];

    public byte[] GetAllMessageThreadsForUser_2(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        ListProxy<MessageThreadView>.Serialize((Stream) bytes, (ICollection<MessageThreadView>) new List<MessageThreadView>(), new ListProxy<MessageThreadView>.Serializer<MessageThreadView>(MessageThreadViewProxy.Serialize));
        return bytes.ToArray();
      }
    }

    public byte[] GetThreadMessages(byte[] data) => new byte[0];

    public byte[] SendMessage(byte[] data) => new byte[0];

    public byte[] GetMessageWithId(byte[] data) => new byte[0];

    public byte[] MarkThreadAsRead(byte[] data) => new byte[0];

    public byte[] DeleteThread(byte[] data) => new byte[0];
  }
}
