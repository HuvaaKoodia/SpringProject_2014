using UnityEngine;
using System.Collections;

public class ObstacleXmlData
{
    public string Type { get; private set; }
    
    public int Health { get; private set; }

    public ObstacleXmlData(string type, int health)
    {
        this.Type = type;

        this.Health = health;
    }
}
