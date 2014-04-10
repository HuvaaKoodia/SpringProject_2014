using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionXmlData{
    public string Description;
    public string RewardClass,LootPool;
	public PoolXmlData LootQualityPool=new PoolXmlData();
	public int TravelTimeMin,TravelTimeMax,ExpirationTimeMin,ExpirationTimeMax;
	public PoolContainerXmlData StatusContainer=new PoolContainerXmlData("Status");
    public List<string> ShipsTypes=new List<string>();
    public List<string> PrimaryObjectives=new List<string>();
    public List<string> SecondaryObjectives=new List<string>();
}
