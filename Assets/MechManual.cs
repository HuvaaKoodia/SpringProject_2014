using UnityEngine;
using System.Collections;

public class MechManual : MonoBehaviour 
{
	public MenuTabController tabs;
	public GameObject nextButton, previousButton;
	public UILabel PageCounter;

	int currentPage;

	void Start () 
	{
		tabs.CloseOtherMenus(null);
		currentPage = 0;
		tabs.ActivateMenu(currentPage);
		UpdateButtonState();
	}

	public void OnNextPressed()
	{
		++currentPage;
		tabs.ActivateMenu(currentPage);
		UpdateButtonState();
	}
	
	public void OnPrevPressed()
	{
		--currentPage;
		tabs.ActivateMenu(currentPage);
		UpdateButtonState();
	}

	//private

	void UpdateButtonState()
	{
		int count = tabs.TabMenus.Count;

		currentPage = Mathf.Clamp(currentPage, 0, count - 1);

		previousButton.SetActive(currentPage != 0);
		nextButton.SetActive(currentPage != count - 1);

		PageCounter.text = (currentPage + 1) + "/" + count;
	}
}