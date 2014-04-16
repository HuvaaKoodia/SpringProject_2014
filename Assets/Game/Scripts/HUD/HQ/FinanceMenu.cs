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
	private void InitializeValues()
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
	private void UpdateValues()
	{
		Days.text = _FinanceManager.days_till_update.ToString();

		_FinanceManager.UpdatePlayerMoney();
		PlayerMoney.text = _FinanceManager.player_money.ToString();
		
		var FM_l = _FinanceManager.listofdebts;

		_FinanceManager.UpdateValues();
		//as long as the player has debts, update the necessary values
		if(FM_l.Count > 0)
		{
			if(LeftToBePayed.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					LeftToBePayed[i].text = FM_l[i].left_tb_payed.ToString();
				}
			}
			
			if(MonthlyCut.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					if(!FM_l[i].active)
					{
						FM_l[i].monthly_cut = 0;
					}
					MonthlyCut[i].text = FM_l[i].monthly_cut.ToString();
				}
			}
			
			if(Interest.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					if(!FM_l[i].active)
					{
						FM_l[i].interest = 0;
					}
					Interest[i].text = ((int)FM_l[i].interest).ToString() + "\n(" + (FM_l[i].interest_percent * 100).ToString() + "%)";
				}
			}
			
			if(Payments.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					if(!FM_l[i].active)
					{
						FM_l[i].debt_payment = 0;
					}
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
		IncrementProcess(1, 0);
	}
	
	//function that increases the value of Shorten Debt By component of Debt2
	public void IncreaseShortDebt2()
	{
		IncrementProcess(3, 1);
	}
	
	//function that increases the value of Shorten Debt By component of Debt3
	public void IncreaseShortDebt3()
	{
		IncrementProcess(5, 2);
	}
	
	//function that decreases the value of Shorten Debt By component of Debt1
	public void DecreaseShortDebt1()
	{
		DecrementProcess(1);
	}
	
	//function that decreases the value of Shorten Debt By component of Debt2
	public void DecreaseShortDebt2()
	{
		DecrementProcess(3);
	}
	
	//function that decreases the value of Shorten Debt By component of Debt3
	public void DecreaseShortDebt3()
	{
		DecrementProcess(5);
	}
	
	//function that activates DebtAdded and deactivates DebtEmpty of Debt1
	public void ActivateDebt1()
	{
		//as long as there are DebtsActive in the List
		if(DebtsActivate.Count > 0)
		{
			ActivateDebtProcess(0, 1);
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
				ActivateDebtProcess(1, 3);
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
				ActivateDebtProcess(2, 5);
			}
		}
	}

	//function to process the activation and deactivation of specific debts
	private void ActivateDebtProcess(int index_1, int index_2)
	{
		//add a debt
		_FinanceManager.AddDebt(index_1);
		
		_FinanceManager.listofdebts[index_1].left_tb_payed = int.Parse(ShortenDebt[index_2].text);
		
		//process the necessary values for the current panel
		ProcessPanel(index_1);
		
		//enable DebtActive and disable DebtEmpty for current panel
		DebtsActivate[index_2 - 1].SetActive(true);
		DebtsActivate[index_2].SetActive(false);
	}

	//function that increases ShortenDebt value
	private void IncrementProcess(int sd_index, int ltbp_index)
	{
		//value for DebtEmpty
		int value1 = int.Parse(ShortenDebt[sd_index].text);
		value1 += 1000;
		
		//cap for the value
		ShortenDebt[sd_index].text = value1.ToString();
		if(int.Parse(ShortenDebt[sd_index].text) >= 50000)
		{
			ShortenDebt[sd_index].text = "50000";
		}
		
		//value for DebtActive
		int value = int.Parse(ShortenDebt[sd_index - 1].text);
		int ltbp = int.Parse(LeftToBePayed[ltbp_index].text);
		
		//value can only increment if left_tb_payed value is > 0
		if(ltbp != 0)
		{
			value += 1000;
		}
		
		//cap for the value
		ShortenDebt[sd_index - 1].text = value.ToString();
		if(int.Parse(ShortenDebt[sd_index - 1].text) >= ltbp && ltbp != 0)
		{
			ShortenDebt[sd_index - 1].text = LeftToBePayed[ltbp_index].text;
		}
	}

	//function that decreases ShortenDebt value
	private void DecrementProcess(int sd_index)
	{
		//value for DebtEmpty
		int value1 = int.Parse(ShortenDebt[sd_index].text);
		value1 -= 1000;
		
		//cap for the value
		ShortenDebt[sd_index].text = value1.ToString();
		if(int.Parse(ShortenDebt[sd_index].text) <= 10000)
		{
			ShortenDebt[sd_index].text = "10000";
		}
		
		//value for DebtActive
		int value = int.Parse(ShortenDebt[sd_index - 1].text);
		value -= 1000;
		
		//cap for the value
		ShortenDebt[sd_index - 1].text = value.ToString();
		if(int.Parse(ShortenDebt[sd_index - 1].text) <= 1000)
		{
			ShortenDebt[sd_index - 1].text = "1000";
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
		if(int.Parse(LeftToBePayed[0].text) > 0)
		{
			_FinanceManager.listofdebts[0].left_tb_payed -= int.Parse(ShortenDebt[0].text);
		}
		else
		{
			ShortenDebtCheck(0, 0);
			ShortenDebtCheck();
		}
	}

	//function to Shorten the amount left to be payed in Panel3
	public void ShortenDebt2()
	{
		if(int.Parse(LeftToBePayed[1].text) > 0)
		{
			_FinanceManager.listofdebts[1].left_tb_payed -= int.Parse(ShortenDebt[2].text);
		}
		else
		{
			ShortenDebtCheck(1, 2);
			ShortenDebtCheck();
		}
	}

	//function to Shorten the amount left to be payed in Panel4
	public void ShortenDebt3()
	{
		if(int.Parse(LeftToBePayed[2].text) > 0)
		{
			_FinanceManager.listofdebts[2].left_tb_payed -= int.Parse(ShortenDebt[4].text);
		}
		else
		{
			ShortenDebtCheck(2, 4);
			ShortenDebtCheck();
		}
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

	//function to ensure such that ShortenDebt values in the different Debt screens are correct
	private void ShortenDebtCheck()
	{
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

	//function to perform the necessary steps on the specific Debts if left to be payed amount < 0
	private void ShortenDebtCheck(int debt_index, int debt_active_index)
	{
		_FinanceManager.listofdebts[debt_index].left_tb_payed = 0;
		
		//enable DebtActive and disable DebtEmpty for current panel
		DebtsActivate[debt_active_index].SetActive(false);
		DebtsActivate[debt_active_index + 1].SetActive(true);
		
		_FinanceManager.listofdebts[debt_index].active = false;
	}
}