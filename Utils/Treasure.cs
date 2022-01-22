using System;
using System.IO;
using Globals;

namespace Treasure 
{
  public class Info
  {
    public Info(int value)
    {
      this.Value = value;
    }

    public int Value { get; }

    public void Pickup(Globals.GameManager gameManager)
    {
      if (gameManager.CarriedTresure == null) 
      {
        gameManager.CarriedTreasure = this;
      }
    }
    public void Dropoff(Globals.GameManager gameManager) 
    {
      gameManager.CarriedTreasure = null;
      int treasureIndex = AllTreasures.IndexOf(this);
      gameManager.CollectedTreasures[treasureIndex] = true;
    }
  }
}

