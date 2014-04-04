using UnityEngine;
using System.Collections;

public class FinanceMenu : MonoBehaviour {

	public FinanceManager _FinanceManager;

	public UILabel Days;
	public UILabel PlayerMoney;
	public UILabel Payment1;
	public UILabel Payment2;
	public UILabel Payment3;
	public UILabel PaymentT;
	public UILabel ExistingCash;
	public UILabel ShortD1;
	public UILabel ShortD2;
	public UILabel ShortD3;

	// Use this for initialization
	void Start () {
		_FinanceManager = new FinanceManager();

		InitializeValues();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateValues();
	}

	void InitializeValues()
	{
		Days.text = _FinanceManager.days_till_update.ToString();
		PlayerMoney.text = _FinanceManager.player_money.ToString();
		Payment1.text = "0";
		Payment2.text = "0";
		Payment3.text = "0";
		PaymentT.text = _FinanceManager.payment_total.ToString();
		ExistingCash.text = _FinanceManager.existing_cash.ToString();
		ShortD1.text = _FinanceManager.shorten_debt.ToString();
		ShortD2.text = _FinanceManager.shorten_debt.ToString();
		ShortD3.text = _FinanceManager.shorten_debt.ToString();
	}

	void UpdateValues()
	{
		Days.text = _FinanceManager.days_till_update.ToString();
		PlayerMoney.text = _FinanceManager.player_money.ToString();
		
		if(_FinanceManager.listofdebts.Count > 0)
		{
			Payment1.text = _FinanceManager.listofdebts[0].debt_payment.ToString();
			Payment2.text = _FinanceManager.listofdebts[1].debt_payment.ToString();
			Payment3.text = _FinanceManager.listofdebts[2].debt_payment.ToString();
		}
		
		PaymentT.text = _FinanceManager.payment_total.ToString();
		ExistingCash.text = _FinanceManager.existing_cash.ToString();
	}

//	public void IncreaseShortDebt(int debtnumber)
//	{
//		var sd = _FinanceManager.shorten_debt;
//	
//		if(debtnumber == 1)
//		{
//			int value = int.Parse(ShortD1.text);
//			value += 100;
//			ShortD1.text = value.ToString();
//		}
//		else if(debtnumber == 2)
//		{
//			int value = int.Parse(ShortD2.text);
//			value += 100;
//			ShortD2.text = value.ToString();
//		}
//		else if(debtnumber == 3)
//		{
//			int value = int.Parse(ShortD3.text);
//			value += 100;
//			ShortD3.text = value.ToString();
//		}
//		else
//		{
//			Debug.Log("Key in numbers 1 to 3 only");
//		}
//	}
//	
//	public void DecreaseShortDebt(int debtnumber)
//	{
//		var sd = _FinanceManager.shorten_debt;
//
//		if(debtnumber == 1)
//		{
//			int value = int.Parse(ShortD1.text);
//			value -= 100;
//			ShortD1.text = value.ToString();
//		}
//		else if(debtnumber == 2)
//		{
//			int value = int.Parse(ShortD2.ToString());
//			value -= 100;
//			ShortD2.text = value.ToString();
//		}
//		else if(debtnumber == 3)
//		{
//			int value = int.Parse(ShortD3.text);
//			value -= 100;
//			ShortD3.text = value.ToString();
//		}
//		else
//		{
//			Debug.Log("Key in numbers 1 to 3 only");
//		}
//	}
}
