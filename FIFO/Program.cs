using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moduł_pamięci
{
    class Pamięć
    {
        public void stwórz_proces()
        {
            Console.WriteLine("Podaj nazwę procesu.");
            string nazwa = Console.ReadLine();

            Proces proces = procesy.Find(poszukiwany => poszukiwany.nazwa == nazwa);

            if (proces != null)
            {
                Console.WriteLine("Już istnieje proces o podanej nazwie!");
                return;
            }

            proces = new Proces();
            proces.nazwa = nazwa;

            Console.WriteLine("Podaj, ile komórek pamięci ma zająć proces");
            proces.pamięć = Convert.ToInt32(Console.ReadLine());

            if (XA(ref proces))
            {
                procesy.Add(proces);
                Console.WriteLine("Stworzono proces!");
                Console.WriteLine("Zakres adresów procesu: {0} - {1}", proces.adres_początkowy, proces.adres_końcowy);
            }
            else
            {
                Console.WriteLine("Nie stworzono procesu!");
            }

            
        }

        public void zabij_proces()
        {
            string nazwa;

            Console.WriteLine("Podaj nazwę procesu do zabicia");
            nazwa = Console.ReadLine();

            Proces proces = procesy.Find(poszukiwany => poszukiwany.nazwa == nazwa);
           
            if (proces == null)
            {
                Console.WriteLine("Nie znaleziono procesu o podanej nazwie");
                return;
            }

            XF(ref proces);

            Console.WriteLine("Prawidłowo zabito proces!");
        }

        public void zapisz_pamięć()
        {
            int adres;
            Console.WriteLine("Podaj adres pod którym chcesz zapisać dane");
            adres = Convert.ToInt32(Console.ReadLine());
            char dane;
            Console.WriteLine("Podaj znak, któy chcesz zapisać");
            dane = Console.ReadLine()[0];
            

            Proces proces = do_którego_procesu_należy_adres(adres);
            adres %= 1000; //usuwam z adresu tysiące aby móc obliczyć segment i offset
            int strona = adres / 16;
            int offset = adres % 16;
            if (proces == null)
            {
                Console.WriteLine("Podany adres nie należy do żadnego procesu");
                return;
            }
            if (tablica_stron[proces.numery_stron[strona]].poprawność == true) //jeżeli zapisujemy w ramce
            {
                pamięć_fizyczna[tablica_stron[proces.numery_stron[strona]].numer * 16 + offset] = dane;
            }
            else //jeżeli zapisujemy w stronnicy
            {
                pamięć_pomocnicza[tablica_stron[proces.numery_stron[strona]].numer * 16 + offset] = dane;
            }
        }

        public void odczytaj_pamięć()
        {
            int adres;
            Console.WriteLine("Podaj adres spod którego chcesz odczytać dane");
            adres = Convert.ToInt32(Console.ReadLine());

            Proces proces = do_którego_procesu_należy_adres(adres);

            adres %= 1000; //usuwam z adresu tysiące aby móc obliczyć segment i offset

            int strona = adres / 16;
            int offset = adres % 16;

            if (proces == null)
            {
                Console.WriteLine("Podany adres nie należy do żadnego procesu");
                return;
            }
            if (tablica_stron[proces.numery_stron[strona]].poprawność == true) //jeżeli odczytujemy z ramki
            {
                Console.WriteLine(pamięć_fizyczna[tablica_stron[proces.numery_stron[strona]].numer * 16 + offset]);
            }
            else //jeżeli odczytujemy ze stronnicy
            {
                if (wolne_ramki.Count > 0) //jeżeli są wolne ramki to stronnica idzie do ramki
                {
                    int wolna_ramka = znajdź_wolną_ramkę();
                    string bufor = odczytaj_stronnicę(tablica_stron[proces.numery_stron[strona]].numer);
                    zapisz_ramkę(wolna_ramka, bufor);
                    zwolnij_stronnicę(tablica_stron[proces.numery_stron[strona]].numer);
                    tablica_stron[proces.numery_stron[strona]].numer = wolna_ramka;
                    tablica_stron[proces.numery_stron[strona]].poprawność = true;
                }
                else if (wolne_stronnice.Count > 0) //jeżeli nie ma wolnych ramek ale jest jakaś wolna stronnica
                {
                    zwolnij_x_najstarszych_ramek(1);
                    int wolna_ramka = znajdź_wolną_ramkę();
                    string bufor = odczytaj_stronnicę(tablica_stron[proces.numery_stron[strona]].numer);
                    zapisz_ramkę(wolna_ramka, bufor);
                    zwolnij_stronnicę(tablica_stron[proces.numery_stron[strona]].numer);
                    tablica_stron[proces.numery_stron[strona]].numer = wolna_ramka;
                    tablica_stron[proces.numery_stron[strona]].poprawność = true;
                    FIFO.Enqueue(wolna_ramka); //dodajemy zapisywaną ramkę do kolejki FIFO
                }
                else //jeżeli nie ma wolnych ramek i stronnic to trzeba je zamienić miejscami
                {
                    int stronnica = tablica_stron[proces.numery_stron[strona]].numer; //ustalam, w której stronnicy znajduje się odczytywany element
                    string zawartość_stronnicy = odczytaj_stronnicę(stronnica); //odczytuję zawartość całej stronnicy do stringa
                    int najstarsza_ramka = FIFO.Dequeue(); //znajduję najstarszą ramkę do przydzielenia znalezionej stronnicy
                    string zawartość_ramki = odczytaj_ramkę(najstarsza_ramka); //odczytuję zawartość najstarszej ramki
                    tablica_stron[proces.numery_stron[strona]].numer = najstarsza_ramka; //od teraz strona będzie wskazywać na ramkę a nie na stronnicę gdyż odczyt realizowany jest tylko z ramek
                    tablica_stron[proces.numery_stron[strona]].poprawność = true; //oznaczamy że to jest ramka
                    Strona strona_z_ramką = tablica_stron.Find(poszukiwany => poszukiwany.numer == najstarsza_ramka && poszukiwany.poprawność == true); //znajduję stronę która wskazuje dotychczas na najstarszą ramkę
                    strona_z_ramką.numer = stronnica; //od teraz strona wskazująca na ramkę wskazuje na stronnicę
                    strona_z_ramką.poprawność = false; //zaznaczam że wskazuje na stronnicę modyfikując bit poprawności
                    zapisz_ramkę(najstarsza_ramka, zawartość_stronnicy); //zapisuję do ramki zawartość ze stronnicy
                    zapisz_stronnicę(stronnica, zawartość_ramki); //zapisuję do stronnicy zawartość z ramki
                    FIFO.Enqueue(najstarsza_ramka); //dodajemy zapisywaną ramkę do kolejki FIFO
                }
                FIFO.Enqueue(strona);
                Console.WriteLine(pamięć_fizyczna[tablica_stron[proces.numery_stron[strona]].numer * 16 + offset]);
            }
           
        }


        #region przydział/zwalnianie pamięci
        bool XA(ref Proces proces)
        {
            int ile_komórek_potrzeba = proces.pamięć;

            int ile_ramek = ile_potrzeba_ramek(proces.pamięć); //w ilu ramek w sumie program zajmie

            int ile_należy_się_ramek = ile_ramek_przydzielić(ile_ramek); //ile ramek się programowi nalezy zgodnie z przydziałem proporcjonalnym
            int ile_należy_się_stronic = ile_ramek - ile_należy_się_ramek; //jezeli jest wymaganych więcej ramek niż proporcjonalnie może dostać proces, pozostałe ramki zostaną przesunięte do pamięci wirtualnej

            if (ile_ramek <= wolne_strony.Count) //jeżeli starczy ramek (uwzględniając przenoszenie do stronic) na proces to alokuję pamięć
            {
                przydziel_pamięć_programowi(ref proces);
                return true;
            }
            else if (ile_ramek > tablica_stron.Count) //jeżeli nigdy nie starczy ramek na proces
            {
                Console.WriteLine("Nie ma możliwości zaalokowania pamięci dla procesu, gdyż ten wymaga więcej pamięci niż jest dostępne");
                return false;
            }
            else //jeżeli nie ma obecnie wystarczająco wolnej pamięci to proces idzie na semafor
            {
                memory.P(ref proces);

                return true;
            }
        }

        bool XF(ref Proces proces)
        {
            zwolnij_przydzieloną_pamięć_programowi(ref proces);
            procesy.Remove(proces);
            int wartość_semafora_memory = memory.getWartość;
            if (wartość_semafora_memory < 0)
            {
                for (int i = wartość_semafora_memory; i < 0; i++)
                {
                    Proces bufor = memory.V();
                    XA(ref bufor);
                }
            }
            return true;
        }

        #endregion

        #region do testowania

        public void wyświetl_zawartość_ramek()
        {
            System.Console.WriteLine("------------------------------------");
            System.Console.WriteLine("TABLICARAMEK:");
            System.Console.WriteLine("Numer   Zajęta? Zawartość", " ");
            for (int i = 0; i < 16; i++)
            {
                Console.Write("{0}\t{1}\t", i, tablica_ramek[i].zajęta);

                System.Console.Write(odczytaj_ramkę(i));

                System.Console.Write("\n");
            }
            System.Console.WriteLine("------------------------------------");
        }

        public void wyświetl_zawartość_stronnic()
        {
            System.Console.WriteLine("------------------------------------");
            System.Console.WriteLine("TABLICASTRONNIC:");
            System.Console.WriteLine("Numer Zajęta? Zawartość");
            for (int i = 0; i < 16; i++)
            {
                Console.Write("{0}\t{1}\t", i, tablica_stronnic[i].zajęta);
                System.Console.Write(odczytaj_stronnicę(i));

                System.Console.Write("\n");
            }


            System.Console.WriteLine("------------------------------------");
        }


        public void wyświetl_tablicę_stron()
        {
            System.Console.WriteLine("------------------------------------");
            System.Console.WriteLine("TABLICASTRON:");
            System.Console.WriteLine("NumerStrony NumerRamki Poprawność");

            for (int i = 0; i < 32; i++)
            {
                Console.WriteLine("\t{0}\t{1}\t\t{2}", i, tablica_stron[i].numer, tablica_stron[i].poprawność);
            }
            System.Console.WriteLine("------------------------------------");
        }

        public void wyświetl_tablicę_stron_procesu()
        {
            Console.WriteLine("Podaj nazwę procesu, którego tablicę stron chcesz wyświetlić");
            string nazwa = Console.ReadLine();
            Proces proces = procesy.Find(poszukiwany => poszukiwany.nazwa == nazwa);

            if (proces == null)
            {
                Console.WriteLine("Nie znaleziono procesu o podanej nazwie");
                return;
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine("TABLICA STRON PROCESU");
            Console.WriteLine("NumerStronyProcesu NumerStrony");

            for (int i=0; i<proces.numery_stron.Count; i++)
            {
                Console.WriteLine("\t {0} \t {1}", i, proces.numery_stron[i]);
            }

            Console.WriteLine("------------------------------------");
        }

        public void wyświetl_kolejkę_FIFO()
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine("KOLEJKA FIFO");
            Console.WriteLine("Zawiera numery ramki w kolejności zajmowania (na górze zajęte najwcześniej)");
            if (FIFO.Count == 0)
            {
                Console.WriteLine("Kolejka FIFO jest pusta");
                Console.WriteLine("------------------------------------");
                return;
            }
            foreach (int bufor in FIFO)
            {
                Console.WriteLine(bufor + " ");
            }
            Console.WriteLine("------------------------------------");
        }

        public void wyświetl_stan_semafora_memory()
        {
            memory.wyświetl_stan_semafora_memory();
        }

        public void wyświetl_procesy()
        {
            Console.WriteLine("------------------------------------");
            if (procesy.Count == 0)
            {
                Console.WriteLine("Brak procesów");
                Console.WriteLine("------------------------------------");
                return;
            }
            foreach (Proces proces in procesy)
            {
                Console.WriteLine("Nazwa: {0}, wymagana pamięć: {1}, adres początkowy: {2}, adres_końcowy{3}", proces.nazwa, proces.pamięć, proces.adres_początkowy, proces.adres_końcowy);
            }
            Console.WriteLine("------------------------------------");
        }

        #endregion

        #region komórki pamięci, wszystkie listy i semafory

        #region komórki pamięci
        char[] pamięć_fizyczna;
        char[] pamięć_pomocnicza;
        #endregion

        #region semafory
        MEMORY memory = new MEMORY();
        #endregion

        #region kolejka FIFO
        Queue<int> FIFO = new Queue<int>();
        #endregion

        #region ramki
        List<Ramka> tablica_ramek = new List<Ramka>();
        Stack<int> wolne_ramki = new Stack<int>();
        void inicjalizacja_listy_ramek() //inicjalizuje listę ramek aby zawierała odpowiednie wskazania na komórki pamięci
        {
            for (int i = 0; i < 16; i++)
            {
                Ramka ramka = new Ramka();
                ramka.adres_początkowy = i * 16;
                ramka.adres_końcowy = i * 16 + 16 - 1;
                tablica_ramek.Add(ramka);
            }
        }
        
        #endregion

        #region stronnice
        Stack<int> wolne_stronnice = new Stack<int>();
        List<Ramka> tablica_stronnic = new List<Ramka>();
        void inicjalizacja_listy_stronnic() //inicjalizuje listę stronic (ramek pamięci na dysku) aby zawierała odpowiednie wskazania na komórek na dysku
        {
            for (int i = 0; i < 16; i++)
            {
                Ramka stronica = new Ramka();
                stronica.adres_początkowy = i * 16;
                stronica.adres_końcowy = i * 16 + 16 - 1;
                tablica_stronnic.Add(stronica);
            }
        }
        #endregion

        #region inne
        List<Proces> procesy = new List<Proces>();
        List<Strona> tablica_stron = new List<Strona>();
        Stack<int> wolne_strony = new Stack<int>();
        Stack<int> wolne_zakresy_adresów = new Stack<int>();
        #endregion

        #endregion

        #region konstruktor i inicjalizacja list i stosów
        public Pamięć()
        {
            pamięć_fizyczna = new char[256];
            pamięć_pomocnicza = new char[256];

            for (int i = 0; i < 256; i++)
            {
                pamięć_fizyczna[i] = '-';
                pamięć_pomocnicza[i] = '-';
            }

            for (int i = 31; i >= 0; i--)
            {
                wolne_strony.Push(i);
                wolne_zakresy_adresów.Push(i);
            }

            for (int i = 15; i >= 0; i--)
            {
                wolne_ramki.Push(i);
                wolne_stronnice.Push(i);
            }

            for (int i = 0; i<32; i++)
            {
                Strona strona = new Strona();
                tablica_stron.Add(strona);
            }
            inicjalizacja_listy_ramek();
            inicjalizacja_listy_stronnic();
        }
        #endregion

        #region operacje na ramkach

        string odczytaj_ramkę(int ramka) //odczytuje ramkę o zadanym numerze i zwraca jej zawartość w stringu
        {
            char[] bufor = new char[16]; //tworzę bufor na odczytane dane

            for (int i = 0; i < 16; i++)
            {
                bufor[i] = pamięć_fizyczna[ramka * 16 + i]; //odczytuję całą ramkę
            }

            string zwracanie = new string(bufor); //tworzę stringa z tablicy charów (były problemy z funkcją ToString)
            return zwracanie; //zwracam stringa
        }

        void zapisz_ramkę(int numer_ramki, string znaki) //zapisuje znaki w konkretnej ramce (odpowiadającej dostarczonemu segmentowi); nie zmienia bitu używania na true; jest to zmieniane przy wyszukiwaniu wolnej ramki
        {
            int adres = numer_ramki * 16;

            for (int i = 0; i < znaki.Length; i++)
            {
                pamięć_fizyczna[adres + i] = znaki[i];
            }
        }

        #endregion

        #region operacje na stronnicach

        string odczytaj_stronnicę(int stronnica) //odczytuje ramkę o zadanym numerze i zwraca jej zawartość w stringu
        {
            char[] bufor = new char[16]; //tworzę bufor na odczytane dane

            for (int i = 0; i < 16; i++)
            {
                bufor[i] = pamięć_pomocnicza[stronnica * 16 + i]; //odczytuję całą ramkę
            }

            string zwracanie = new string(bufor); //tworzę stringa z tablicy charów (były problemy z funkcją ToString)
            return zwracanie; //zwracam stringa
        }

        void zapisz_stronnicę(int numer_stronnicy, string znaki) //zapisuje znaki w konkretnej ramce (odpowiadającej dostarczonemu segmentowi); nie zmienia bitu używania na true; jest to zmieniane przy wyszukiwaniu wolnej ramki
        {
            int adres = numer_stronnicy * 16;

            for (int i = 0; i < znaki.Length; i++)
            {
                pamięć_pomocnicza[adres + i] = znaki[i];
            }
        }

        #endregion

        #region przydział pamięci

        Proces do_którego_procesu_należy_adres(int adres)
        {
            return procesy.Find(poszukiwany =>  adres >= poszukiwany.adres_początkowy &&  adres <= poszukiwany.adres_końcowy);
        }

        int ile_potrzeba_ramek(int ile) //zwraca liczbę wymaganych ramek/stronnic wymaganych do przechowania zadanej ilości znaków
        {
            if (ile <= 0)
            {
                return 0;
            }
            int ilość_ramek = ile / 16;
            if (ile % 16 != 0)
            {
                ilość_ramek++;
            }

            return ilość_ramek;
        }

        int znajdź_wolną_ramkę()
        {
            int ramka = wolne_ramki.Pop();
            tablica_ramek[ramka].zajęta = true;
            return ramka;
        }

        void zwolnij_ramkę(int ramka)
        {
            wolne_ramki.Push(ramka); //oznaczam ramkę jako wolną
            tablica_ramek[ramka].zajęta = false; //w tablicy ramek oznaczam ją jako niezajętą

            //z kolejki FIFO muszę teraz usunąć tę ramkę:
            Queue<int> bufor = new Queue<int>(); //tworzę tymczasową kolejkę pomocniczą

            foreach (int element in FIFO) //wrzucam do niej wszystkie elementy z kolejki FIFO nie będące rozpatrywaną ramką
            {
                if (element != ramka)
                {
                    bufor.Enqueue(element);
                }
            }
            FIFO = bufor; //nowa buforowa kolejka bez numeru rozpatrywanej ramki jest od teraz kolejką FIFO
        }

        int znajdź_wolną_stronę()
        {
            int strona = wolne_strony.Pop();
            return strona;
        }

        void zwolnij_stronę(int strona)
        {
            wolne_strony.Push(strona);

            if (tablica_stron[strona].poprawność == true)
            {
                zwolnij_ramkę(tablica_stron[strona].numer);
            }
            else
            {
                zwolnij_stronnicę(tablica_stron[strona].numer);
            }

            tablica_stron[strona].poprawność = false;
        }

        int znajdź_wolną_stronnicę()
        {
            int stronnica = wolne_stronnice.Pop();
            tablica_stronnic[stronnica].zajęta = true;
            return stronnica;
        }

        void zwolnij_stronnicę(int stronnica)
        {
            wolne_stronnice.Push(stronnica);
            tablica_stronnic[stronnica].zajęta = false;
        }

        int ile_ramek_przydzielić(int ile_potrzebuje) //algorytm proporcjonalnego przydziału ramek
        {
            int ile_wolnych = wolne_ramki.Count;
            if (ile_potrzebuje <= ile_wolnych)
            {
                return ile_potrzebuje;
            }
            else
            {
                int ile_zajęte = 16 - ile_wolnych;
                int zwracanie = (int)Math.Ceiling(((double)ile_potrzebuje / ((double)ile_zajęte + (double)ile_potrzebuje) * (double)16));
                return zwracanie;
            }
        }

        void zwolnij_x_najstarszych_ramek(int x)
        {
            for (int i=0; i< x; i++)
            {
                int zdjęta = FIFO.Dequeue();
                int stronnica = znajdź_wolną_stronnicę();
                zapisz_stronnicę(stronnica, odczytaj_ramkę(zdjęta));
                zwolnij_ramkę(zdjęta);
                Strona strona = tablica_stron.Find(poszukiwany => poszukiwany.numer == zdjęta && poszukiwany.poprawność == true);

                strona.numer = stronnica;
                strona.poprawność = false;
            }
        }

        void przydziel_pamięć_programowi(ref Proces proces) //przydziela pamięć programowi, ramki/stronnice pozostają puste
        {
            int potrzeba_stron = ile_potrzeba_ramek(proces.pamięć); //ile program potrzebuje ramke + stronnic
            int przyznano_ramek = ile_ramek_przydzielić(potrzeba_stron); //biorąc pod uwagę zajętość ramek oblicza ile proporcjonalnie należy się ramek programowi


            if (potrzeba_stron == 0) //jeżeli następuje alokacja pamięci dla programu potrzebującego 0 komórek (w wyniku błędu?)
            {
                return;
            }

            if (przyznano_ramek > wolne_ramki.Count)  //jeżeli programowi należy się więcej ramek niż jest wolnych, należy zwolnić brakującą ilość
            {
                int ile_zwolnic = przyznano_ramek - wolne_ramki.Count;
                zwolnij_x_najstarszych_ramek(ile_zwolnic);
            }

            for (int i = 0; i < przyznano_ramek; i++)
            {
                int strona = znajdź_wolną_stronę();

                tablica_stron[strona].numer = znajdź_wolną_ramkę();
                tablica_stron[strona].poprawność = true;
                FIFO.Enqueue(tablica_stron[strona].numer);
                zapisz_ramkę(tablica_stron[strona].numer, proces.nazwa);

                proces.numery_stron.Add(strona);
            }
            for (int i = przyznano_ramek; i < potrzeba_stron; i++)
            {
                int strona = znajdź_wolną_stronę();

                tablica_stron[strona].numer = znajdź_wolną_stronnicę();
                tablica_stron[strona].poprawność = false;
                zapisz_stronnicę(tablica_stron[strona].numer, proces.nazwa);

                proces.numery_stron.Add(strona);
            }

            proces.adres_początkowy = wolne_zakresy_adresów.Pop() * 1000;
            proces.adres_końcowy = proces.adres_początkowy + proces.pamięć - 1;
        }

        void zwolnij_przydzieloną_pamięć_programowi(ref Proces proces)
        {
            for (int i = 0; i < proces.numery_stron.Count; i++)
            {
                zwolnij_stronę(i);
            }

            wolne_zakresy_adresów.Push(proces.adres_początkowy / 1000);
            proces.adres_początkowy = -1;
            proces.adres_końcowy = -1;
        }
        #endregion
    }
}