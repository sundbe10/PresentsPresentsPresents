using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class CharacterCollection : Singleton<CharacterCollection> {

	[System.Serializable]
	public class CharacterModel{
		public int order;
		public string identifierName;
		public string displayName;
		public AudioClip taunt;
		public Costume[] costumes;
		public bool locked;
		public GameStats.Stat unlockKey;
		public float unlockValue;
		public string unlockMessage;
	}

	[System.Serializable]
	public class Costume{
		public string characterSpriteSheetName;
		public bool taken = false;
	}

	public class Character{
		public string identifierName;
		public string displayName;
		public AudioClip taunt;
		public string characterSpriteSheetName;
		public string unlockMessage;
		public GameStats.Stat unlockKey;
		public float unlockValue;
		public bool locked;
		public Character(CharacterModel characterModel, Costume costume){
			identifierName = characterModel.identifierName;
			displayName = characterModel.displayName;
			taunt = characterModel.taunt;
			locked = characterModel.locked;
			characterSpriteSheetName = costume.characterSpriteSheetName;
			unlockMessage = characterModel.unlockMessage;
			unlockKey = characterModel.unlockKey;
			unlockValue = characterModel.unlockValue;
		}
	}

	public CharacterModel[] characterModels;
	public Costume[] playerChoices;

	public List<string> unlockedCharacters;

	// Use this for initialization
	void Start () {
		characterModels = characterModels.OrderBy(o=>o.order).ToArray();
		LoadUnlocks();
		InitializePlayerChoices();
	}

	void OnLevelWasLoaded(int level){
		//If navigating to a new scene, reset character collection
		Instance.InitializePlayerChoices();
	}

	//Static
		
	static public Character GetFirstOpenCharacter(int playerNumber){
		foreach(CharacterModel characterModel in Instance.characterModels){
			//If no costumes in a character are taken, assign the player the first costume for the given character
			if(!Instance.AnyCostumesTaken(characterModel)){
				CharacterCollection.SelectCharacter(playerNumber, characterModel.costumes[0]);
				return new Character(characterModel, characterModel.costumes[0]);
			}
		}
		return null;
	} 

	static public void SelectCharacter(int playerNumber, Costume costume){
		costume.taken = true;
		Instance.playerChoices[playerNumber-1] = costume;
	}

	static public void DeselectCharacter(int playerNumber){
		Instance.playerChoices[playerNumber-1].taken = false;
		Instance.playerChoices[playerNumber-1] = null;
	}

	static public Character GetNextOpenCharacter(int playerNumber){
		return Instance.SelectNextOpenCharacter(playerNumber);
	}
		

	static public Character GetPreviousOpenCharacter(int playerNumber){
		return Instance.SelectPreviousOpenCharacter(playerNumber);
	}

	static public Character GetNextOpenCostume(int playerNumber){
		return Instance.SelectNextOpenCostume(playerNumber);
	}

	static public Character GetPreviousOpenCostume(int playerNumber){
		return Instance.SelectPreviousOpenCostume(playerNumber);
	}

	static public CharacterModel[] GetAllCharacters(){
		return Instance.characterModels;
	}

	static public void UnlockCharacter(CharacterModel character){
		Instance.unlockedCharacters.Add(character.identifierName);
		Instance.SetUnlockedCharacters();
		Instance.SaveUnlocks();
	}


	//private
	void InitializePlayerChoices(){
		//TODO: Change functionality so control of selected characters is separate from the library of characters
		Instance.playerChoices = new Costume[4];
		//Reset taken costumes
		foreach(CharacterModel characterModel in characterModels){
			foreach(Costume costume in characterModel.costumes){
				costume.taken = false;
			}
		}
	}

	Character SelectNextOpenCharacter(int playerNumber){
		int characterPosition = GetCharacterModelIndex(playerChoices[playerNumber-1]);
		playerChoices[playerNumber-1].taken = false;

		if(characterPosition == characterModels.Length-1){
			characterPosition = 0;
		}else{
			characterPosition++;
		}

		Costume nextCharacterCostume = GetFirstOpenCostume(characterModels[characterPosition].costumes);
		CharacterCollection.SelectCharacter(playerNumber, nextCharacterCostume);

		return new Character(characterModels[characterPosition], nextCharacterCostume);
	}

	Character SelectPreviousOpenCharacter(int playerNumber){
		int characterPosition = GetCharacterModelIndex(playerChoices[playerNumber-1]);
		playerChoices[playerNumber-1].taken = false;

		if(characterPosition == 0){
			characterPosition = characterModels.Length-1;
		}else{
			characterPosition--;
		}

		Costume prevCharacterCostume = GetFirstOpenCostume(characterModels[characterPosition].costumes);
		CharacterCollection.SelectCharacter(playerNumber, prevCharacterCostume);

		return new Character(characterModels[characterPosition], prevCharacterCostume);
	}

	Character SelectNextOpenCostume(int playerNumber){
		Costume nextCharacterCostume = null;
		int characterPosition = GetCharacterModelIndex(playerChoices[playerNumber-1]);
		int costumePosition = GetCostumeIndex(playerChoices[playerNumber-1]);
		playerChoices[playerNumber-1].taken = false;

		while(nextCharacterCostume == null){
			if(costumePosition == characterModels[characterPosition].costumes.Length-1){
				costumePosition = 0;
			}else{
				costumePosition++;
			}
			if(characterModels[characterPosition].costumes[costumePosition].taken == false){
				nextCharacterCostume = characterModels[characterPosition].costumes[costumePosition];
			}
		}

		SelectCharacter(playerNumber, nextCharacterCostume);

		return new Character(characterModels[characterPosition], nextCharacterCostume);
	}

	Character SelectPreviousOpenCostume(int playerNumber){
		Costume previousCharacterCostume = null;
		int characterPosition = GetCharacterModelIndex(playerChoices[playerNumber-1]);
		int costumePosition = GetCostumeIndex(playerChoices[playerNumber-1]);
		playerChoices[playerNumber-1].taken = false;

		while(previousCharacterCostume == null){
			if(costumePosition == 0){
				costumePosition = characterModels[characterPosition].costumes.Length-1;
			}else{
				costumePosition--;
			}
			if(characterModels[characterPosition].costumes[costumePosition].taken == false){
				previousCharacterCostume = characterModels[characterPosition].costumes[costumePosition];
			}
		}

		SelectCharacter(playerNumber, previousCharacterCostume);

		return new Character(characterModels[characterPosition], previousCharacterCostume);
	}

	Costume GetFirstOpenCostume(Costume[] costumes){
		foreach(Costume _costume in costumes){
			if(_costume.taken == false) return _costume;
		}
		return null;
	}

	int GetCharacterModelIndex(Costume costume){
		int i = 0;
		foreach(CharacterModel _characterModel in characterModels){
			foreach(Costume _costume in _characterModel.costumes){
				if(_costume == costume) return i;
			}
			i++;
		}
		return 0;
	}

	int GetCostumeIndex(Costume costume){
		foreach(CharacterModel _characterModel in characterModels){
			int i = 0;
			foreach(Costume _costume in _characterModel.costumes){
				if(_costume == costume) return i;
				i++;
			}
		}
		return 0;
	}

	bool AnyCostumesTaken(CharacterModel characterModel){
		foreach(Costume _costume in characterModel.costumes){
			if(_costume.taken) return true;
		}
		return false;
	}

	void SetUnlockedCharacters(){
		foreach(CharacterModel characterModel in Instance.characterModels){
			foreach(string unlockName in unlockedCharacters){
				if(characterModel.identifierName == unlockName) characterModel.locked = false;
			}
		}
	}

	void SaveUnlocks(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/unlocks.gd");
		bf.Serialize(file, Instance.unlockedCharacters);
		file.Close();
	}

	void LoadUnlocks(){
		if(File.Exists(Application.persistentDataPath + "/unlocks.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/unlocks.gd", FileMode.Open);
			Instance.unlockedCharacters = (List<string>)bf.Deserialize(file);
			file.Close();
		}
		SetUnlockedCharacters();
	}

}
