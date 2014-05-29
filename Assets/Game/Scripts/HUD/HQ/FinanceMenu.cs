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
	public List<UILabel> ShortenDebtList;							//List that keeps track of the Shorten Debt By component for each debt
	public List<GameObject> DebtsActivate;						//List that keeps track of the Debt types ie, DebtAdded and DebtEmpty for each Debt
	public List<GameObject> DebtPanels;

	public FinanceManager _FinanceManager;

	public System.Action OnDebtShorten;

	//function to search for currently active Debts and display in the FinanceScreen
	private void SearchActiveDebt()
	{
		for(int i = 0; i < _FinanceManager.listofdebts.Count; i++)
		{
			bool active=_FinanceManager.listofdebts[i].active;

			DebtsActivate[(2 * i)].SetActive(active);
			DebtsActivate[(2 * i) + 1].SetActive(!active);
			DebtPanels[i].SetActive(active);
		}

		UpdateValues();
	}

	//function to set the instance of FinanceManager to refer to
	public void SetFinanceManager(FinanceManager FM)
	{
		_FinanceManager = FM;
		SearchActiveDebt();
		UpdateValues();
	}

	//UPDATE LABELS FOR FINANCE SCREEN PROPERTIES
	//function to update the value of amount left to be payed per debt
	private void UpdateLeftToBePayed(int i)
	{
		var FM_l_i = _FinanceManager.GetDebt(i);
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
				//_FinanceManager.CalcMonthlyCut();
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

	public void OpenMenu(){
		UpdateValues();
	}

	//function that updates variables
	private void UpdateValues()
	{
		//Days
		Days.text = _FinanceManager.days_till_update.ToString();

		//Player's money
		PlayerMoney.text = _FinanceManager.player_money.ToString();

		//amount left to be payed, monthly cut, interest, debt payments, payment total
		//_FinanceManager.UpdateValues();
		_FinanceManager.CalcAll();

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
		int value1 = int.Parse(ShortenDebtList[sd_index].text);
		value1 += 1000;
		
		//cap for the value
		ShortenDebtList[sd_index].text = value1.ToString();
		if(int.Parse(ShortenDebtList[sd_index].text) >= 50000)
		{
			ShortenDebtList[sd_index].text = "50000";
		}
		
		//value for DebtActive
		int value = int.Parse(ShortenDebtList[sd_index - 1].text);
		int ltbp = int.Parse(LeftToBePayed[ltbp_index].text);
		
		//value can only increment if left_tb_payed value is > 0
		if(ltbp != 0)
		{
			value += 1000;
		}
		
		//cap for the value
		ShortenDebtList[sd_index - 1].text = value.ToString();
		if(int.Parse(ShortenDebtList[sd_index - 1].text) >= ltbp && ltbp != 0)
		{
			ShortenDebtList[sd_index - 1].text = LeftToBePayed[ltbp_index].text;
		}

		UpdateValues();
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
		int value1 = int.Parse(ShortenDebtList[sd_index].text);
		value1 -= 1000;
		
		//cap for the value
		ShortenDebtList[sd_index].text = value1.ToString();
		if(int.Parse(ShortenDebtList[sd_index].text) <= 10000)
		{
			ShortenDebtList[sd_index].text = "10000";
		}
		
		//value for DebtActive
		int value = int.Parse(ShortenDebtList[sd_index - 1].text);
		value -= 1000;
		
		//cap for the value
		ShortenDebtList[sd_index - 1].text = value.ToString();
		if(int.Parse(ShortenDebtList[sd_index - 1].text) <= 1000)
		{
			ShortenDebtList[sd_index - 1].text = "1000";
		}

		UpdateValues();
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


	//function to process the activation and deactivation of specific debts
	private void ActivateDebtProcess(int index_1, int index_2)
	{
		int amount=int.Parse(ShortenDebtList[index_2].text);

		//add a debt
		_FinanceManager.AddDebt(index_1,amount);
		
		//process the necessary values for the current panel
		_FinanceManager.player_money += _FinanceManager.GetDebt(index_1).left_tb_payed;
		_FinanceManager.CalcAll();
		
		//enable DebtAdded and disable DebtEmpty for current panel
		DebtsActivate[index_2 - 1].SetActive(true);
		DebtsActivate[index_2].SetActive(false);
		DebtPanels[index_1].SetActive(true);

		UpdateValues();
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

	//function to deduct the amount of money to shorten the debt by from the Player's money
	private void DeductPlayerMoney(int amount)
	{
		_FinanceManager.player_money -= amount;
	}

	//function to Shorten the amount left to be payed in Panel2
	public void ShortenDebt1()
	{
		ShortenDebt(0);
	}

	//function to Shorten the amount left to be payed in Panel3
	public void ShortenDebt2()
	{
		ShortenDebt(1);
	}

	//function to Shorten the amount left to be payed in Panel4
	public void ShortenDebt3()
	{
		ShortenDebt(2);
	}

	/// <summary>
	/// Shortens the amount of a selected debt
	/// </summary>
	private void ShortenDebt(int index)
	{
		//calculate shorten amount
		var debt=_FinanceManager.GetDebt(index);
		int shorten_amount=int.Parse(ShortenDebtList[index*2].text);

		if(debt.left_tb_payed <shorten_amount)
		{
			shorten_amount=debt.left_tb_payed;
		}

		//spend money
		if(_FinanceManager.player_money >= shorten_amount)
		{
			debt.ShortenAmount(shorten_amount);
			DeductPlayerMoney(shorten_amount);

			//update debts
			SearchActiveDebt();
			UpdateValues();

			if (OnDebtShorten!=null) OnDebtShorten();
		}
	}
}