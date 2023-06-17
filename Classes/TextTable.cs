using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class TextTable : ScriptableObject
{	
	public List<Param> paramList = new List<Param>();

	[System.SerializableAttribute]
	public class Param
	{
		public int id;
		public string[] texts;
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("id : " + id + " / ");

			builder.Append("texts : [");
			foreach (string element in texts)
				builder.AppendFormat("{0}, ", element);
			builder.Append("] / ");

			return builder.ToString();
		}
	}
}