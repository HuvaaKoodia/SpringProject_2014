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
	
	// Use this for initialization
	void Start () {
		_FinanceManager = new FinanceManager();
		
		InitializeValues();
	}
	
	// Update is called once per frame
	void Update () {
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

		ExistingCash.text = _FinanceManager.existing_cash.ToString();

		if(ShortenDebt.Count > 0)
		{
			for(int i = 0; i < ShortenDebt.Count; i++)
			{
				ShortenDebt[i].text = "1000";
			}
		}
	}

	//function that updates variables
	void UpdateValues()
	{
		Days.text = _FinanceManager.days_till_update.ToString();
		PlayerMoney.text = _FinanceManager.player_money.ToString();

		var FM_l = _FinanceManager.listofdebts;
		
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
					MonthlyCut[i].text = FM_l[i].monthly_cut.ToString();
				}
			}
			
			if(Interest.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					Interest[i].text = FM_l[i].interest.ToString() + "(" + FM_l[i].interest_percent.ToString() + "%)";
				}
			}

			if(Payments.Count > 0)
			{
				for(int i = 0; i < FM_l.Count; i++)
				{
					Payments[i].text = _FinanceManager.listofdebts[i].debt_payment.ToString();
				}
				Payments[Payments.Count - 1].text = _FinanceManager.payment_total.ToString();
			}

//			if(ShortenDebt.Count > 0)
//			{
//				for(int i = 0; i < FM_l.Count; i++)
//				{
//					ShortenDebt[i].text = FM_l[i].shorten_debt.ToString();
//				}
//			}
		}
		
		ExistingCash.text = _FinanceManager.existing_cash.ToString();
	}

	//function that increases the value of Shorten Debt By component of Debt1
	public void IncreaseShortDebt1()
	{
		int value = int.Parse(ShortenDebt[1].text);
		value += 100;

		if(_FinanceManager.listofdebts.Count > 0)
		{
			_FinanceManager.listofdebts[0].shorten_debt = value;
		}
		ShortenDebt[1].text = value.ToString();
		ShortenDebt[0].text = value.ToString();
	}

	//function that increases the value of Shorten Debt By component of Debt2
	public void IncreaseShortDebt2()
	{
		int value = int.Parse(ShortenDebt[3].text);
		value += 100;
		
//		if(_FinanceManager.listofdebts.Count > 0 && _FinanceManager.listofdebts.Count <= 1)
//		{
//			if(_FinanceManager.listofdebts[1] != null)
//			{
//				_FinanceManager.listofdebts[1].shorten_debt = value;
//			}
//		}
		ShortenDebt[3].text = value.ToString();
		ShortenDebt[2].text = value.ToString();
	}

	//function that increases the value of Shorten Debt By component of Debt3
	public void IncreaseShortDebt3()
	{
		int value = int.Parse(ShortenDebt[5].text);
		value += 100;
		
//		if(_FinanceManager.listofdebts.Count > 0)
//		{
//			if(_FinanceManager.listofdebts[2] != null)
//			{
//				_FinanceManager.listofdebts[2].shorten_debt = value;
//			}
//		}
		ShortenDebt[5].text = value.ToString();
		ShortenDebt[4].text = value.ToString();
	}

	//function that decreases the value of Shorten Debt By component of Debt1
	public void DecreaseShortDebt1()
	{
		int value = int.Parse(ShortenDebt[1].text);
		value -= 100;

		if(value <= 1000)
		{
			value = 1000;
		}
		
//		if(_FinanceManager.listofdebts.Count > 0)
//		{
//			_FinanceManager.listofdebts[0].shorten_debt = value;
//		}
		ShortenDebt[1].text = value.ToString();
		ShortenDebt[0].text = value.ToString();
	}

	//function that decreases the value of Shorten Debt By component of Debt2
	public void DecreaseShortDebt2()
	{
		int value = int.Parse(ShortenDebt[3].text);
		value -= 100;

		if(value <= 1000)
		{
			value = 1000;
		}
		
//		if(_FinanceManager.listofdebts.Count > 0)
//		{
//			if(_FinanceManager.listofdebts[1] != null)
//			{
//				_FinanceManager.listofdebts[1].shorten_debt = value;
//			}
//		}
		ShortenDebt[3].text = value.ToString();
		ShortenDebt[2].text = value.ToString();
	}

	//function that decreases the value of Shorten Debt By component of Debt3
	public void DecreaseShortDebt3()
	{
		int value = int.Parse(ShortenDebt[5].text);
		value -= 100;

		if(value <= 1000)
		{
			value = 1000;
		}
		
//		if(_FinanceManager.listofdebts.Count > 0)
//		{
//			if(_FinanceManager.listofdebts[2] != null)
//			{
//				_FinanceManager.listofdebts[2].shorten_debt = value;
//			}
//		}
		ShortenDebt[5].text = value.ToString();
		ShortenDebt[4].text = value.ToString();
	}

	//function that activates DebtAdded and deactivates DebtEmpty of Debt1
	public void ActivateDebt1()
	{
		if(DebtsActivate.Count > 0)
		{
			_FinanceManager.AddDebt();
			ProcessPanel1(0);

			DebtsActivate[0].SetActive(true);
			DebtsActivate[1].SetActive(false);

			_FinanceManager.add_debt = false;

		}
	}

	//function that activates DebtAdded and deactivates DebtEmpty of Debt2
	public void ActivateDebt2()
	{
		if(DebtsActivate.Count > 0)
		{
			if(DebtsActivate[0].activeInHierarchy)
			{
				_FinanceManager.AddDebt();
				ProcessPanel1(1);

				DebtsActivate[2].SetActive(true);
				DebtsActivate[3].SetActive(false);

				_FinanceManager.add_debt = false;
			}
		}
	}

	//function that activates DebtAdded and deactivates DebtEmpty of Debt3
	public void ActivateDebt3()
	{
		if(DebtsActivate.Count > 0)
		{
			if(DebtsActivate[2].activeInHierarchy)
			{
				_FinanceManager.AddDebt();
				ProcessPanel1(2);

				DebtsActivate[4].SetActive(true);
				DebtsActivate[5].SetActive(false);

				_FinanceManager.add_debt = false;
			}
		}
	}

	//functions that calls all the necessary calculation functions to update the labels in Panel1
	void ProcessPanel1(int index)
	{
		if(index >= 0 && index < 3)
		{
			_FinanceManager.CalcInterestPercent();
			_FinanceManager.listofdebts[index].CalcInterest();
			_FinanceManager.listofdebts[index].CalcDebtPayment();
			_FinanceManager.CalcPaymentTotal();
			_FinanceManager.CalcExistingCash();
		}
		else
		{
			Debug.Log("Key in 0 to 2 only.");
		}
	}
}