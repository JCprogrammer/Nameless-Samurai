using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public class CubeFile {


	public static void SerializeObject<T>(string filename, T objectToSerialize,string subFolder = "")
	{
		Stream stream = File.Open( "SavedFiles/"+ subFolder + filename, FileMode.Create);
		BinaryFormatter bFormatter = new BinaryFormatter();
		bFormatter.Serialize(stream, objectToSerialize);
		stream.Close();
	}
	
	public static T DeSerializeObject<T>(string filename,string subFolder = "")
	{
		try {
			T objectToSerialize;
			Stream stream = File.Open("SavedFiles/" + subFolder + filename, FileMode.Open);

			BinaryFormatter bFormatter = new BinaryFormatter();
			objectToSerialize = (T)bFormatter.Deserialize(stream);
			stream.Close();
			return objectToSerialize;


		}
		catch (System.Exception ex) {
			Debug.Log("Failed Loading File. Error Message: " + ex.ToString());	

				}
		return default(T);
	}

	public static void DeleteFile(string fileName, string subFolder = "")
	{
		File.Delete ("SavedFiles/" + subFolder + fileName);
	}
	public static void DeleteScene(string sceneName)
	{
		File.Delete ("Assets/Levels/" + sceneName+ ".unity");
	}
	public static string[] CheckDirectory(string directory)
	{
		string[] tmpItems = Directory.GetFiles (directory);
		for (int i = 0; i < tmpItems.Length; i++) {
			tmpItems[i] = tmpItems[i].Remove(0,directory.Length);


				}
			
		return tmpItems;
	}
}
