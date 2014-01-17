using UnityEngine;
using System.Collections;

public class WeaponXmlData
{
    public string Type { get; private set; }
    public string Name { get; private set; }

    public int Damage { get; private set; }
    public int Accuracy { get; private set; }
    public int Heat { get; private set; }

    public WeaponXmlData(string type, string name, int damage, int accuracy, int heat)
    {
        this.Type = type;
        this.Name = name;

        this.Damage = damage;
        this.Accuracy = accuracy;
        this.Heat = heat;
    }
}
