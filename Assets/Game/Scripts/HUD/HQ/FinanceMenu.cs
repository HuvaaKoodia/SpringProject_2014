using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinanceMenu : MonoBehaviour {

	public UILabel Days;										//variable that keeps track of the label for Days Until Next Update
	public UILabel PlayerMoney;									//variable that keeps track of the label for Player Money 
	public UILabel ExistingCash;								//variable that keeps track of the label for Existing Cash
	
	public List<UILabel> LeftToBePayed;							//List that keeps track of the amount Left To Be Paid for each Debt
	public List<UILabel> MonthlyCut;							//List that keeps track of the Monthly Cut for each Debt
	public List<UILabel> Interest;								//List that keeps track of the Interest for each Debt
	public List<UILabel> Payments;								//List that keeps track of the Debt Payments for each debt and Payment Total
	public List<UILabel> ShortenDebt;							//List that keeps track of the Shorten Debt By component for each debt
	public List<GameObject> DebtsActivate;						//List that keeps track of the Debt types ie, DebtAdded and DebtEmpty for each Debt
	
	public FinanceManager _FinanceManager;
	
	// Use this for initialization
	void Start () {
		InitializeValues();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateValues();
	}

	//function to search for currently active Debts and display in the FinanceScreen
	private void SearchActiveDebt()
	{
		for(int i = 0; i < _FinanceManager.listofdebts.Count; i++)
		{
			if(_FinanceManager.listofdebts[i].active)
			{
				DebtsActivate[(2 * i)].SetActive(true);
				DebtsActivate[(2 * i) + 1].SetActive(false);
			}
		}
	}

	//function to set the instance of FinanceManager to refer to
	public void SetFinanceManager(FinanceManager FM)
	{
		_FinanceManager = FM;
		SearchActiveDebt();
	}

	//INITIALIZE LABELS FOR FINANCE SCREEN PROPERTIES
	//function that inializes variables
	private void InitializeValues()
	{
		Days.text = _FinanceManager.days_till_update.ToString();

		PlayerMoney.text = _FinanceManager.player_money.ToString();
		ExistingCash.text = _FinanceManager.existing_cash.ToString();
	}

	//UPDATE LABELS FOR FINANCE SCREEN PROPERTIES
	//function to update the value of amount left to be payed per debt
	private void UpdateLeftToBePayed(int i)
	{
		var FM_l_i = _FinanceManager.listofdebts[i];
		if(LeftToBePayed.Count > 0)
		{
			LeftToBePayed[i].text = FM_l_i.left_tb_payed.ToString();
		}
	}

	//function to update the value of monthly cut per debt
	private void UpdateMonthlyCut(int i)
	{
		var FM_l_i = _FinanceManager.listofdebts[i];
		if(MonthlyCut.Count > 0)
		{
			//as long as the debt is active, monthly_cut will have a value
			if(!FM_l_i.active)
			{
				FM_l_i.monthly_cut = 0;
			}
			else
			{
				_FinanceManager.CalcMonthlyCut();
				MonthlyCut[i].text = FM_l_i.monthly_cut.ToString();
			}
		}
	}

	//function to update the value of interest per debt
	private void UpdateInterest(int i)
	{
		var FM_l_i = _FinanceManager.listofdebts[i];
		if(Interest.Count > 0)
		{
			//as long as the debt is active, interest will have a value
			if(!FM_l_i.active)
			{
				FM_l_i.interest = 0;
			}
			else
			{
				Interest[i].text = ((int)FM_l_i.interest).ToString() + "\n(" + (FM_l_i.interest_percent * 100).ToString() + "%)";
			}
		}
	}

	//function to update the value of debt payment per debt and payment total
	private void UpdatePayments(int i)
	{
		var FM_l_i = _FinanceManager.listofdebts[i];
		if(Payments.Count > 0)
		{
			//as long as the debt is active, debt_payment will have a value
			if(!FM_l_i.active)
			{
				FM_l_i.debt_payment = 0;
			}
			else
			{
				Payments[i].text = ((int)FM_l_i.debt_payment).ToString();
			}
			Payments[Payments.Count - 1].text = ((int)_FinanceManager.payment_total).ToString();
		}
	}

	//function that updates variables
	private void UpdateValues()
	{
		//Days
		Days.text = _FinanceManager.days_till_update.ToString();

		//Player's money
		PlayerMoney.text = _FinanceManager.player_money.ToString();

		//amount left to be payed, monthly cut, interest, debt payments, payment total
		_FinanceManager.UpdateValues();

		var FM_l = _FinanceManager.listofdebts;
		
		//as long as the player has debts, update the necessary values
		if(FM_l.Count > 0)
		{
			for(int i = 0; i < FM_l.Count; i++)
			{
				UpdateLeftToBePayed(i);
				UpdateMonthlyCut(i);
				UpdateInterest(i);
				UpdatePayments (i);
			}
		}

		//existing cash
		ExistingCash.text = _FinanceManager.existing_cash.ToString();
	}

	//INCREASE
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

	//DECREASE
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

	//ACTIVATE
	//functions that calls all the necessary calculation functions to update the labels in Panel(index+2)
	private void ProcessPanel(int index)
	{
		if(index >= 0 && index < 3)
		{
			_FinanceManager.CalcMonthlyCut();
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

	//function to process the activation and deactivation of specific debts
	private void ActivateDebtProcess(int index_1, int index_2)
	{
		int amount=int.Parse(ShortenDebt[index_2].text);

		if (_FinanceManager.Player.Money<amount) return;
		//add a debt
		_FinanceManager.AddDebt(index_1);
		
		_FinanceManager.listofdebts[index_1].left_tb_payed = amount;
		
		//process the necessary values for the current panel
		ProcessPanel(index_1);
		
		//enable DebtAdded and disable DebtEmpty for current panel
		DebtsActivate[index_2 - 1].SetActive(true);
		DebtsActivate[index_2].SetActive(false);

		for(int i = 0; i < _FinanceManager.listofdebts.Count; i++)
		{
			_FinanceManager.original_debt_sum += _FinanceManager.listofdebts[i].left_tb_payed;
		}
	}
	
	//function that activates DebtAdded and deactivates DebtEmpty of Debt1
	public void ActivateDebt1()
	{
		//as long as there are DebtsActive in the List
		if(DebtsActivate.Count > 0)
		{
			ActivateDebtProcess(0, 1);

//			for(int i = 0; i < _FinanceManager.listofdebts.Count; i++)
//			{
//				_FinanceManager.listofdebts[i].original_debt_sum += _FinanceManager.listofdebts[i].left_tb_payed;
//				_FinanceManager.listofdebts[i].CalcMonthlyCut();
//			}
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

//				for(int i = 0; i < _FinanceManager.listofdebts.Count; i++)
//				{
//					_FinanceManager.listofdebts[i].original_debt_sum += _FinanceManager.listofdebts[i].left_tb_payed;
//					_FinanceManager.listofdebts[i].CalcMonthlyCut();
//				}
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

//				for(int i = 0; i < _FinanceManager.listofdebts.Count; i++)
//				{
//					_FinanceManager.listofdebts[i].original_debt_sum += _FinanceManager.listofdebts[i].left_tb_payed;
//					_FinanceManager.listofdebts[i].CalcMonthlyCut();
//				}
			}
		}
	}

	//SHORTEN DEBT
	//function to ensure such that ShortenDebt values in the different Debt screens are correct
	private void ShortenDebtCheck()
	{
		if(ShortenDebt.Count > 0)
		{
			for(int i = 0; i < ShortenDebt.Count; i++)
			{
				//let all odd debts be initialized to 10000 and all even debts be  initialized to 1000 in the list
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

	//function to deduct the amount of money to shorten the debt by from the Player's money
	private void DeductPlayerMoney(int index)
	{
		_FinanceManager.player_money -= int.Parse(ShortenDebt[index].text);
	}

	private void UpdateOriginalDebtSum()
	{
		_FinanceManager.original_debt_sum = 0.0f;
		for(int i = 0; i < _FinanceManager.listofdebts.Count; i++)
		{
			_FinanceManager.original_debt_sum += _FinanceManager.listofdebts[i].left_tb_payed;
		}
	}

	//function to Shorten the amount left to be payed in Panel2
	public void ShortenDebt1()
	{
		if(_FinanceManager.player_money >= int.Parse(ShortenDebt[0].text))
		{
			//has to be > 1000 to deactivate DebtAdded for the specific panel
			if(int.Parse(LeftToBePayed[0].text) > 1000)
			{
				_FinanceManager.listofdebts[0].left_tb_payed -= int.Parse(ShortenDebt[0].text);
				UpdateOriginalDebtSum();
			}
			else
			{
				ShortenDebtCheck(0, 0);
				ShortenDebtCheck();
			}

			DeductPlayerMoney(0);
		}
	}

	//function to Shorten the amount left to be payed in Panel3
	public void ShortenDebt2()
	{
		if(_FinanceManager.player_money >= int.Parse(ShortenDebt[0].text))
		{
			//has to be > 1000 to deactivate DebtAdded for the specific panel
			if(int.Parse(LeftToBePayed[1].text) > 1000)
			{
				_FinanceManager.listofdebts[1].left_tb_payed -= int.Parse(ShortenDebt[2].text);
				UpdateOriginalDebtSum();
			}
			else
			{
				ShortenDebtCheck(1, 2);
				ShortenDebtCheck();
			}

			DeductPlayerMoney(2);
		}
	}

	//function to Shorten the amount left to be payed in Panel4
	public void ShortenDebt3()
	{
		if(_FinanceManager.player_money >= int.Parse(ShortenDebt[0].text))
		{
			//has to be > 1000 to deactivate DebtAdded for the specific panel
			if(int.Parse(LeftToBePayed[2].text) > 1000)
			{
				_FinanceManager.listofdebts[2].left_tb_payed -= int.Parse(ShortenDebt[4].text);
				UpdateOriginalDebtSum();
			}
			else
			{
				ShortenDebtCheck(2, 4);
				ShortenDebtCheck();
			}

			DeductPlayerMoney(4);
		}
	}
}