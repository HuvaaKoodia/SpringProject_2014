using UnityEngine;
using System.Collections;

//=============================================================
//THIS SCRIPT IS USED ONLY FOR TESTING OF FINANCEMANAGER SCRIPT
//=============================================================

public class TestFinance : MonoBehaviour {
	
	FinanceManager PlayerFinance;
	
	// Use this for initialization
	void Start () {
		PlayerFinance = new FinanceManager();
		
		//add the 1st debt
		PlayerFinance.AddDebt(0);
		Debug.Log("List of debts: " + PlayerFinance.listofdebts.Count);
		PlayerFinance.listofdebts[0].left_tb_payed = 14000;
		
		//calculate the interest percentage upon taking on a new debt
		PlayerFinance.CalcInterestPercent();
		Debug.Log("Debt interest percent: " + PlayerFinance.listofdebts[PlayerFinance.listofdebts.Count - 1].interest_percent);
		
		//add the 2nd debt
		PlayerFinance.AddDebt(1);
		Debug.Log("List of debts: " + PlayerFinance.listofdebts.Count);
		PlayerFinance.listofdebts[1].left_tb_payed = 6000;
		
		//calculate the interest percentage upon taking on a new debt
		PlayerFinance.CalcInterestPercent();
		Debug.Log("Debt interest percent: " + PlayerFinance.listofdebts[PlayerFinance.listofdebts.Count - 1].interest_percent);
	}
	
	// Update is called once per frame
	void Update () {
		
		for(int i = 0; i < PlayerFinance.listofdebts.Count; i++)
		{
			PlayerFinance.listofdebts[i].CalcInterest();
			Debug.Log("Debt interest[" + i + "]: " + PlayerFinance.listofdebts[i].interest);
			PlayerFinance.listofdebts[i].CalcDebtPayment();
			Debug.Log("Debt payment[" + i + "]: " + PlayerFinance.listofdebts[i].debt_payment);
		}
		
		PlayerFinance.CalcPaymentTotal(false);
		Debug.Log("Payment total: " + PlayerFinance.payment_total);
		
		PlayerFinance.CalcExistingCash();
		Debug.Log("Existing cash: " + PlayerFinance.existing_cash);

		Debug.Log("Player money: " + PlayerFinance.player_money);
		
		Debug.Log("Days till next update: " + PlayerFinance.days_till_update);
		
		PlayerFinance.UpdateDays();
		Debug.Log("Days till next update: " + PlayerFinance.days_till_update);
		
		Debug.Break();
	}
}
