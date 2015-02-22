using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public class Listbox {

	int upperUnvisibleItems;
	int lowerUnvisibleItems;
	public string listName;
	public List<ListItem> items;
	public Rect boundryBox;
	public int itemsMaxHeight;
	public int visibleItemCount;
	int itemCapacity;
	int selectedItemId;

	public Listbox(string[] stringItems, 
	               string ListName,
	               Rect BoundryBox,
	               int itemsMaxHeight)
	{

		this.itemsMaxHeight = itemsMaxHeight;
		this.boundryBox = BoundryBox;
		this.items = new List<ListItem> ();
		visibleItemCount = (int)BoundryBox.height / itemsMaxHeight;
		Additem (stringItems);
		itemCapacity = (int)(boundryBox.height / itemsMaxHeight);


		upperUnvisibleItems = 0;
		lowerUnvisibleItems = items.Count - itemCapacity;
		//Debug.Log (upperUnvisibleItems + "  " + lowerUnvisibleItems);
		this.listName = ListName;
		if (items.Count > 0) {
						items [0].ToggleSelection ();
						selectedItemId = 0;
				}
		}
	public void Additem(string[] items)
	{

		this.items.RemoveAll (x => {return x.content[0].text == "None";});
		foreach (string item in items) {
			this.items.Add(new ListItem(new GUIContent(item),
			                            new Rect( boundryBox.xMin,
			         boundryBox.yMin + (itemsMaxHeight * ( this.items.Count )),
			         boundryBox.width,itemsMaxHeight)
			                            ,( this.items.Count)
			                            ,this));

		}
		lowerUnvisibleItems = this.items.Count - itemCapacity;
		}
	public void DeleteItem(string[] items)
	{
		foreach (var item in items) {
			this.items.RemoveAll(x =>{ return x.content[0].text == item;});
				}
		upperUnvisibleItems = 0;
		lowerUnvisibleItems = this.items.Count - itemCapacity;
		if(lowerUnvisibleItems < 0)
			lowerUnvisibleItems = 0;
		//Debug.Log (this.items.Count);
	}
	public void DrawAll()
	{
				GUI.Box (boundryBox, new GUIContent ("", listName));

				for (int i = upperUnvisibleItems; 
		    		 i < (itemCapacity < items.Count ? itemCapacity : items.Count) + upperUnvisibleItems;
		     		 i++) {
					if(items[i] != null)	
						items [i].Draw ();
					
				}
		}

	public void Move(bool isUp)
	{
		foreach (var item in items) {
			if(isUp && upperUnvisibleItems > 0)

				item.MoveUp();
			else if(!isUp && lowerUnvisibleItems >0)
				item.MoveDown();
				}

		if (!isUp) {
			upperUnvisibleItems++;
			lowerUnvisibleItems--;
			if(lowerUnvisibleItems <0)
			{
				lowerUnvisibleItems++;
				upperUnvisibleItems--;
			}
				} else {

			upperUnvisibleItems--;
			lowerUnvisibleItems++;
			if(upperUnvisibleItems <0)
			{
				lowerUnvisibleItems--;
				upperUnvisibleItems++;
			}
				}
		//Debug.Log (upperUnvisibleItems + "  " + lowerUnvisibleItems);
			DrawAll ();
	}
	public void onClick ()
	{
		foreach (var item in items) {
			item.OnClick();
				}
	}

	public void SelectItem(int id)
	{
		selectedItemId = id;
		foreach (var item in items) {
			if(item.id == id)
				item.ToggleSelection(true);

			else
				item.ToggleSelection(false);
				}
		Debug.Log (getSelectedItemContent ());
	}

	public void SelectItem(string name)
	{

		foreach (var item in items) {
			if(item.content[0].text == name)
			{
				selectedItemId = item.id;
				item.ToggleSelection(true);
			}
			else
				item.ToggleSelection(false);
		}
	}
	public void EventHandler()
	{
		OnNavigationButtonClick ();
		onClick ();

	}


	public void OnNavigationButtonClick()
	{
		if (GUI.Button (new Rect (boundryBox.xMin,boundryBox.yMax,boundryBox.width,20), "Down")) {
			Move(false);
        }
		if (GUI.Button (new Rect (boundryBox.xMin,boundryBox.yMin - 22,boundryBox.width,20), "up")) {	
			Move(true);
		}
		DrawAll ();
	}

	public string getSelectedItemContent()
	{
		//Debug.Log ("Selected Item Id: " + selectedItemId.ToString ());
		//Debug.Log ("Selected Item Content: " + items.Find (x => {
			//			return x.id == selectedItemId;}).content [0].text);

		return items.Find (x => {
						return x.id == selectedItemId;}).content[0].text;
	}
}
 public class ListItem
{
	public Listbox parent;
	public int id;
	public List<GUIContent> content; 
	public Rect boundryBox;
	public bool isSelected;
	public ListClickAction action;
	public void Draw()	
	{

		//GUI.BeginGroup (boundryBox);
		int i = 0;
		foreach (GUIContent item in content) {
			GUI.Box(new Rect(boundryBox.xMin,
			                 boundryBox.yMin + (boundryBox.height * i),
			                 boundryBox.width,
			                 boundryBox.height)
			        ,item);
			i++;
				}
		//GUI.EndGroup ();
	}
	public void MoveUp()
	{
		boundryBox = new Rect (boundryBox.xMin,
		         boundryBox.yMin + 20,
		         boundryBox.width,
		         boundryBox.height);
			Draw();
		}
	public void MoveDown()
	{
		boundryBox = new Rect (boundryBox.xMin,
		          boundryBox.yMin - 20,
		          boundryBox.width,
		          boundryBox.height);
		Draw();
	}
	public ListItem(GUIContent GuiContent,
	                Rect BoundryBox , 
	                int id,
	                Listbox parent)
	{
		this.boundryBox = BoundryBox;
		this.content = new List<GUIContent>();
		this.content.Add (GuiContent);
		this.id = id;
		this.parent = parent;
		isSelected = true;
		ToggleSelection ();

	}

	public void OnClick()
	{
		if (GUI.Button (boundryBox, "", GUIStyle.none)) {
						parent.SelectItem (id);
					if(action != null)
				action.Invoke(content[0].text);
		}
	}

	public void ToggleSelection()
	{
		this.isSelected = !this.isSelected;
		if (this.isSelected) 
			content[0] = new GUIContent(content[0].text,Resources.Load<Texture>("Selected"));
			else
			content[0] = new GUIContent(content[0].text,Resources.Load<Texture>("NotSelected"));


	}

	public void ToggleSelection(bool isSelected)
	{
		this.isSelected = isSelected;
		if (isSelected) 
			content[0] = new GUIContent(content[0].text,Resources.Load<Texture>("Selected"));
		else
			content[0] = new GUIContent(content[0].text,Resources.Load<Texture>("NotSelected"));
		
		
	}

}