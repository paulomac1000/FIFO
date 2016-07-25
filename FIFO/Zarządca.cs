using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moduł_pamięci
{
    class Zarządca
    {
        static void Main(string[] args)
        {
            Pamięć pamięć = new Pamięć();
            Console.WriteLine("Dostępne polecenia:");
            Console.WriteLine("1: stwórz proces o podanej nazwie i zarezerwuj podaną liczbę komórek pamięci");
            Console.WriteLine("2: zabij proces o podanej nazwie");
            Console.WriteLine("3: odczytaj zawartość komórki pod podanym adresem");
            Console.WriteLine("4: zapisz komórkę pamięci pod podanym adresem");
            Console.WriteLine("5: wyświetl stan kolejki FIFO");
            Console.WriteLine("6: wyświetl globalną tablicę stron");
            Console.WriteLine("7: wyświetl tablicę stron procesu o podanej nazwie");
            Console.WriteLine("8: wyświetl zawartość ramek");
            Console.WriteLine("9: wyświetl zawartość stronnic");
            Console.WriteLine("10: wyświetl zawartość semafora MEMORY");
            Console.WriteLine("11: wyświetl informacje o procesach");
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine("\n");
            for (;;)
            {
                Console.WriteLine("Co chcesz zrobić?");

                int wybór;

                try
                {
                    wybór = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Podaj liczbę!");
                    continue;
                }
                

                switch (wybór)
                {
                    case 1: //stwórz proces
                        {
                            pamięć.stwórz_proces();
                            break;
                        }
                    case 2: //zabij proces
                        {
                            pamięć.zabij_proces();
                            break;
                        }
                    case 3: //odczyt pamięci spod adresu
                        {
                            pamięć.odczytaj_pamięć();
                            break;
                        }

                    case 4: //zapis pamięci pod adresem
                        {
                            pamięć.zapisz_pamięć();
                            break;
                        }
                    case 5: //stan kolejki FIFO
                        {
                            pamięć.wyświetl_kolejkę_FIFO();
                            break;
                        }
                    case 6: //wyświetla globalną tablicę stron
                        {
                            pamięć.wyświetl_tablicę_stron();
                            break;
                        }

                    case 7: //wyświetla tablicę stron procesu
                        {
                            pamięć.wyświetl_tablicę_stron_procesu();
                            break;
                        }
                    case 8: //wyświetla zawartość ramek
                        {
                            pamięć.wyświetl_zawartość_ramek();
                            break;
                        }
                    case 9: //wyświetla zawartość stronnic
                        {
                            pamięć.wyświetl_zawartość_stronnic();
                            break;
                        }
                    case 10: //wyświetla zawartość semafora MEMORY
                        {
                            pamięć.wyświetl_stan_semafora_memory();
                            break;
                        }
                    case 11: //wyświetla informacje o procesach
                        {
                            pamięć.wyświetl_procesy();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Podałeś zły numer! Dostępne komendy to:");
                            Console.WriteLine("1: stwórz proces o podanej nazwie i zarezerwuj podaną liczbę komórek pamięci");
                            Console.WriteLine("2: zabij proces o podanej nazwie");
                            Console.WriteLine("3: odczytaj zawartość komórki pod podanym adresem");
                            Console.WriteLine("4: zapisz komórkę pamięci pod podanym adresem");
                            Console.WriteLine("5: wyświetl stan kolejki FIFO");
                            Console.WriteLine("6: wyświetl globalną tablicę stron");
                            Console.WriteLine("7: wyświetl tablicę stron procesu o podanej nazwie");
                            Console.WriteLine("8: wyświetl zawartość ramek");
                            Console.WriteLine("9: wyświetl zawartość stronnic");
                            Console.WriteLine("10: wyświetl zawartość semafora MEMORY");
                            Console.WriteLine("11: wyświetl informacje o procesach");
                            break;
                        }
                }
            }
        }
    }
}
