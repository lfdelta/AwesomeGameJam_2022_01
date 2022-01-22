using System;
using System.IO;
using Treasure;

namespace Globals {
  public class GameManager
  {
    public GameManager(Treasure.Info[] foundTreasures)
    {
      AllTreasures = foundTreasures;
      
      CollectedTreasures = new bool[AllTreasures.Length];
      for (int i = 0; i < CollectedTreasures.Length; i++) {
        CollectedTreasures[i] = false;
      }

      CarriedTreasure = null;
    }

    public Treasure.Info[] AllTreasures { get; }
    public bool[] CollectedTreasures { get; set; }
    public Treasure.Info CarriedTreasure { get; set; }
    
  }
}
