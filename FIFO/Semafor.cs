using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moduł_pamięci
{
    class MEMORY //semafor oczekujących
    {
        private Queue<Proces> oczekujące = new Queue<Proces>();
        private int wartość = 0;

        public int getWartość
        {
            get { return wartość; }
        }

        public void P(ref Proces proces)
        {
            wartość--;

            oczekujące.Enqueue(proces);

            Console.WriteLine("Dodano proces na semafor oczekujących!");
        }

        public Proces V()
        {
            wartość++;

            Console.WriteLine("Zdjęto proces z semaforu, nastąpi próba alokacji pamięci!");

            Proces proces = oczekujące.Dequeue();

            return proces;
        }

        public void wyświetl_stan_semafora_memory()
        {
            string bufor = "Wartość semafora: " + wartość.ToString() + "\n";

            foreach (Proces proces in oczekujące)
            {
                bufor += "Nazwa procesu: " + proces.nazwa + "\n";
            }

            Console.WriteLine(bufor);
        }

    }
}
