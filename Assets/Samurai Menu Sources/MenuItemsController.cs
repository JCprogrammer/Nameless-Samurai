using UnityEngine;
using System.Collections;

public class MenuItemsController : MonoBehaviour {
	private int _selectedIndex = 0;
	public int selectedIndex
	{
		get
		{
			return _selectedIndex;
		}

		set
		{
			_selectedIndex = value;

			_selectedIndex = Mathf.Clamp(_selectedIndex, 0, 1);

			foreach(MenuItemController item in GetComponentsInChildren<MenuItemController>())
			{
				if(item.menuItemIndex == _selectedIndex)
					item.guiText.color = Color.yellow;
				else
					item.guiText.color = Color.white;
			}
		}
	}
	// Use this for initialization
	void Start () {
		selectedIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.DownArrow))
			selectedIndex = 1;

		if(Input.GetKeyDown(KeyCode.UpArrow))
			selectedIndex = 0;

		if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
			Select ();
	
	}

	public void Select()
	{
		switch(selectedIndex)
		{
		case 0:
			//Application.LoadLevel("FirstLevel");
			FindObjectOfType<MenuController>().StartGame();
			break;
		case 1:
			Application.Quit();
			break;
		}
	}
}
