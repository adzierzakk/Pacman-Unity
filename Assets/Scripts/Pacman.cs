using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;//uzywamy


public class Pacman : MonoBehaviour {
	public Vector2 orientacja;

	public float predkosc=4.0f;
	public Sprite nieruchomyPacman;//bedzie sluzylo do przerwania animacji, gdy pac uderzy w sciane
	private Vector2 kierunek = Vector2.zero;
	private Vector2 kolejnyKierunek;
	private int zjedzoneZiarenka=0;
	private int iloscZyc = 3;
	private Wezly obecnyWezel, poprzedniWezel, wezelDocelowy;
	private Wezly pozycjaPoczatkowa;//upup
	// Use this for initialization
	void Start () {//zachowanie pacmana na poczatku rozgrywki, poczatkowe polozenie, poczatkowy kierunek ruchu
		Wezly wezel = PozycjonowanieWezla (transform.localPosition);//ustawianie poczatkowej pozycji wezla
		pozycjaPoczatkowa = wezel;//upup
		if (wezel != null) {
			obecnyWezel = wezel;
			Debug.Log (obecnyWezel);
		}
		kierunek = Vector2.left;//zakladam poczatkowy kierunek ruchu pacmana w lewo
		orientacja = Vector2.left;
		//ZmianaPozycji (kierunek); //wczesniej tego uzywalem to pacman poruszal sie od razu na lewo, a bez tego moge sobie wybrac kierunek ruchu pacmana
	}

	public void Restart(){//powrot do poczatkowych pozycji po zabiciu pacmana
		iloscZyc--;
		if(iloscZyc==0)
			SceneManager.LoadScene("Przegrana");
		transform.position=pozycjaPoczatkowa.transform.position;//resetuje pozycje pacmana do pozycji startowej
		obecnyWezel=pozycjaPoczatkowa;//ustawiam pierwszy wezel na pozycji pacmana
		kierunek = Vector2.left;//ruszamy w lewo
		orientacja = Vector2.left;//orientacja grafiki pacmana
		kolejnyKierunek = Vector2.left;
		ZmianaPozycji (kierunek);// to musi byc, bo inaczej pacman wraca nie na pozycje startowa tylko na pozycje w ktorej zginal


	}

	// Update is called once per frame
	void Update () {
		Debug.Log ("WYNIK" + GameObject.Find ("Gra").GetComponent<Plansza> ().wynik);//wyswietla wynik w konsoli
		if (GameObject.Find ("Gra").GetComponent<Plansza> ().wynik==234)//pokazuje nam wygrana
			Debug.Log("WYGRALES MISTRZU");
		SprawdzenieWejscia ();
		Ruch ();
		AktualizacjaOrientacji ();
		AktualizowanieAnimacji ();
		ZjadanieZiarenek ();
	}
	void SprawdzenieWejscia ()//jaki klawisz nacisniety
	{
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {//jezeli wcisniety jest konkretny przycisk na klawiaturze to:
			ZmianaPozycji (Vector2.left);
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			ZmianaPozycji (Vector2.right);//(1,0)
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			ZmianaPozycji (Vector2.up);
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			ZmianaPozycji (Vector2.down);//(0,-1)
		}else if (Input.GetKeyDown (KeyCode.S) && predkosc<20) {//przyspieszamy wciskajac przycisk s
			predkosc += 2;
		}else if (Input.GetKeyDown (KeyCode.W) && predkosc>2) {//zwalniamy wciskajac przycisk s
			predkosc -= 2;
		}
	}

	void ZmianaPozycji(Vector2 d){//jezeli napotka na wezel to ma do wyboru rozne kierunki poruszania sie
		if (d != kierunek)//kierunkowi na poczatek przypisalem wartosc wektora zerowego
			kolejnyKierunek = d;//nowy kierunek
		if (obecnyWezel != null) {
			Wezly ruchWKierunkuWezla = MozliwoscRuchu (d);
			if (ruchWKierunkuWezla != null) {//do poruszania sie od wezla do wezla Pacmana
				kierunek = d;
				wezelDocelowy = ruchWKierunkuWezla;
				poprzedniWezel = obecnyWezel;
				obecnyWezel = null;
			}
		}
	}

	void Ruch()
	{
		
		if (wezelDocelowy != obecnyWezel && wezelDocelowy != null) {

			if (kolejnyKierunek == kierunek * -1) {//zeby mogl zmieniac zwrot pomiedzy wezlami
				kierunek *= -1;
				Wezly chwilowyWezel = wezelDocelowy;
				wezelDocelowy = poprzedniWezel;//zeby nie zmienial pozycji w wezle tylko w obojetnym polozeniu
				poprzedniWezel = chwilowyWezel;//zeby nie wracal nagle na pozycje starego wezla
			}

			if (OminietyCel ()) {//do poruszania sie miedzy wezlami
				obecnyWezel = wezelDocelowy;
				transform.localPosition = obecnyWezel.transform.position;
				GameObject innyPortal = PrzejsciePortalem (obecnyWezel.transform.position);

				if (innyPortal != null) {
					transform.localPosition = innyPortal.transform.position;//zamienia pozycje portali
					obecnyWezel=innyPortal.GetComponent<Wezly>();//teraz na pozycji drugiego portalu znajduje sie obecnywezel
				}
				Wezly poruszaniewKierunkuWezla = MozliwoscRuchu (kolejnyKierunek);
				if (poruszaniewKierunkuWezla != null)//zeby nie przechodzic przez sciany
					kierunek = kolejnyKierunek;
				if (poruszaniewKierunkuWezla == null)
					poruszaniewKierunkuWezla = MozliwoscRuchu (kierunek);
				if (poruszaniewKierunkuWezla != null) {
					wezelDocelowy = poruszaniewKierunkuWezla;//zeby pacman poruszal sie od wezla do wezla po kazdym ziarnie
					poprzedniWezel = obecnyWezel;
					obecnyWezel = null;
				} else {
					kierunek = Vector2.zero;
				}
			} else {//do poruszenia pacmanem
				transform.localPosition += (Vector3)(kierunek * predkosc) * Time.deltaTime;//zeby pacman mogl wystartowac
			}
		}
	}

	void RuchWKierunkuWezla(Vector2 d)//zeby poruszal sie od wezla do wezla
	{
		Wezly ruchWKierunkuWezla = MozliwoscRuchu (d);
		if (ruchWKierunkuWezla != null) {
			transform.localPosition = ruchWKierunkuWezla.transform.position;
			obecnyWezel = ruchWKierunkuWezla;
		}
	}

	void AktualizacjaOrientacji ()
	{
		if (kierunek == Vector2.left) {
			orientacja = Vector2.left;
			transform.localScale = new Vector3(-1,1,0);//do zmiany kierunku animacji,, zmieniamy kierunek animacji na lewo 
			transform.localRotation = Quaternion.Euler (0, 0, 0);//zxy
		} else if (kierunek == Vector2.right) {
			orientacja = Vector2.right;
			transform.localScale = new Vector3(1,1,1);
			transform.localRotation = Quaternion.Euler (0, 0, 0);
		} else if (kierunek == Vector2.up) {
			orientacja = Vector2.up;
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 90);//do obrocenia o 90 stopni w trakcie poruszania sie w gore
		} else if (kierunek == Vector2.down) {
			orientacja = Vector2.down;
			transform.localScale = new Vector3 (1, 1, 1);
			transform.localRotation = Quaternion.Euler (0, 0, 270);//obrot o 270 stopni osi y
		}
	}
	void AktualizowanieAnimacji(){
		if (kierunek == Vector2.zero) {
			GetComponent<Animator> ().enabled = false;//stop animacji
			GetComponent<SpriteRenderer> ().sprite = nieruchomyPacman;//przerywa animacje gdy uderzy o sciane pacman
		} else {
			GetComponent<Animator> ().enabled = true;//animacja jest widoczna
		}
	}

	void ZjadanieZiarenek(){
		GameObject o = UstawianieZiarenekNaPozycjach (transform.position);
		if (o != null) {
			Pytania pytanie = o.GetComponent<Pytania> ();
			if (pytanie != null) {
				if (!pytanie.zjedzone && (pytanie.czyToZiarno || pytanie.czyToDuzeZiarno)) {
					o.GetComponent<SpriteRenderer> ().enabled = false;//znika nam ziarenko
					pytanie.zjedzone = true;//zjedzony
					GameObject.Find("Gra").GetComponent<Plansza>().wynik+=1;
					zjedzoneZiarenka++;
				}
				if(zjedzoneZiarenka==234)
					SceneManager.LoadScene("Wygrana");
			}
		}
	}

	Wezly MozliwoscRuchu(Vector2 d)//wskazuje na mozliwosci ruchow po wezlach
	{
		Wezly ruchDoWezla = null;
		for (int i = 0; i < obecnyWezel.sasiedztwo.Length; i++) {
			if (obecnyWezel.prawidlowyKierunek [i] == d) {//jezeli znajdzie co chcial to przerywa dzialanie petli
				ruchDoWezla = obecnyWezel.sasiedztwo [i];
				break;//w pewnym momencie wyrzucalo blad, a po zastosowaniu break dziala poprawnie
			}
		}
		return ruchDoWezla;
	}

	GameObject UstawianieZiarenekNaPozycjach (Vector2 pos)//uwidacznianie ziaren dla pacmana
	{
		int ziarnoX = Mathf.RoundToInt (pos.x);
		int ziarnoY = Mathf.RoundToInt (pos.y);
		GameObject ziarno = GameObject.Find ("Gra").GetComponent<Plansza> ().plansza [ziarnoX, ziarnoY];
		if (ziarno != null) 
			return ziarno;
		
		return null;
	}

	Wezly PozycjonowanieWezla(Vector2 pos)//uwidocznianie wezlow dla pacmana
	{
		GameObject ziarno = GameObject.Find ("Gra").GetComponent<Plansza> ().plansza [(int)pos.x, (int)pos.y];
		if (ziarno != null) {
			return ziarno.GetComponent<Wezly> ();
		}
		return null;
	}

	bool OminietyCel(){//jezeli jest falszem to mozemy kontynuuowac gre,
		float wezelDoCelu = OdlegloscOdWezla (wezelDocelowy.transform.position);
		float wezelOdSiebie = OdlegloscOdWezla (transform.localPosition);
		return wezelOdSiebie > wezelDoCelu;
	}

	float OdlegloscOdWezla(Vector2 docelowaPozycja){
		Vector2 vec = docelowaPozycja - (Vector2)poprzedniWezel.transform.position;
		return vec.sqrMagnitude;//zwraca spierwiastkowana dlugosc tego wektora
	}

	GameObject PrzejsciePortalem(Vector2 pos)//sprawdza czy obecna pozycja wezla jest portalem
	{
		GameObject ziarno = GameObject.Find ("Gra").GetComponent<Plansza> ().plansza [(int)pos.x, (int)pos.y];
		if (ziarno != null) {
			if (ziarno.GetComponent<Pytania> () != null) {
				if (ziarno.GetComponent<Pytania> ().czyToPortal) {
					GameObject innyPortal = ziarno.GetComponent<Pytania> ().PrzejsciePortalem;
					return innyPortal;//zmienia pozycje na drugi portal
				}
			}
		}
		return null;
	}
}
