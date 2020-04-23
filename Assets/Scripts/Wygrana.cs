using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//uzywamy
using UnityEngine.SceneManagement;//uzywamy
public class Wygrana : MonoBehaviour {
	public void klikniecie()//jezeli nacisne na mysz to wlaczy sie gra
	{
		SceneManager.LoadScene("Level1");
	}
	public void klikniecie2()
	{
		Application.Quit ();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
