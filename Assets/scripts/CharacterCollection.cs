using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterCollection : MonoBehaviour {

	[System.Serializable]
	public class CharacterModel{
		public string identifierName;
		public string displayName;
		public AudioClip taunt;
		public Costume[] costumes;
		public bool locked;
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
		public bool locked;
		public Character(CharacterModel characterModel, Costume costume){
			identifierName = characterModel.identifierName;
			displayName = characterModel.displayName;
			taunt = characterModel.taunt;
			locked = characterModel.locked;
			characterSpriteSheetName = costume.characterSpriteSheetName;
		}
	}

	public CharacterModel[] characterModels;
	public Costume[] playerChoices;

	// Use this for initialization
	void Start () {
		playerChoices = new Costume[4];
	}
		
	public Character GetFirstOpenCharacter(int playerNumber){
		foreach(CharacterModel characterModel in characterModels){
			//If no costumes in a character are taken, assign the player the first costume for the given character
			if(!AnyCostumesTaken(characterModel)){
				SelectCharacter(playerNumber, characterModel.costumes[0]);
				return new Character(characterModel, characterModel.costumes[0]);
			}
		}
		return null;
	} 

	public void SelectCharacter(int playerNumber, Costume costume){
		costume.taken = true;
		playerChoices[playerNumber-1] = costume;
	}

	public void DeselectCharacter(int playerNumber){
		playerChoices[playerNumber-1].taken = false;
		playerChoices[playerNumber-1] = null;
	}

	public Character GetNextOpenCharacter(int playerNumber){
		int characterPosition = GetCharacterModelIndex(playerChoices[playerNumber-1]);
		playerChoices[playerNumber-1].taken = false;

		if(characterPosition == characterModels.Length-1){
			characterPosition = 0;
		}else{
			characterPosition++;
		}

		Costume nextCharacterCostume = GetFirstOpenCostume(characterModels[characterPosition].costumes);
		SelectCharacter(playerNumber, nextCharacterCostume);

		return new Character(characterModels[characterPosition], nextCharacterCostume);
	}

	public Character GetPreviousOpenCharacter(int playerNumber){
		int characterPosition = GetCharacterModelIndex(playerChoices[playerNumber-1]);
		playerChoices[playerNumber-1].taken = false;

		if(characterPosition == 0){
			characterPosition = characterModels.Length-1;
		}else{
			characterPosition--;
		}

		Costume prevCharacterCostume = GetFirstOpenCostume(characterModels[characterPosition].costumes);
		SelectCharacter(playerNumber, prevCharacterCostume);

		return new Character(characterModels[characterPosition], prevCharacterCostume);
	}

	public Character GetNextOpenCostume(int playerNumber){
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

	public Character GetPreviousOpenCostume(int playerNumber){
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

	private Costume GetFirstOpenCostume(Costume[] costumes){
		foreach(Costume _costume in costumes){
			if(_costume.taken == false) return _costume;
		}
		return null;
	}

	private int GetCharacterModelIndex(Costume costume){
		int i = 0;
		foreach(CharacterModel _characterModel in characterModels){
			foreach(Costume _costume in _characterModel.costumes){
				if(_costume == costume) return i;
			}
			i++;
		}
		return 0;
	}

	private int GetCostumeIndex(Costume costume){
		foreach(CharacterModel _characterModel in characterModels){
			int i = 0;
			foreach(Costume _costume in _characterModel.costumes){
				if(_costume == costume) return i;
				i++;
			}
		}
		return 0;
	}

	private bool AnyCostumesTaken(CharacterModel characterModel){
		foreach(Costume _costume in characterModel.costumes){
			if(_costume.taken) return true;
		}
		return false;
	}

}
