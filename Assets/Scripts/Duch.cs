using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duch : MonoBehaviour {
	//zakladam czasy oraz przyspieszenia, ktore mozna pozniej zmienic w inspectorze
	public float przyspieszenie = 3.9f;
	public int CzasUwolnieniaRozowegoDucha = 5;
	public int CzasUwolnieniaNiebieskiegoDucha=14;
	public int CzasUwolnieniaPomaranczowegoDucha=21;
	public float CzasUwolnieniaCzerwonegoDucha = 0;

	public bool czyJestWDomuDuchow=false;//zaznaczam czy jest w domu duchow

	public Wezly pozycjaStartowa;//zaznaczam w Unity, na pozycji którego Wezla znajduje sie duch
	public Wezly domowyWezel;//jest to wezel do ktorego zmierzaja duszki w pewnym etapie gry

	public int czasTrybuRozproszenia1=7;
	public int czasTrybuScigania1=20;
	public int czasTrybuRozproszenia2=7;
	public int czasTrybuScigania2=20;
	public int czasTrybuRozproszenia3=5;
	public int czasTrybuScigania3=20;
	public int czasTrybuRozproszenia4=5;
	//4 czas mam nizej

	private int iteracjaZmianyTrybu=1;//potrzebne bedzie do obejscia kazdego trybu
	private float czasZmianyTrybu=0;
	public enum Stan{//lista wyliczeniowa, ktora jest przypisana do tego typu
		Poscig,
		Rozproszenie
	}
	Stan obecnyStan=Stan.Rozproszenie;//defaultowa wartosc
	Stan poprzedniStan;

	public enum TypDucha{//lista wyliczeniowa, ktora jest przypisana do tego typu
		Czerwony,
		Rozowy,
		Niebieski,
		Pomaranczowy
	}
	public TypDucha typDucha=TypDucha.Czerwony;//defaultowy kolor to czerwony
	private GameObject pacMan;
	private Wezly obecnyWezel, docelowyWezel, poprzedniWezel;
	private Vector2 kierunek,kolejnyKierunek;

	// Use this for initialization
	void Start () {
		pacMan = GameObject.FindGameObjectWithTag ("Pacman");//szuka obiektu oznaczonego tagiem Pacman
		Wezly ziarno = UstawianieZiarenekNaPozycjach (transform.localPosition);
		if (ziarno != null) {
			obecnyWezel = ziarno;//przypisuje wezel w ktorym sie znajduje do ziarna
		}
		if(czyJestWDomuDuchow){
			kierunek=Vector2.up;//bo w gore wychodza
			docelowyWezel=obecnyWezel.sasiedztwo[0];//poniewaz nad nim jest tylko jeden sasiad
		}else{//jak jest poza domkiem
			kierunek=Vector2.left;//zakladamy kierunek ruchu w lewo
			docelowyWezel = WybierzKolejneZiarno ();
		}

		poprzedniWezel = obecnyWezel;
	}
	public void Restart(){
		transform.position = pozycjaStartowa.transform.position;//ustawia duchy na pozycji startowej
		CzasUwolnieniaCzerwonegoDucha = 0;
		iteracjaZmianyTrybu = 1;
		czasZmianyTrybu = 0;
		if (transform.name != "Duch")//czy nazwa obiektu to nie duch, czyli wszystkie poza czerwonym
			czyJestWDomuDuchow = true;//zaznaczamy automatycznie ze sa w domu duszkow
		obecnyWezel = pozycjaStartowa;
		if (czyJestWDomuDuchow) {
			kierunek = Vector2.up;//poczatkowy ruch w gore
			docelowyWezel = obecnyWezel.sasiedztwo [0];//jest tylko jeden docelowy wezel na poczatku
		} else {
			kierunek = Vector2.left;//czerwony rusza sie w lewo
			docelowyWezel = WybierzKolejneZiarno ();
		}
		poprzedniWezel = obecnyWezel;
	}
	// Update is called once per frame
	void Update () {
		AktualizacjaStanu ();
		Ruch ();
		UwolnienieDuchow ();
		SprawdzenieKolizji ();

	}

	void SprawdzenieKolizji (){
		Rect kwadratWokolDucha = new Rect (transform.position, transform.GetComponent<SpriteRenderer> ().sprite.bounds.size / 4); //sprite, to aktualna grafika, rozmiar bounds//pokazuje szerokosc i wysokosc duchow//robi kwadrat wokol naszych duchow, zeby byl rozmiar, znamy rozmiar ducha, dzielimy na cztery zeby zmniejszyc obszar naszego ducha, blizej centrum
		Rect kwadratWokolPacmana=new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size/4);//kwadrat wokol pacmana podzielony na 4, zeby nie tylko kawalek pacmana lapal tylko blizej centrum
		if (kwadratWokolDucha.Overlaps (kwadratWokolPacmana)) {//jezeli kwadrat wokol ducha pokrywa sie z kwadratem wokol pacmana
			GameObject.Find ("Gra").transform.GetComponent<Plansza> ().Restart ();//resetuje gre, jak zlapie duch pacmana
			Debug.Log ("ZOSTALES ZLAPANY");//w konsoli wyswietla sie ze zostal zlapany
		}
	}
	void Ruch(){
		if (docelowyWezel != obecnyWezel && docelowyWezel != null && !czyJestWDomuDuchow) {//czyjestwdomuduchow musi byc falszem(odpala sie jak duch jest poza domem)
			if (OminietyCel ()) {//sprawdza czy duch znajduje sie w swoim otoczeniu, jezeli tak to wykonuje sie else
				obecnyWezel = docelowyWezel;
				transform.localPosition = obecnyWezel.transform.position;//pozycja wzgledem macierzystej pozycji
				GameObject innyPortal = PrzejsciePortalem (obecnyWezel.transform.position);//poruszanie sie ducha w portalu
				if (innyPortal != null) {
					transform.localPosition = innyPortal.transform.position;//zamiana pozycji na pozycje drugiego portalu
					obecnyWezel = innyPortal.GetComponent<Wezly> ();//ustawia obecnyWezel na pozycje innego portalu
				}
				docelowyWezel = WybierzKolejneZiarno ();//nastepny wezel bedzie kolejnym wezlem przez ktory przejdzie duch
				poprzedniWezel = obecnyWezel;
				obecnyWezel = null;
			} else {
				transform.localPosition += (Vector3)kierunek * przyspieszenie * Time.deltaTime;//sluzy do poruszania sie duchow
			}//time.deltaTime daje czas jaki uplynal od wyrenderowania ostatniej klatki gry
		}
	}
	void AktualizacjaStanu(){//zmienia tryb/aktualizuje go, w zaleznosci od czasu
		 {
			czasZmianyTrybu += Time.deltaTime;//czas jaki uplynal od poczatka gry
			if (iteracjaZmianyTrybu == 1) {
				if (obecnyStan == Stan.Rozproszenie && czasZmianyTrybu > czasTrybuRozproszenia1) {
					ZmianaStanu (Stan.Poscig);
					czasZmianyTrybu = 0;
				}
				if (obecnyStan == Stan.Poscig && czasZmianyTrybu > czasTrybuScigania1) {
					iteracjaZmianyTrybu = 2;
					ZmianaStanu (Stan.Rozproszenie);
					czasZmianyTrybu = 0;
				}
			} else if (iteracjaZmianyTrybu == 2) {
				if (obecnyStan == Stan.Rozproszenie && czasZmianyTrybu > czasTrybuRozproszenia2) {
					ZmianaStanu (Stan.Poscig);
					czasZmianyTrybu = 0;
				}
				if(obecnyStan==Stan.Poscig&&czasZmianyTrybu>czasTrybuScigania2){
					iteracjaZmianyTrybu=3;
					ZmianaStanu (Stan.Rozproszenie);
					czasZmianyTrybu = 0;
				}
			} else if (iteracjaZmianyTrybu == 3) {
				if (obecnyStan == Stan.Rozproszenie && czasZmianyTrybu > czasTrybuRozproszenia3) {
					ZmianaStanu (Stan.Poscig);
					czasZmianyTrybu = 0;
				}
				if (obecnyStan == Stan.Poscig && czasZmianyTrybu > czasTrybuScigania3) {
					iteracjaZmianyTrybu = 4;
					ZmianaStanu (Stan.Rozproszenie);
					czasZmianyTrybu = 0;
				}
			} else if (iteracjaZmianyTrybu == 4) {//iteracje stosujemy po to zeby przejsc do kazdego stanu
				if (obecnyStan == Stan.Rozproszenie && czasZmianyTrybu > czasTrybuRozproszenia4) {
					ZmianaStanu (Stan.Poscig);
					czasZmianyTrybu = 0;
				}
			}
		} 
	}

	void ZmianaStanu(Stan m){
		obecnyStan = m;
	}
	//bedzie zwracalo wektor2
	Vector2 PobieranieCzerwonegoDuchaZiarnaCel(){//czerwony duch sciga Pacmana
		Vector2 pozycjaPacmana = pacMan.transform.localPosition;//pobiera pozycje pacmana
		Vector2 ziarnoDocelowe = new Vector2 (Mathf.RoundToInt (pozycjaPacmana.x), Mathf.RoundToInt (pozycjaPacmana.y));//rowna sie zaokraglonej pozycji pacmana
		return ziarnoDocelowe;
	}
	Vector2 PobieranieRozowegoDuchaZiarnaCel(){//rozowy probuje zagrodzic droge pacmanowi
		//dlatego bierzemy pod uwage pozycje i orientacje, orientacje po to, ze zawsze np jak pacman porusza sie w lewo to rozowy probuje zagrodzic mu droge od lewej
		Vector2 pozycjaPacmana=pacMan.transform.localPosition;//pobiera pozycje pacmana
		Vector2 orientacjaPacmana = pacMan.GetComponent<Pacman>().orientacja;//pobiera orientacje
		int pozycjaPacmanaX = Mathf.RoundToInt (pozycjaPacmana.x);//zaokragla pozycje pacmana x
		int pozycjaPacmanaY = Mathf.RoundToInt (pozycjaPacmana.y);
		Vector2 ziarnoPacmana = new Vector2 (pozycjaPacmanaX, pozycjaPacmanaY);//pozycja pacmana zaokraglona
		Vector2 ziarnoDocelowe = ziarnoPacmana + (4 * orientacjaPacmana);//mnoze razy 4, zeby byc 4 kroki przed pacmanem(zakladam ruch w danym kierunku)
		return ziarnoDocelowe;//docelowy punkt rozowego
	}
	Vector2 PobieranieNiebieskiegoDuchaZiarnaCel(){//niebieski duch krazy wokol swojego otoczenia, ktore jest na dole mapy
		return Vector2.zero;
	}
	Vector2 PobieraniePomaranczowegoDuchaZiarnaCel(){//pomaranczowy tez
		return Vector2.zero;
	}
	Vector2 PobieranieZiarnaDocelowego(){//duchy skupiaja sie na swoim celu
		Vector2 doceloweZiarno = Vector2.zero;//(0,0) poczatkowe wspolrzedne
		if (typDucha == TypDucha.Czerwony) {
			doceloweZiarno = PobieranieCzerwonegoDuchaZiarnaCel ();
		} else if (typDucha == TypDucha.Rozowy) {
			doceloweZiarno = PobieranieRozowegoDuchaZiarnaCel ();
		} else if (typDucha == TypDucha.Niebieski) {
			doceloweZiarno = PobieranieNiebieskiegoDuchaZiarnaCel ();
		} else if (typDucha == TypDucha.Pomaranczowy) {
			doceloweZiarno = PobieraniePomaranczowegoDuchaZiarnaCel ();
		}
		return doceloweZiarno;
	}

	void UwolnienieRozowegoDucha(){
		if (typDucha == TypDucha.Rozowy && czyJestWDomuDuchow) {
			czyJestWDomuDuchow = false;
		}
	}
	void UwolnienieNiebieskiegoDucha(){
		if(typDucha==TypDucha.Niebieski&&czyJestWDomuDuchow){
			czyJestWDomuDuchow=false;
		}
	}
			void UwolnieniePomaranczowegoDucha(){
		if(typDucha==TypDucha.Pomaranczowy&&czyJestWDomuDuchow){
					czyJestWDomuDuchow=false;
				}
					}

	void UwolnienieDuchow(){
		CzasUwolnieniaCzerwonegoDucha += Time.deltaTime;//czas jaki uplynal od poczatku gry
		if (CzasUwolnieniaCzerwonegoDucha > CzasUwolnieniaRozowegoDucha)//jezeli przekroczy ten czas to uwalniaamy duchy
			UwolnienieRozowegoDucha ();
		if (CzasUwolnieniaCzerwonegoDucha > CzasUwolnieniaNiebieskiegoDucha)
			UwolnienieNiebieskiegoDucha ();
		if (CzasUwolnieniaCzerwonegoDucha > CzasUwolnieniaPomaranczowegoDucha)
			UwolnieniePomaranczowegoDucha ();
	}

	Wezly WybierzKolejneZiarno(){
		Vector2 doceloweZiarno = Vector2.zero;//(0,0)
		if (obecnyStan == Stan.Poscig) {

			doceloweZiarno = PobieranieZiarnaDocelowego ();//miejsce znajdowania sie pacmana
		} else if (obecnyStan == Stan.Rozproszenie) {//duchy poruszaja sie wokol swojego otoczenia
			doceloweZiarno = domowyWezel.transform.position;
		}
		Wezly ruchWKierunkuWezla = null;//wczesniej wyrzucalo blad, a po przypisaniu null jest ok
		Wezly[] znalezioneWezly = new Wezly[4];
		Vector2[] znalezionyKierunekWezla = new Vector2[4];
		int licznikWezlow = 0;
		for (int i = 0; i < obecnyWezel.sasiedztwo.Length; i++) {//do ilosci sasiadow wezla
			if (obecnyWezel.prawidlowyKierunek [i] != kierunek * -1) {//jezeli nie zmienia kierunku o 180 stopni, bo tak to wraca sie na stara pozycje
				znalezioneWezly [licznikWezlow] = obecnyWezel.sasiedztwo [i];//sasiedztwo danego wezla
				znalezionyKierunekWezla [licznikWezlow] = obecnyWezel.prawidlowyKierunek [i];//w ktorym kierunku moze sie poruszac
				licznikWezlow++;//liczy wezly w sasiedztwie
			}
		}
		if (znalezioneWezly.Length == 1) {//jezeli mam jeden wezel
			ruchWKierunkuWezla = znalezioneWezly [0];
			kierunek = znalezionyKierunekWezla [0];
		}
		if (znalezioneWezly.Length > 1) {//dla kilku wezlow szukamy tego najkrotszego
			float najmniejszaOdleglosc = 100000f;//f na koncu, zeby zajmowalo mniej pamieci bo inaczej jest double
			for (int i = 0; i < znalezioneWezly.Length; i++) {
				if (znalezionyKierunekWezla [i] != Vector2.zero) {
					float odleglosc = PobieranieOdleglosci (znalezioneWezly [i].transform.position, doceloweZiarno);//liczy odleglosc
					if (odleglosc < najmniejszaOdleglosc) {//szukamy najmniejszej odleglosci do pokonania jak najkrotszej trasy
						najmniejszaOdleglosc = odleglosc;
						ruchWKierunkuWezla = znalezioneWezly [i];
						kierunek = znalezionyKierunekWezla [i];
					}
				
				}
			
			}
		}
		return ruchWKierunkuWezla;
	}


	Wezly UstawianieZiarenekNaPozycjach(Vector2 pos){//widocznosc ziaren dla duchow
		GameObject ziarno = GameObject.Find ("Gra").GetComponent<Plansza> ().plansza [(int)pos.x, (int)pos.y];
		if (ziarno != null) {
			if (ziarno.GetComponent<Wezly>() != null) {
				return ziarno.GetComponent<Wezly> ();
			}
		}
		return null;
	}
	//GameObject zawsze ma na poczatku pozycje,,//GameObject zachowuje sie jak kontener dla komponentow
	GameObject PrzejsciePortalem (Vector2 pos){//przyjmuje wspolrzedne portalu
		GameObject ziarno = GameObject.Find ("Gra").GetComponent<Plansza> ().plansza [(int)pos.x, (int)pos.y];//zbiera pozycje na planszy
		if (ziarno != null) {
			if (ziarno.GetComponent<Pytania> ().czyToPortal) {//jezeli napotkalem na portal to:
				GameObject innyPortal = ziarno.GetComponent<Pytania> ().PrzejsciePortalem;//pobiera wspolrzedne drugiego portalu
				return innyPortal;
			}
		}
		return null;
	}
	float odlegloscOdWezla(Vector2 pozycjaDocelowa)
	{
		Vector2 vec = pozycjaDocelowa - (Vector2)poprzedniWezel.transform.position;
		return vec.sqrMagnitude;//zwraca spierwiastkowana dlugosc wektora
	}
	bool OminietyCel(){//sprawdzam czy duch znajduje sie juz przy swoim otoczeniu
		float wezelDoCelu = odlegloscOdWezla (docelowyWezel.transform.position);
		float wezelOdSiebie = odlegloscOdWezla (transform.localPosition);//odleglosc wezla znajdujacego sie tuz przy duchu
		return wezelOdSiebie > wezelDoCelu;//jezeli wezme na odwrot znaki to duch zaczyna wariowac wokol swojego otoczenia
	}
	float PobieranieOdleglosci(Vector2 posA, Vector2 posB){//liczymy odleglosc pomiedzy wezlami
		float dx = posA.x - posB.x;
		float dy = posA.y - posB.y;
		float dystans = Mathf.Sqrt (dx * dx + dy * dy);
		return dystans;
	}

}
