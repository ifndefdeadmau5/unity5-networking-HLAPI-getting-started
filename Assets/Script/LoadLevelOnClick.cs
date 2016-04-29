using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadLevelOnClick : MonoBehaviour {
	public string LevelName;
	// Use this for initialization
	public void LoadLevel( )
	{
		SceneManager.LoadScene(LevelName);
	}
}
