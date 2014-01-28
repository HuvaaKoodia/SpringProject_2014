using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class XMLDataLoader : XML_Loader
{
    public static void Read(GameDatabase database)
    {
		var DOX=XML_Loader.GetAllXmlDocuments("Data");

		foreach (var Xdoc in DOX)
        {
            var root = Xdoc["Root"];

            foreach (XmlNode node in root)
            {
				#region Player
				if (node.Name == "Player")
                {
                    int playerHealth = XML_Loader.getAttInt(node, "Health");

                    float movementSpeed = XML_Loader.getAttFlt(node, "MovementSpeed");
                    float turnSpeed = XML_Loader.getAttFlt(node, "TurnSpeed");

                    PlayerXmlData newPlayer = new PlayerXmlData(playerHealth, movementSpeed, turnSpeed);
                    database.players.Add(newPlayer);
                }
				#endregion
				#region Weapon
				if (node.Name == "Weapon")
                {
                    string weaponType = XML_Loader.getAttStr(node, "Type");
                    string weaponName = XML_Loader.getAttStr(node, "Name");

                    int damage = XML_Loader.getAttInt(node, "Damage");
                    int accuracy = XML_Loader.getAttInt(node, "Accuracy");
                    int heat = XML_Loader.getAttInt(node, "Heat");

                    WeaponXmlData newWeapon = new WeaponXmlData(weaponType, weaponName, damage, accuracy, heat);
                    database.weapons.Add(newWeapon);
                }
				#endregion
				#region Enemy
				if (node.Name == "Enemy")
                {
                    string enemyType = XML_Loader.getAttStr(node, "Type");

                    int health = XML_Loader.getAttInt(node, "Health");
                    int damage = XML_Loader.getAttInt(node, "Damage");

                    EnemyXmlData newEnemy = new EnemyXmlData(enemyType, health, damage);
                    database.enemies.Add(newEnemy);
                }
				#endregion
				#region Obstacle
                if (node.Name == "Obstacle")
                {
                    string obstacleType = XML_Loader.getAttStr(node, "Type");

                    int health = XML_Loader.getAttInt(node, "Health");

                    ObstacleXmlData newObstacle = new ObstacleXmlData(obstacleType, health);
                    database.obstacles.Add(newObstacle);
                }
				#endregion
            }
		}
    }
}
