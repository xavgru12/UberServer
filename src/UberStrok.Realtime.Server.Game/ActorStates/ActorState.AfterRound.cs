using System;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Realtime.Server.Game;

public sealed class PlayerAfterRoundState : ActorState
{
    public PlayerAfterRoundState(GameActor actor)
        : base(actor)
    {
    }
}
