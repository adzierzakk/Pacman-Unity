using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//robie klase wezly i zaznaczam wszystkie ziarenka, ktore znajduja sie w wezlach
public class Wezly : MonoBehaviour {
	public Wezly[] sasiedztwo;//w grze zaznaczamy sasiadujace wezly//sasiedztwo to tablica obiektow typu wezly
	public Vector2[] prawidlowyKierunek;//obiekt bez przypisanej wartosci//pod spodem piszemy kod, ktory za mnie przypisuje mozliwe kierunki(gora,dol,lewo,prawo)
	//vector2 reprezentuje 2wymiarowy wektor lub punkt
	// Use this for initialization
	void Start () {// w trakcie rozpoczacia gry dopiero przypisuje kierunki//length to rozmiar tablicy
		prawidlowyKierunek = new Vector2[sasiedztwo.Length];//tablica prawidlowyKierunek ma dlugosc taka sama jak sasiedztwo 
		for (int i = 0; i < sasiedztwo.Length; i++) {//zalezy ile ma sasiedztwa wokol siebie wezel
			Wezly sasiad = sasiedztwo [i];
			Vector2 chwilowyWektor = sasiad.transform.localPosition - transform.localPosition;//odejmuje pozycje sasaiada od aktualnej pozycji
			prawidlowyKierunek [i] = chwilowyWektor.normalized;//normalized sprawia ze zachowuje swoj kierunek, ale dlugosc staje sie 1
		}
	}
	
	// Update is called once per frame

}
