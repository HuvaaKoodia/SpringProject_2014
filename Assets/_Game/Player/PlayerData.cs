using UnityEngine;
using System.Collections;

public class PlayerData
{
    public int Health { get; private set; }

    public float MovementSpeed { get; private set; }
    public float TurnSpeed { get; private set; }

    public PlayerData(int health, float movementSpeed, float turnSpeed)
    {
        this.Health = health;
        this.MovementSpeed = movementSpeed;
        this.TurnSpeed = turnSpeed;
    }
}
