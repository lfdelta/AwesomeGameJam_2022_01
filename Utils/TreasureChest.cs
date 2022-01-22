using System;
using System.IO;
using Globals;
using Treasure;

namespace TreasureChest
{
  public class Methods
  {
    public void DetermineDropoff(GameManager.Info gameManager)
    {
      if (gameManager.CarriedTreasure != null) {
        gameManager.CarriedTreasure.Dropoff(gameManager);
      }
    }
  }
}