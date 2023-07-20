// Decompiled with JetBrains decompiler
// Type: Webservices.Database.Items.Stream.StreamDocument
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Webservices.Database.Items.Stream
{
  [BsonKnownTypes(new Type[] {typeof (ContactRequestStream), typeof (GroupInvitationStream)})]
  public abstract class StreamDocument : MongoDocument
  {
    public int StreamId { get; set; }

    public abstract StreamType StreamType { get; }
  }
}
