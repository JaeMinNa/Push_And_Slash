using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("PlayerData", "GameData", "DataWrapper")]
	public class ES3UserType_DataManager : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DataManager() : base(typeof(DataManager)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (DataManager)obj;
			
			writer.WriteProperty("PlayerData", instance.PlayerData, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(CharacterData)));
			writer.WriteProperty("GameData", instance.GameData, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(GameData)));
			writer.WriteProperty("DataWrapper", instance.DataWrapper, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(DataWrapper)));
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (DataManager)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "PlayerData":
						instance.PlayerData = reader.Read<CharacterData>();
						break;
					case "GameData":
						instance.GameData = reader.Read<GameData>();
						break;
					case "DataWrapper":
						instance.DataWrapper = reader.Read<DataWrapper>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_DataManagerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DataManagerArray() : base(typeof(DataManager[]), ES3UserType_DataManager.Instance)
		{
			Instance = this;
		}
	}
}