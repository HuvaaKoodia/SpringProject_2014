using System;
using System.Collections.Generic;

public class PoolContainerXmlData
{
	public string Name{get;private set;}
	public Dictionary<string,PoolXmlData> Pools{get;private set;}

	public PoolContainerXmlData(string name){
		Name=name;
		Pools=new Dictionary<string, PoolXmlData>();
	}
	
	public string GetRandomItem (string pool)
	{
		if (!Pools.ContainsKey(pool)){
			UnityEngine.Debug.Log("PoolContainer doesn't have a pool: "+pool + " in "+Name);
			return null;
		}
		var p=Pools[pool];
		return p.GetRandomItem();
	}

	public void AddPool (string pool)
	{
		if (Pools.ContainsKey(pool)){
			UnityEngine.Debug.Log("PoolContainer already has a pool: "+pool+" in "+Name);
			return;
		}
		Pools.Add(pool,new PoolXmlData());
	}
}


