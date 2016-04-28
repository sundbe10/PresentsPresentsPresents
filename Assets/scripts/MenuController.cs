using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	[System.Serializable]
	public class MenuItem{
		public string displayText;
		public string sceneToLoad;
	}
	public MenuItem[] menuItems;
	public GameObject menuItemObject;

	int currentMenuItem = 0;
	GameObject[] menuObjects;

	// Use this for initialization
	void Start () {
		menuObjects = new GameObject[menuItems.Length];
		for(int i = 0; i < menuItems.Length; i++){
			GameObject newMenuItem = Instantiate(menuItemObject, Vector3.zero, Quaternion.identity) as GameObject;
			newMenuItem.transform.parent = transform;
			newMenuItem.transform.position = new Vector3(0,-16f,0);
			newMenuItem.GetComponent<Text>().text = menuItems[i].displayText;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Vertical")){
			currentMenuItem += Input.GetAxis ("Vertical") > 0 ? 1 : -1;
			if(currentMenuItem > menuItems.Length-1){
				currentMenuItem = 0;
			}else if(currentMenuItem < 0){
				currentMenuItem = menuItems.Length-1;
			}
			//Select the menu item

		}
	}
}
