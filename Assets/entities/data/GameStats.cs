using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class GameStats : Singleton<GameStats> {

	public Dictionary<Stat, float> playerStats;

	public enum Stat{
		HighScore,
		TotalScore,
		KidsCaught,
		BulliesCaught,
		PresentsThrown,
		CoalThrown,
		PowerupsUsed,
		GamesPlayed
		//Accuracy
	}

	[System.Serializable]
	public class CurrentStats{
		public float TotalScore;
		public float KidsCaught;
		public float BulliesCaught;
		public float PresentsThrown;
		public float CoalThrown;
		public float PowerupsUsed;
		public float GamesPlayed;
	}

	public CurrentStats currentStats;

	// Use this for initialization
	void Start () {
		LoadStats();
		if(playerStats == null){
			playerStats = new Dictionary<Stat,float>(){
				{Stat.TotalScore, 0},
				{Stat.KidsCaught, 0},
				{Stat.BulliesCaught, 0},
				{Stat.PresentsThrown, 0},
				{Stat.CoalThrown, 0},
				{Stat.PowerupsUsed, 0},
				{Stat.GamesPlayed, 0}
			};
			SaveStats();
		}
	}
	
	// Update is called once per frame
	void Update () {
		//This is just for debugging in the inspector
		currentStats.TotalScore = playerStats[Stat.TotalScore];
		currentStats.KidsCaught = playerStats[Stat.KidsCaught];
		currentStats.BulliesCaught = playerStats[Stat.BulliesCaught];
		currentStats.PresentsThrown = playerStats[Stat.PresentsThrown];
		currentStats.CoalThrown = playerStats[Stat.CoalThrown];
		currentStats.PowerupsUsed = playerStats[Stat.PowerupsUsed];
		currentStats.GamesPlayed = playerStats[Stat.GamesPlayed];
	}

	static public void IncrementStat(Stat stat){
		Instance.playerStats[stat]++;
	}
	static public void IncrementStat(Stat stat, float value){
		Instance.playerStats[stat] += value;
	}
	static public float GetStat(Stat stat){
		if(stat == Stat.HighScore){
			return Leaderboard.GetLeaderboard()[0].score;
		}else{
			return Instance.playerStats[stat];
		}
	}

	//Save/Load stats
	static public void SaveStats(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/stats.gd");
		bf.Serialize(file, Instance.playerStats);
		file.Close();
	}

	static public CharacterCollection.CharacterModel GetUnlockedCharacter(){
		foreach(CharacterCollection.CharacterModel character in CharacterCollection.GetAllCharacters()){
			if(character.locked == true && GetStat(character.unlockKey) >= character.unlockValue){
				Debug.Log(character.displayName+" unlocked!");
				return character;
			}
		}
		return null;
	}

	void LoadStats(){
		if(File.Exists(Application.persistentDataPath + "/stats.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/stats.gd", FileMode.Open);
			Instance.playerStats = (Dictionary<Stat,float>)bf.Deserialize(file);
			file.Close();
		}
	}
		
}
