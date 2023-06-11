﻿using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
  [Serializable]
  public class MapView
  {
    public int MapId { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public string SceneName { get; set; }

    public string FileName { get; set; }

    public bool IsBlueBox { get; set; }

    public int RecommendedItemId { get; set; }

    public int SupportedGameModes { get; set; }

    public int MaxPlayers { get; set; }

    public Dictionary<GameModeType, MapSettings> Settings { get; set; }
  }
}
