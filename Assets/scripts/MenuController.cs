using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	int currentMenuItem = 0;
	GameObject[] menuItems;
	int numberOfMenuItems;

	// Use this for initialization
	void Start () {
		menuItems = GameObject.FindGameObjectsWithTag("Menu Item");
		numberOfMenuItems = menuItems.Length - 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Vertical")){
			currentMenuItem += Input.GetAxis ("Vertical") > 0 ? 1 : -1;
			if(currentMenuItem > numberOfMenuItems){
				currentMenuItem = 0;
			}else if(currentMenuItem < 0){
				currentMenuItem = numberOfMenuItems;
			}
			//Select the menu item

		}
	}
}
