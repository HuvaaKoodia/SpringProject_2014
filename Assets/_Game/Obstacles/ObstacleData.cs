using UnityEngine;
using System.Collections;

public class ObstacleData
{
    public string Type { get; private set; }
    
    public int Health { get; private set; }

    public ObstacleData(string type, int health)
    {
        this.Type = type;

        this.Health = health;
    }
}
