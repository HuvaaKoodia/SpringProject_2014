using UnityEngine;
using System.Collections;

public class EnemyXmlData
{
    public string Type { get; private set; }

    public int Health { get; private set; }
    public int Damage { get; private set; }

    public EnemyXmlData(string type, int health, int damage)
    {
        this.Type = type;

        this.Health = health;
        this.Damage = damage;
    }
}
