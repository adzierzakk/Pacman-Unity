using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plansza : MonoBehaviour {
	private static int szerokoscPlanszy=28;
	private static int wysokoscPlanszy=36;
	public int iloscZiaren=0;
	public int wynik = 0;
	public new AudioSource audio;//bez slowa new wyrzucalo warning
	public bool pauza = false;//flaga do pauzowania gry

	public GameObject[,] plansza = new GameObject [szerokoscPlanszy, wysokoscPlanszy];//tworze tablice dwuwymiarowa
	// Use this for initialization
	void Start () {
		Object[] obiekty = GameObject.FindObjectsOfType (typeof(GameObject));//kazda klasa dziedziczy z klasy Object(nie znamy typu)
		foreach (GameObject o in obiekty) {//petla foreach sluzy do poruszania sie po tablicy
			Vector2 pos = o.transform.position;
			if (o.name != "Pacman"&&o.name!="Wezly"&&o.name!="Reszta"&&o.name!="Labirynt"&&o.name!="Ziarenka"&&o.tag!="Duch"&&o.tag!="ObszaryDuszkow"&&o.name!="Canvas"&&o.name!="Rezultat") {//jezeli nie dokonamy tego to gra moze nie dzialac
				if (o.GetComponent<Pytania> () != null) {
					if (o.GetComponent<Pytania> ().czyToZiarno || o.GetComponent<Pytania> ().czyToDuzeZiarno) {
						iloscZiaren++;//zapisuje ilosc ziaren w inspektorze
					}
				}
				plansza [(int)pos.x, (int)pos.y] = o;//do ustalenia pozycji na planszy
			} else {
				Debug.Log ("Znaleziono Pacmana: " + pos);//wczesniej stosowalem zeby ustalic pozycje pacmana
			}
		}//tutaj koncze petle
		audio = GetComponent<AudioSource>();

	}
	public void Restart(){//upup
		GameObject pacMan = GameObject.Find ("Pacman");//szuka obiektu o nazwie Pacman
		pacMan.transform.GetComponent<Pacman> ().Restart ();//Ustawia pozycje Pacmana na poczatkowa
		GameObject[] o = GameObject.FindGameObjectsWithTag ("Duch");//szuka duchow
		foreach (GameObject duch in o) {//petla foreach sluzy do znalezienia kazdego ducha i zmiany pozycji na poczatkowa
			duch.transform.GetComponent<Duch> ().Restart ();
		}
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {//naciskamy spacje zeby wlaczyc lub wylaczyc dzwiek
			audio.mute = !audio.mute;
		} else if (Input.GetKeyDown (KeyCode.P)) {//naciskamy spacje zeby wlaczyc lub wylaczyc dzwiek
			if (!pauza) {
				Time.timeScale = 0;
				pauza = true;
			} else {
				Time.timeScale = 1;
				pauza = false;
			}
		}
	}
}
