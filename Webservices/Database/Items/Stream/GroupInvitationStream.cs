// Decompiled with JetBrains decompiler
// Type: Webservices.Database.Items.Stream.GroupInvitationStream
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;

namespace Webservices.Database.Items.Stream
{
  public class GroupInvitationStream : StreamDocument
  {
    public GroupInvitationView GroupInvitation { get; set; }

    public override StreamType StreamType => StreamType.GroupInvitation;
  }
}
