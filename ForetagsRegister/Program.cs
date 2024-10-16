using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ForetagsRegister
{

    internal class Program
    {
        public struct Foretag 
        {
            public string foretagsNamn;
            public string foretagsAdress;
            public string foretagsNummer; 

            // för sortering av txtfilen
            public string sort_ForetagNamn() 
            {
                return foretagsNamn + foretagsAdress; 
            }
        }
        // data från txtfilen läggs in i detta 
        public static Foretag[] foretagRegister;

      

        static void Main(string[] args)
        {

            while (true) //Huvudmenyn loopas och visas när programmet körs färdig, användaren avbryter själv 
            {
                HamtaData(); // ny data hämtas innan meyn visas upp
                Console.WriteLine("");
                Console.WriteLine("| Välkommen till Företagsregistret");
                Console.WriteLine("| Tryck 1 för att söka upp  ett företag");
                Console.WriteLine("| Tryck 2 för att lägga till ett företag till registret");
                Console.WriteLine("| Tryck 3 för att ta bort ett företag från registret");
                Console.WriteLine("| Tryck 4 för att skriva ut alla företag från registret");
                Console.WriteLine("| Tryck 5 för att skriva ut alla företag från registret sorterade");
                Console.WriteLine("| Tryck 6 för att avsluta!");
                

                Console.WriteLine(" ");
                Console.Write("Ange ditt val: ");

                if (int.TryParse(Console.ReadLine(), out int val)) // kontrollstruktur som tillåter enbart heltal att användas
                {
                    switch (val)
                    {
                        case 1:
                            Console.Clear(); //cleara consolen så att det inte blir mycket text efter flera inmatningar
                            SokForetag();
                            Thread.Sleep(2000);  // sleep gör så att programmet pausas i 2sek innan det går vidare till huvudmenyn
                            break;


                        case 2:
                            Console.Clear();
                            LaggTillForetag();
                            Thread.Sleep(2000);
                            break;


                        case 3:
                            Console.Clear();
                            TaBortForetag();
                            Thread.Sleep(2000);
                            break;

                        case 4:
                            Console.Clear();
                            SkrivUtListan();
                            Thread.Sleep(2000);
                            break;

                        case 5:
                            Console.Clear();
                            SkrivUtListanSorterad();
                            Thread.Sleep(2000);
                            break;

                        case 6:
                            Console.Clear();
                            Console.WriteLine("Tack för du använde programmet!\nVälkommen åter :)");
                            Thread.Sleep(3000);
                            Environment.Exit(0);
                            break;

                        default:
                            Console.WriteLine("Ogiltigt val, försök igen.");
                            Thread.Sleep(1500);
                            Console.Clear();
                            break;

                    }
                }     
            }
        }

        public static void HamtaData()   //Metod för att hämta data för användaresökning

        {
            StreamReader utData = new StreamReader("org.txt");  
            int antalPlatserTxt = File.ReadLines("org.txt").Count(); //läser antal rader i txtfil
            foretagRegister = new Foretag[antalPlatserTxt];
            string rad;
            int i = 0;

            while ((rad = utData.ReadLine()) != null)  // loopar tills raden i txtfilen är tom
            {
                Foretag foretag = new Foretag();
                string[] led = rad.Split("\t"); //tempörar array som sorterar datan innan det läggs till foretagRegister
                foretag.foretagsNamn = led[0];
                foretag.foretagsAdress = led[1];
                foretag.foretagsNummer = led[2]; //string används så 0 i början av numret skrivs ut
                foretagRegister[i] = foretag;   
                i++;

            }
            utData.Close();

        }


        public static void SokForetag()
        {
            Console.Write("Skriv namnet på företaget du vill veta mer om: ");
            string inmattadSvar = Console.ReadLine().ToLower();

            Boolean hittad = false; 
            for (int i = 0; i < foretagRegister.Length; i++)
            {
                if ( foretagRegister[i].foretagsNamn == inmattadSvar) // kontroll om företaget finns med i listan
                {
                    Console.WriteLine("Företags namn: {0} \nAdress: {1} \nTelefonnummer: {2}", foretagRegister[i].foretagsNamn, foretagRegister[i].foretagsAdress, foretagRegister[i].foretagsNummer);
                    hittad = true;
                    return;
                    
                }


            }

            if (hittad == false) // företag finns ej
            {
                Console.WriteLine("Företaget hittades inte. Testa igen genom att klicka på (1) eller klicka på (2) för att lägga till det i registret");
            }

        }

        public static void LaggTillForetag() 
        {

            using (StreamWriter inData = new StreamWriter("org.txt", true)) 
            {
                Console.Write("Skriv namnet på företaget du vill lägga till: ");
                string foretagsNamn = Console.ReadLine().ToLower();

                Console.Write("Skriv företagets adress: ");
                string foretagsAdress = Console.ReadLine().ToLower();

                Console.Write("Skriv företagets telefonnummer: ");
                string foretagsNummer = Console.ReadLine().ToLower(); //används som string då 0 i början av numret ska synnas

                inData.WriteLine("{0}\t{1}\t{2}", foretagsNamn, foretagsAdress, foretagsNummer); //värden i variablerna skrivs över i txtfilen
                inData.Close();
            }
            Console.WriteLine("Företaget är nu tillagt :)");

        }


        public static void TaBortForetag()
        {
            
                int i = 0;
                Boolean foretagFinns = false;

                Console.Write("Skriv namnet på företaget du vill ta bort: ");
                string foretagSomTasBort = Console.ReadLine().ToLower();

                // Läs innehållet från textfilen 
                string[] filRader = File.ReadAllLines("org.txt");

                // array för att lagra rader som ska behållas
                string[] nyFilRader = new string[filRader.Length];

                // Gå igenom varje rad och lägg till dem i den nya arrayen om de inte matchar namnet som ska tas bort
                foreach (string rad in filRader)
                {
                    string[] led = rad.Split("\t");
                    string foretagsNamn = led[0].ToLower();
                    if (foretagsNamn != foretagSomTasBort)
                    {
                        nyFilRader[i] = rad;
                        i++;
                    }
                    else
                    {
                        foretagFinns = true; // om företaget hittas i listan 
                    }
                }

                if (foretagFinns == false)
                {
                    Console.WriteLine("Företaget du vill ta bort finns inte i registret. Kontrollera stavningen och försök igen.");
                    return; 
                }

                // rader som ska behållas skrivs över i textfilen, där den tomma raden inte skrivs in i textfilen
                File.WriteAllLines("org.txt", nyFilRader.Where(x => x != null));

                Console.WriteLine("Företaget är nu borttaget från registret.");

        }

        public static void SkrivUtListan() 
        {
            Console.WriteLine("Alla företag i registret osorterade:");

            string tabell = " | {0,-3} | {1,-15} | {2,-25} | {3,-15} | "; // Justera längden på varje kolumn
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine(tabell, "Nr", "Namn", "Adress", "Nummer");
            Console.WriteLine("------------------------------------------------------------------------");

            for (int i = 0; i < foretagRegister.Length; i++) // går genom listan och matar in värden i tabellen
            {
                Console.WriteLine(tabell, (i+1),  foretagRegister[i].foretagsNamn, foretagRegister[i].foretagsAdress, foretagRegister[i].foretagsNummer);

                
            }
            Console.WriteLine("------------------------------------------------------------------------");

        }


        public static void SkrivUtListanSorterad()
        {
            Console.WriteLine("Alla företag i registret sorterade på företags namn:");

            Array.Sort(foretagRegister, (x,y) => x.sort_ForetagNamn().CompareTo(y.sort_ForetagNamn())); //arraysort för struct på FöretagsNamn

            string tabell = " | {0,-3} | {1,-15} | {2,-25} | {3,-15} | "; 
            Console.WriteLine("------------------------------------------------------------------------");
            Console.WriteLine(tabell, "Nr", "Namn", "Adress", "Nummer");
            Console.WriteLine("------------------------------------------------------------------------");

            for (int i = 0; i < foretagRegister.Length; i++)
            {
                Console.WriteLine(tabell, (i+1), foretagRegister[i].foretagsNamn, foretagRegister[i].foretagsAdress, foretagRegister[i].foretagsNummer);


            }
            Console.WriteLine("------------------------------------------------------------------------");


        }
    }
}


