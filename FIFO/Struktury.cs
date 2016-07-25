using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moduł_pamięci
{
    public class Ramka
    {
        public int adres_początkowy { get; set; }
        public int adres_końcowy { get; set; }
        public Boolean zajęta {get; set;}

        public Ramka()
        {
            zajęta = false;
        }
    }

    public class Strona
    {
        public int numer { get; set; } //na jaką ramkę/stronę wskazuje
        public bool poprawność { get; set; } // 1-fizyczna 0 -wirtualna
    }

    public class Proces
    {
        public string nazwa { get; set; }
        public int pamięć { get; set; }
        public int adres_początkowy { get; set; }
        public int adres_końcowy { get; set; }
        public List<int> numery_stron { get; set; }

        public Proces()
        {
            nazwa = "brak";
            pamięć = -1;
            adres_początkowy = -1;
            adres_końcowy = 1;
            numery_stron = new List<int>();
        }
    }

}
