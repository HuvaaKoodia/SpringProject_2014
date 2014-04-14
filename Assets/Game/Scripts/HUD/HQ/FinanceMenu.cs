using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinanceMenu : MonoBehaviour {
	
	public FinanceManager _FinanceManager;
	
	public UILabel Days;										//variable that keeps track of the label for Days Until Next Update
	public UILabel PlayerMoney;									//variable that keeps track of the label for Player Money 
	public UILabel ExistingCash;								//variable that keeps track of the label for Existing Cash
	
	public List<UILabel> LeftToBePayed;							//List that keeps track of the amount Left To Be Paid for each Debt
	public List<UILabel> MonthlyCut;							//List that keeps track of the Monthly Cut for each Debt
	public List<UILabel> Interest;								//List that keeps track of the Interest for each Debt
	public List<UILabel> Payments;								//List that keeps track of the Debt Payments for each debt and Payment Total
	public List<UILabel> ShortenDebt;							//List that keeps track of the Shorten Debt By component for each debt
	public List<GameObject> DebtsActivate;						//List that keeps track of the Debt types ie, DebtAdded and DebtEmpty for each Debt
	
	private float ticks = 0;									//variable to keep track of time
	
	// Use this for initialization
	void Start () {
		_FinanceManager = new FinanceManager();
		InitializeValues();
	}
	
	// Update is called once per frame
	void Update () {
		DaysTimer();
		UpdateValues();
	}
	
	//function that inializes variables
	void InitializeValues()
	{
		Days.text = _FinanceManager.days_till_update.ToString();
		
		PlayerMoney.text = _FinanceManager.player_money.ToString();
		
		if(LeftToBePayed.Count > 0)
		{
			for(int i = 0; i < LeftToBePayed.Count; i++)
			{
				LeftToBePayed[i].text = "0";
			}
		}
		
		if(MonthlyCut.Count > 0)
		{
			for(int i = 0; i < MonthlyCut.Count; i++)
			{
				MonthlyCut[i].text = "0";
			}
		}
		
		if(Interest.Count > 0)
		{
			for(int i = 0; i < Interest.Count; i++)
			{
				Interest[i].text = "0";
			}
		}
		
		if(Payments.Count > 0)
		{
			for(int i = 0; i < Payments.Count - 1; i++)
			{
				Payments[i].text = "0";
			}
			Payments[Payments.Count - 1].text = _FinanceManager.payment_total.ToString();
		}

		_FinanceManager.CalcExistingCash();
		ExistingCash.text = _FinanceManager.existing_cash.ToString();
		
		if(ShortenDebt.Count > 0)
		{
			for(int i = 0; i < ShortenDebt.Count; i++)
			{
				if(i%2 != 0)
				{
					ShortenDebt[i].text = "10000";
				}
				else
				{
					ShortenDebt[i].text = "1000";
				}
			}
		}
	}
	
	//function that updates variables
	void UpdateValues()
	{
		Days.text = _FinanceManager.days_till_update.ToString();

		_FinanceManager.UpdatePlayerMoney();
		PlayerMoney.text = _FinanceManager.player_money.ToString();
		
		var FM_l = _FinanceManager.listofdebts;

		//as long as the player has debts, update the necessary values
		if(FM_l.Count > 0)
		{
			if(LeftToBePayed.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					//Debug.Log(FM_l.Count);
					LeftToBePayed[i].text = FM_l[i].left_tb_payed.ToString();
				}
			}
			
			if(MonthlyCut.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					MonthlyCut[i].text = FM_l[i].monthly_cut.ToString();
				}
			}
			
			if(Interest.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					Interest[i].text = ((int)FM_l[i].interest).ToString() + "\n(" + (FM_l[i].interest_percent * 100).ToString() + "%)";
				}
			}
			
			if(Payments.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					Payments[i].text = ((int)FM_l[i].debt_payment).ToString();
				}
				Payments[Payments.Count - 1].text = ((int)_FinanceManager.payment_total).ToString();
			}
		}
		
		ExistingCash.text = _FinanceManager.existing_cash.ToString();
	}
	
	//function that increases the value of Shorten Debt By component of Debt1
	public void IncreaseShortDebt1()
	{
		//value for DebtEmpty
		int value1 = int.Parse(ShortenDebt[1].text);
		value1 += 1000;

		//cap for the value
		ShortenDebt[1].text = value1.ToString();
		if(int.Parse(ShortenDebt[1].text) >= 50000)
		{
			ShortenDebt[1].text = "50000";
		}

		//value for DebtActive
		int value0 = int.Parse(ShortenDebt[0].text);
		int ltbp = int.Parse(LeftToBePayed[0].text);

		//value can only increment if left_tb_payed value is > 0
		if(ltbp != 0)
		{
			value0 += 1000;
		}

		//cap for the value
		ShortenDebt[0].text = value0.ToString();
		if(int.Parse(ShortenDebt[0].text) >= ltbp && ltbp != 0)
		{
			ShortenDebt[0].text = LeftToBePayed[0].text;
		}
	}
	
	//function that increases the value of Shorten Debt By component of Debt2
	public void IncreaseShortDebt2()
	{
		//value for DebtEmpty
		int value3 = int.Parse(ShortenDebt[3].text);
		value3 += 1000;
	
		//cap for the value
		ShortenDebt[3].text = value3.ToString();
		if(int.Parse(ShortenDebt[3].text) >= 50000)
		{
			ShortenDebt[3].text = "50000";
		}

		//value for DebtActive
		int value2 = int.Parse(ShortenDebt[2].text);
		int ltbp = int.Parse(LeftToBePayed[1].text);

		//value can only increment if left_tb_payed value is > 0
		if(ltbp != 0)
		{
			value2 += 1000;
		}

		//cap for the value
		ShortenDebt[2].text = value2.ToString();
		if(int.Parse(ShortenDebt[2].text) >= ltbp && ltbp != 0)
		{
			ShortenDebt[2].text = LeftToBePayed[1].text;
		}
	}
	
	//function that increases the value of Shorten Debt By component of Debt3
	public void IncreaseShortDebt3()
	{
		//value for DebtEmpty
		int value5 = int.Parse(ShortenDebt[5].text);
		value5 += 1000;

		//cap for the value
		ShortenDebt[5].text = value5.ToString();
		if(int.Parse(ShortenDebt[5].text) >= 50000)
		{
			ShortenDebt[5].text = "50000";
		}

		//value for DebtActive
		int value4 = int.Parse(ShortenDebt[4].text);
		int ltbp = int.Parse(LeftToBePayed[2].text);

		//value can only increment if left_tb_payed value is > 0
		if(ltbp != 0)
		{
			value4 += 1000;
		}

		//cap for the value
		ShortenDebt[4].text = value4.ToString();
		if(int.Parse(ShortenDebt[4].text) >= ltbp && ltbp != 0)
		{
			ShortenDebt[4].text = LeftToBePayed[2].text;
		}
	}
	
	//function that decreases the value of Shorten Debt By component of Debt1
	public void DecreaseShortDebt1()
	{
		//value for DebtEmpty
		int value1 = int.Parse(ShortenDebt[1].text);
		value1 -= 1000;

		//cap for the value
		ShortenDebt[1].text = value1.ToString();
		if(int.Parse(ShortenDebt[1].text) <= 10000)
		{
			ShortenDebt[1].text = "10000";
		}

		//value for DebtActive
		int value0 = int.Parse(ShortenDebt[0].text);
		value0 -= 1000;

		//cap for the value
		ShortenDebt[0].text = value0.ToString();
		if(int.Parse(ShortenDebt[0].text) <= 1000)
		{
			ShortenDebt[0].text = "1000";
		}
	}
	
	//function that decreases the value of Shorten Debt By component of Debt2
	public void DecreaseShortDebt2()
	{
		//value for DebtEmpty
		int value3 = int.Parse(ShortenDebt[3].text);
		value3 -= 1000;

		//cap for the value
		ShortenDebt[3].text = value3.ToString();
		if(int.Parse(ShortenDebt[3].text) <= 10000)
		{
			ShortenDebt[3].text = "10000";
		}

		//value for DebtActive
		int value2 = int.Parse(ShortenDebt[2].text);
		value2 -= 1000;

		//cap for the value
		ShortenDebt[2].text = value2.ToString();
		if(int.Parse(ShortenDebt[2].text) <= 1000)
		{
			ShortenDebt[2].text = "1000";
		}
	}
	
	//function that decreases the value of Shorten Debt By component of Debt3
	public void DecreaseShortDebt3()
	{
		//value for DebtEmpty
		int value5 = int.Parse(ShortenDebt[5].text);
		value5 -= 1000;

		//cap for the value
		ShortenDebt[5].text = value5.ToString();
		if(int.Parse(ShortenDebt[5].text) <= 10000)
		{
			ShortenDebt[5].text = "10000";
		}

		//value for DebtActive
		int value4 = int.Parse(ShortenDebt[4].text);
		value4 -= 1000;

		//cap for the value
		ShortenDebt[4].text = value4.ToString();
		if(int.Parse(ShortenDebt[4].text) <= 1000)
		{
			ShortenDebt[4].text = "1000";
		}
	}
	
	//function that activates DebtAdded and deactivates DebtEmpty of Debt1
	public void ActivateDebt1()
	{
		//as long as there are DebtsActive in the List
		if(DebtsActivate.Count > 0)
		{
			//add a debt
			_FinanceManager.AddDebt();
		
			_FinanceManager.listofdebts[0].left_tb_payed = int.Parse(ShortenDebt[1].text);

			//process the necessary values for the current panel
			ProcessPanel(0);
			
			//enable DebtActive and disable DebtEmpty for current panel
			DebtsActivate[0].SetActive(true);
			DebtsActivate[1].SetActive(false);

			_FinanceManager.add_debt = false;
			
		}
	}
	
	//function that activates DebtAdded and deactivates DebtEmpty of Debt2
	public void ActivateDebt2()
	{
		//as long as there are DebtsActive in the List
		if(DebtsActivate.Count > 0)
		{
			//as long as Panel2's debt has been activated
			if(DebtsActivate[0].activeInHierarchy)
			{
				//add a debt
				_FinanceManager.AddDebt();

				_FinanceManager.listofdebts[1].left_tb_payed = int.Parse(ShortenDebt[3].text);

				//process the necessary values for the current panel
				ProcessPanel(1);
				
				//enable DebtActive and disable DebtEmpty for current panel
				DebtsActivate[2].SetActive(true);
				DebtsActivate[3].SetActive(false);

				_FinanceManager.add_debt = false;
			}
		}
	}
	
	//function that activates DebtAdded and deactivates DebtEmpty of Debt3
	public void ActivateDebt3()
	{
		//as long as there are DebtsActive in the List
		if(DebtsActivate.Count > 0)
		{
			//as long as Panel3's debt has been activated
			if(DebtsActivate[2].activeInHierarchy)
			{
				//add a debt
				_FinanceManager.AddDebt();
			
				_FinanceManager.listofdebts[2].left_tb_payed = int.Parse(ShortenDebt[5].text);

				//process the necessary values for the current panel
				ProcessPanel(2);
				
				//enable DebtActive and disable DebtEmpty for current panel
				DebtsActivate[4].SetActive(true);
				DebtsActivate[5].SetActive(false);

				_FinanceManager.add_debt = false;
			}
		}
	}
	
	//functions that calls all the necessary calculation functions to update the labels in Panel(index+2)
	private void ProcessPanel(int index)
	{
		if(index >= 0 && index < 3)
		{
			_FinanceManager.listofdebts[index].CalcMonthlyCut();
			_FinanceManager.CalcInterestPercent();
			_FinanceManager.listofdebts[index].CalcInterest();
			_FinanceManager.listofdebts[index].CalcDebtPayment();
			_FinanceManager.CalcPaymentTotal(false);
			_FinanceManager.CalcExistingCash();
		}
		else
		{
			Debug.Log("Key in 0 to 2 only.");
			Debug.Break();
		}
	}

	//function to Shorten the amount left to be payed in Panel2
	public void ShortenDebt1()
	{
		//as long as value is > 0, allow decrement of value
		//if(_FinanceManager.listofdebts[0].left_tb_payed >= 0)
		if(int.Parse(LeftToBePayed[0].text) >= 0)
		{
			_FinanceManager.listofdebts[0].left_tb_payed -= int.Parse(ShortenDebt[0].text);

			//as long as value is <= 0, limit to 0
			if(_FinanceManager.listofdebts[0].left_tb_payed < 0)
			{
				_FinanceManager.listofdebts[0].left_tb_payed = 0;

				//enable DebtActive and disable DebtEmpty for current panel
				DebtsActivate[0].SetActive(false);
				DebtsActivate[1].SetActive(true);
			}
		}

		//update value due to the changes
		_FinanceManager.UpdateValues();
	}

	//function to Shorten the amount left to be payed in Panel3
	public void ShortenDebt2()
	{
		int value = int.Parse(LeftToBePayed[1].text);
		//as long as value is > 0, allow decrement of value
		//if(_FinanceManager.listofdebts[1].left_tb_payed >= 0)
		if(int.Parse(LeftToBePayed[1].text) >= 0)
		{
			//_FinanceManager.listofdebts[1].left_tb_payed -= int.Parse(ShortenDebt[2].text);
			_FinanceManager.listofdebts[1].left_tb_payed -= int.Parse(ShortenDebt[2].text);
			if(_FinanceManager.listofdebts[1].left_tb_payed < 0)
			{
				_FinanceManager.listofdebts[1].left_tb_payed = 0;

				//enable DebtActive and disable DebtEmpty for current panels
				DebtsActivate[2].SetActive(false);
				DebtsActivate[3].SetActive(true);
			}
			//update value due to the changes
			_FinanceManager.UpdateValues();
		}
	}

	//function to Shorten the amount left to be payed in Panel4
	public void ShortenDebt3()
	{
		//as long as value is > 0, allow decrement of value
		//if(_FinanceManager.listofdebts[2].left_tb_payed >= 0)
		if(int.Parse(LeftToBePayed[2].text) >= 0)
		{
			_FinanceManager.listofdebts[2].left_tb_payed -= int.Parse(ShortenDebt[4].text);

			//as long as value is <= 0, limit to 0
			if(_FinanceManager.listofdebts[2].left_tb_payed < 0)
			{
				_FinanceManager.listofdebts[2].left_tb_payed = 0;

				//enable DebtActive and disable DebtEmpty for current panel
				DebtsActivate[4].SetActive(false);
				DebtsActivate[5].SetActive(true);
			}
		}

		//update value due to the changes
		_FinanceManager.UpdateValues();
	}

	//function to update time for number of days until next update
	private void DaysTimer()
	{
		//1 Day = 60s * 60mins * 24hrs
		//		= 86400s
		//as long as a day has passed, update the value for the number of days until next updtae.
		//then reset timer
		if(ticks >= 86400
		   || Input.GetKeyDown("p")
		   )
		{
			_FinanceManager.day_pass = true;
			_FinanceManager.UpdateDays();
			ticks = 0.0f;
		}
		else
		{
			ticks += Time.deltaTime;
		}
	}
}