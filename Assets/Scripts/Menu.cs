using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//uzywamy
using UnityEngine.SceneManagement;//uzywamy

public class Menu : MonoBehaviour {

	void Start () {
		
	}



	public void klikniecie()//jezeli nacisne na mysz to wlaczy sie gra
	{
		SceneManager.LoadScene("Level1");
	}
	// Update is called once per frame
	void Update () {//jesli nacisne enter to wlaczy mi sie gra
		if (Input.GetKeyUp (KeyCode.Return)) {
			SceneManager.LoadScene("Level1");
		} 
	}
}
