using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class XMLDataLoader : XML_Loader
{
    public static void Read(GameDatabase database)
    {
        checkFolder("Data/Weapons");

        var files = Directory.GetFiles("Data/Weapons");

        #region Weapons
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

                    WeaponData newWeapon = new WeaponData(weaponType, weaponName, damage, accuracy, heat);
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

                    EnemyData newEnemy = new EnemyData(enemyType, health, damage);
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

                    ObstacleData newObstacle = new ObstacleData(obstacleType, health);
                    database.obstacles.Add(newObstacle);
                }
            }
        }
        #endregion
    }
}
