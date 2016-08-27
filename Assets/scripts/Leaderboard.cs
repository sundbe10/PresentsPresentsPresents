using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Leaderboard: Singleton<Leaderboard> {

	[System.Serializable]
	public class Leader{
		public string name;
		public int score;
		public string characterSpriteSheet;

		public Leader(string _name, int _score, string _characterSpriteSheet){
			name = _name;
			score = _score;
			characterSpriteSheet = _characterSpriteSheet;
		}
	}
		
	public Leader[] leaderboard;

	// Use this for initialization
	void Awake () {
		Load();
		if(Instance.leaderboard == null){
			leaderboard = new Leader[8];
			for(int i=0; i < leaderboard.Length; i++){
				leaderboard[i] = new Leader("---",0,"santa");
			}
			Save();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	static public bool IsNewHighScore(int score){
		foreach(Leader leader in Instance.leaderboard){
			if(score > leader.score){
				return true;
			}
		}
		return false;
	}

	static public void SetHighScore(Leader newLeader){
		Leader[] newLeaderboard = new Leader[Instance.leaderboard.Length];
		bool inserted = false;
		int counter = 0;
		foreach(Leader leader in Instance.leaderboard){
			if(newLeader.score > leader.score && !inserted){
				newLeaderboard[counter] = newLeader;
				inserted = true;
				counter++;
			}
			if(counter < Instance.leaderboard.Length) newLeaderboard[counter] = leader;
			counter++;
		}
		Instance.leaderboard = newLeaderboard;
		Instance.Save();
	}

	static public Leader[] GetLeaderboard(){
		return Instance.leaderboard;
	}

	void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/leaderboard.gd");
		bf.Serialize(file, Instance.leaderboard);
		file.Close();
	}

	void Load(){
		if(File.Exists(Application.persistentDataPath + "/leaderboard.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/leaderboard.gd", FileMode.Open);
			Instance.leaderboard = (Leader[])bf.Deserialize(file);
			file.Close();
		}
	}
}
