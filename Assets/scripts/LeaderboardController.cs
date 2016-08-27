using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour {

	public GameObject leaderObject;

	// Use this for initialization
	void Start () {
		int counter = 1;
		Leaderboard.Leader[] leaderboard = Leaderboard.GetLeaderboard();
		foreach(Leaderboard.Leader leader in leaderboard){
			if(counter < 7){
				//Create leader object
				GameObject _leaderObject = Instantiate(leaderObject, new Vector3(0,70+(counter-1)*-40,0), Quaternion.identity) as GameObject;
				_leaderObject.transform.SetParent(transform);
				//Set visible elements for leader
				GameObject score = _leaderObject.transform.Find("Score").gameObject;
				GameObject name = _leaderObject.transform.Find("Name").gameObject;
				score.GetComponent<Text>().text = leader.score.ToString();
				name.GetComponent<Text>().text = counter+". "+leader.name;
				if(leader.characterSpriteSheet != null) _leaderObject.GetComponentInChildren<SpriteSwitch>().SetSpriteSheet(leader.characterSpriteSheet);
				//Increase counter for leaderboard
				counter++;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
