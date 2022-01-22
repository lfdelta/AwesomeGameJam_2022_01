using System;
using System.IO;

public class Player
{
  public Player(string name)
  {
    Name = name;
  }

  public string Name { get; }

  public override string ToString()
  {
    return $"The player's name is {this.Name}";
  }
}