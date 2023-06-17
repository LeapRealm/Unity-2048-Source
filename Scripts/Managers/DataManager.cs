using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DataManager
{
	public Language language = Language.English;
	private Dictionary<int, TextModel> textModels = new Dictionary<int, TextModel>();

	public void Init()
	{
		switch (Application.systemLanguage)
		{
			case SystemLanguage.Korean:
				language = Language.Korean;
				break;
			case SystemLanguage.Japanese:
				language = Language.Japanese;
				break;
			default:
				language = Language.English;
				break;
		}

		TextTable textTable = Managers.Resource.Load<TextTable>("Data/TextTable");
		foreach (TextTable.Param param in textTable.paramList)
		{
			textModels.Add(param.id, new TextModel(
				param.id, param.texts
			));
		}
	}
	
	public string GetText(TextID id)
	{
		if (textModels.TryGetValue((int)id, out TextModel model))
			return model.texts[(int)language];
		return "";
	}

	public void Clear()
	{
		textModels.Clear();
	}
}