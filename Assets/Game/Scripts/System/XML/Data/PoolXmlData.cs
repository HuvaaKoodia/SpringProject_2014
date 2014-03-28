using System;
using System.Collections.Generic;

public class PoolXmlData
{
	public List<PoolItemXmlData> Pool{get;private set;}

	public PoolXmlData(){
		Pool=new List<PoolItemXmlData>();
	}
	
	public string GetRandomItem ()
	{
		int pool_size=0;
		foreach (var i in Pool){
			pool_size+=i.weight;
		}

		int r=Subs.GetRandom(pool_size);
		int t=0;
		foreach (var i in Pool){
			if (r<t+i.weight){
				return i.item;
			}
			t+=i.weight;
		}
		return "";
	}

	public void AddItem(PoolItemXmlData item){
		Pool.Add(item);
	}
}


