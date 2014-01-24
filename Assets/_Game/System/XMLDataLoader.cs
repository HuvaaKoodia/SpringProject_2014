using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class XMLDataLoader : XML_Loader
{
    public static void Read(GameDatabase database)
    {
        checkFolder("Data/Player");
        var files = Directory.GetFiles("Data/Player");
  
        #region Player

        foreach (var f in files)
        {
            var Xdoc = new XmlDocument();
            Xdoc.Load(f);

            var root = Xdoc["Root"];

            foreach (XmlNode node in root)
            {
                if (node.Name == "Player")
                {
                    int playerHealth = XML_Loader.getAttInt(node, "Health");

                    float movementSpeed = XML_Loader.getAttFlt(node, "MovementSpeed");
                    float turnSpeed = XML_Loader.getAttFlt(node, "TurnSpeed");

                    PlayerXmlData newPlayer = new PlayerXmlData(playerHealth, movementSpeed, turnSpeed);
                    database.players.Add(newPlayer);
                }
            }
        }

        #endregion

        #region Weapons
        
        checkFolder("Data/Weapons");
        files = Directory.GetFiles("Data/Weapons");

        foreach (var f in files)
        {
            var Xdoc = new XmlDocument();
            Xdoc.Load(f);

            var root = Xdoc["Root"];

            foreach (XmlNode node in root)
            {
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
            }
        }
        #endregion

        #region Enemies
        checkFolder("Data/Enemies");
        files = Directory.GetFiles("Data/Enemies");
        foreach (var f in files)
        {
            var Xdoc = new XmlDocument();
            Xdoc.Load(f);

            var root = Xdoc["Root"];

            foreach (XmlNode node in root)
            {
                if (node.Name == "Enemy")
                {
                    string enemyType = XML_Loader.getAttStr(node, "Type");

                    int health = XML_Loader.getAttInt(node, "Health");
                    int damage = XML_Loader.getAttInt(node, "Damage");

                    EnemyXmlData newEnemy = new EnemyXmlData(enemyType, health, damage);
                    database.enemies.Add(newEnemy);
                }
            }
        }
        #endregion

        #region Obstacles
        checkFolder("Data/Obstacles");
        files = Directory.GetFiles("Data/Obstacles");
        foreach (var f in files)
        {
            var Xdoc = new XmlDocument();
            Xdoc.Load(f);

            var root = Xdoc["Root"];

            foreach (XmlNode node in root)
            {
                if (node.Name == "Obstacle")
                {
                    string obstacleType = XML_Loader.getAttStr(node, "Type");

                    int health = XML_Loader.getAttInt(node, "Health");

                    ObstacleXmlData newObstacle = new ObstacleXmlData(obstacleType, health);
                    database.obstacles.Add(newObstacle);
                }
            }
        }
        #endregion
    }
}
