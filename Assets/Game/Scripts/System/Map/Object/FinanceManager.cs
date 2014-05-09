using System;
using System.Collections.Generic;

public class Debt
{
	public int left_tb_payed{get;set;}									//variable to keep track of amount left to be payed per debt
	public int monthly_cut{get;set;}										//variable to keep track of monthly cut per debt (default value = 1000)
	public float interest_percent{get;set;}									//variable to keep track of interest percentage upon taking up a new debt
	public int interest{get;set;}											//variable to keep track of interest value per debt
	public int debt_payment{get;set;}										//variable to keep track of debt payment per debt

	public bool active{get;set;}											//variable to keep track of debt's active state
	public float original_debt_sum{get;set;}								//variable to keep track of the Player's original debt sum

	public Debt()															//constructor that initializes variables
	{
		left_tb_payed = 0;
		monthly_cut = 0;

		active = false;
		original_debt_sum = 0.0f;
	}

	public void SetStartingDepth(int sum){
		left_tb_payed=sum;
	}

	//function that calculates interest per debt
	//called each time a new debt is taken
	public void CalcInterest()
	{
		interest = (int)(interest_percent * left_tb_payed);
	}
	
	//function to calculate debt payment per debt
	//called each time a new debt is taken but after all the necessary calculations have been performed
	//ie calculate interest percent and interest
	public void CalcDebtPayment()
	{
		debt_payment = monthly_cut + interest;
	}
}

public class FinanceManager
{
	public PlayerObjData Player{get;set;}									//access the Player's data
	public List<Debt> listofdebts{get;set;}									//store all the debts taken
	
	public int days_till_update{get;set;}									//variable to keep track of the number of days until the next update (default value = 30)
	private int debt_max;													//variable for the maximum number of debts the Player can take
	
	public int player_money{get{return Player.Money;} set{Player.Money=value;}}										//variable to keep track of the amount of money the player has at the given moment
	public int payment_total{get;set;}									//variable to store the sum of all the debts
	public int existing_cash{get;set;}									//variable to keep track the resultant amount after minusing payment total from player money
		
	float default_percent;													//variable contributing to calculating interest percentage
	float increase_percent;													//variable contributing to calculating interest percentage

	public int month{get;set;}												//variable to keep track of months (required in the calculation of interest percent)
	
	public FinanceManager(){}													//empty constructor (needs to be introduced due to presence of public properties in the class)

	public FinanceManager (PlayerObjData player)							//constructor that initializes variables, takes in a PlayerObjData
	{
		Player = player;
		listofdebts = new List<Debt>();
		
		days_till_update = 30;
		debt_max = 3;
		
		payment_total = 0;
				
		default_percent = 0.05f;
		increase_percent = 0.05f;

		//adding debt_max amount of Debts to the list, limit to 3
		for(int i = 0; i < debt_max; i++)
		{
			listofdebts.Add(new Debt());
		}

		month = 1;
	}
	
	//function that calculates the Player's existing cash
	private void CalcExistingCash()
	{
		existing_cash = (int)(player_money - payment_total);
	}
	
	//function to calculate the new interest percentage each time Player takes on a new debt
	private void CalcInterestPercent()
	{				
		//as long as there is a debt, calculate interest percentage for the particular debt
		for(int i = 0; i < listofdebts.Count; i++)
		{
			if(listofdebts[i] != null)
			{
				listofdebts[i].interest_percent = (increase_percent * month) + default_percent;
			}
		}
	}

	//function to calculate the monthly cut for each debt
	private void CalcMonthlyCut()
	{
		for(int i = 0; i < listofdebts.Count; i++)
		{
			listofdebts[i].monthly_cut = (int)(listofdebts[i].original_debt_sum * 0.05f);
		}
	}
	
	//function to calculate the sum of all debt payments upon taking up new debt
	private void CalcPaymentTotal(bool traverse)
	{
		//as long as ther is debt
		if(listofdebts.Count > 0)
		{
			if(!traverse)
			{
				//add the last debt's payment value to the payment total
				payment_total += listofdebts[listofdebts.Count - 1].debt_payment;
			}
			else
			{
				//traverse through the debt payments of each debt and add it to payment total
				for(int i = 0; i < listofdebts.Count; i++)
				{
					payment_total += listofdebts[i].debt_payment;
				}
			}
		}
	}

	public void CalcAll()
	{
		CalcMonthlyCut();
		CalcInterestPercent();
		for(int i = 0; i < listofdebts.Count; i++)
		{
			listofdebts[i].CalcInterest();
			listofdebts[i].CalcDebtPayment();
		}
		payment_total = 0;
		CalcPaymentTotal(true);
		CalcExistingCash();
	}
	
	//function to update the number of days until the next update
	public void UpdateDays()
	{
		//once, number of days are used up for the month, increase month value, update the necessary values and reset the value for the number of days
		if(days_till_update == 0)
		{
			month++;
			days_till_update = 30;
		}
		else if(days_till_update < 0)
		{
			month++;
			days_till_update = 30 + days_till_update;
			CalcExistingCash();
			Player.Money=existing_cash;
		}
	}

//	//function to update values upon click of shorten debt button and upon update of month
//	public void UpdateValues()
//	{
//		//update interest percents, interests, debt payments of each debt and peyment total
//		CalcInterestPercent();
//		
//		for(int i = 0; i < listofdebts.Count; i++)
//		{
//			listofdebts[i].CalcInterest();
//			listofdebts[i].CalcDebtPayment();
//		}
//
//		//reset payment total and recalculate value of payment total
//		payment_total = 0;
//		CalcPaymentTotal(true);
//		CalcExistingCash();
//	}
	
	//function for Player to take on a new debt
	//assign new debt into specific index
	//set debt to active
	public void AddDebt(int index)
	{
		listofdebts[index] = new Debt();
		listofdebts[index].active = true;
	}

	public void AddDebt(int index,int amount)
	{
		AddDebt(index);
		GetDebt(index).SetStartingDepth(amount);
		listofdebts[index].original_debt_sum = amount;
	}

	public Debt GetDebt(int index)
	{
		return listofdebts[index];
	}
}