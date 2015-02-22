using UnityEngine;
using System.Collections;

public class MenuItemController : MonoBehaviour {
	public int menuItemIndex;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		//GetComponentInParent<MenuItemsController>().selectedIndex = menuItemIndex;
		//GetComponentInParent<MenuItemsController>().Select();
	}

	void OnMouseEnter()
	{
	}
}
