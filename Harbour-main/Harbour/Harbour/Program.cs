using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Harbour
{
    class Program
    {
        static void Main(string[] args)
        {

            var fileText = File.ReadLines("BoatsInHarbour.txt", System.Text.Encoding.UTF7);

            //Console.WriteLine(Utils.PrintTextFromFile(fileText));

            HarbourSpace[] harbour = new HarbourSpace[64];

            for (int i = 0; i < harbour.Length; i++)
            {
                harbour[i] = new HarbourSpace(i);
            }

            AddBoatsFromFileToHarbour(fileText, harbour);

            Console.WriteLine("Båtar i hamn efter uppstart\n");
            Console.WriteLine(PrintHarbour2(harbour));
            Console.WriteLine();

            bool goToNextDay = true;

            while (goToNextDay)
            {
                List<Boat> boatsInHarbour = GenerateBoatsInHarbourList(harbour);
                AddDayToDaysSinceArrival(boatsInHarbour);

                bool boatRemoved = true;

                while (boatRemoved)
                {
                    boatRemoved = RemoveBoats(harbour);
                }

                Console.WriteLine("Båtar i hamn efter dagens avfärder");
                Console.WriteLine(PrintHarbour2(harbour));
                Console.WriteLine();

                int rejectedRowingBoats = 0;
                int rejectedMotorBoats = 0;
                int rejectedSailingBoats = 0;
                int rejectedCatamarans = 0;
                int rejectedCargoShips = 0;

                List<Boat> arrivingBoats = new List<Boat>();
                int NumberOfArrivingBoats = 5;

                CreateNewBoats(arrivingBoats, NumberOfArrivingBoats); // Tar bor tillfälligt, för att kunna styra vilka båtar som läggs till

                // Skapar båtar för test, ta bort sedan
                //arrivingBoats.Add(new MotorBoat("M-" + Boat.GenerateID(), 10, 2, 3, 0, 4));
                //arrivingBoats.Add(new RowingBoat("R-" + Boat.GenerateID(), 10, 2, 1, 0, 4));
                //arrivingBoats.Add(new SailingBoat("S-" + Boat.GenerateID(), 10, 2, 4, 0, 4));
                //arrivingBoats.Add(new CargoShip("L-" + Boat.GenerateID(), 10, 2, 6, 0, 4));
                //arrivingBoats.Add(new RowingBoat("R-" + Boat.GenerateID(), 10, 2, 1, 0, 4));
                //arrivingBoats.Add(new Catamaran("K-" + Boat.GenerateID(), 10, 2, 1, 0, 4));

                Console.WriteLine("Anländande båtar");
                foreach (var boat in arrivingBoats)  //Kontroll, ta bort sedan
                {
                    Console.WriteLine(boat.ToString());
                }
                Console.WriteLine();

                foreach (var boat in arrivingBoats)
                {
                    int harbourPosition;
                    bool spaceFound;

                    if (boat is RowingBoat)
                    {
                        (harbourPosition, spaceFound) = RowingBoat.FindRowingboatSpace(harbour);

                        if (spaceFound)
                        {
                            harbour[harbourPosition].ParkedBoats.Add(boat);
                        }
                        else
                        {
                            rejectedRowingBoats++;
                        }
                    }

                    else if (boat is MotorBoat)
                    {
                        (harbourPosition, spaceFound) = MotorBoat.FindMotorBoatSpace(harbour);

                        if (spaceFound)
                        {
                            harbour[harbourPosition].ParkedBoats.Add(boat);
                        }
                        else
                        {
                            rejectedMotorBoats++;
                        }
                    }

                    else if (boat is SailingBoat)
                    {
                        (harbourPosition, spaceFound) = SailingBoat.FindSailingBoatSpace(harbour);

                        if (spaceFound)
                        {
                            harbour[harbourPosition].ParkedBoats.Add(boat);
                            harbour[harbourPosition + 1].ParkedBoats.Add(boat);
                        }

                        if (spaceFound == false)
                        {
                            rejectedSailingBoats++;
                        }
                    }

                    else if (boat is Catamaran)
                    {
                        (harbourPosition, spaceFound) = Catamaran.FindCatamaranSpace(harbour);

                        if (spaceFound)
                        {
                            harbour[harbourPosition].ParkedBoats.Add(boat);
                            harbour[harbourPosition + 1].ParkedBoats.Add(boat);
                            harbour[harbourPosition + 2].ParkedBoats.Add(boat);
                        }

                        if (spaceFound == false)
                        {
                            rejectedCatamarans++;
                        }
                    }

                    else if (boat is CargoShip)
                    {
                        (harbourPosition, spaceFound) = CargoShip.FindCargoShipSpace(harbour);

                        if (spaceFound)
                        {
                            harbour[harbourPosition].ParkedBoats.Add(boat);
                            harbour[harbourPosition + 1].ParkedBoats.Add(boat);
                            harbour[harbourPosition + 2].ParkedBoats.Add(boat);
                            harbour[harbourPosition + 3].ParkedBoats.Add(boat);
                        }

                        if (spaceFound == false)
                        {
                            rejectedCargoShips++;
                        }
                    }
                }

                Console.WriteLine("Båtar i hamn\n------------\n");
                Console.WriteLine(PrintHarbour2(harbour));
                Console.WriteLine();

                boatsInHarbour = GenerateBoatsInHarbourList(harbour);

                Console.WriteLine(GnerateSummaryOfBoats(boatsInHarbour));

                int sumOfWeight = GenerateSumOfWeight(boatsInHarbour);
                double averageSpeed = GenerateAverageSpeed(boatsInHarbour);
                int availableSpaces = CountAvailableSpaces(harbour);

                Console.WriteLine(PrintStatistics(sumOfWeight, averageSpeed, availableSpaces,
                    rejectedRowingBoats, rejectedMotorBoats, rejectedSailingBoats, rejectedCatamarans, rejectedCargoShips));

                Console.WriteLine();
                Console.WriteLine();

                Console.Write("Tryck \"Q\" för att avsluta eller valfri annan tangent för att gå till nästa dag ");

                ConsoleKey input = Console.ReadKey().Key;

                goToNextDay = input != ConsoleKey.Q;

                Console.WriteLine();
                Console.WriteLine();
            }


            StreamWriter sw = new StreamWriter("BoatsInHarbour.txt", false, System.Text.Encoding.UTF7);

            SaveToFile(sw, harbour);
            sw.Close();

        }

        private static string PrintStatistics(int sumOfWeight, double averageSpeed, int availableSpaces,
            int rejectedRowingBoats, int rejectedMotorBoats, int rejectedSailingBoats, int rejectedCatamarans, int rejectedCargoShips)
        {
            return $"Statistik\n---------\n" +
                $"Total båtvikt i hamn:\t{sumOfWeight} kg\n" +
                $"Medel av maxhastighet:\t{Math.Round(Utils.ConvertKnotToKmPerHour(averageSpeed), 1)} km/h\n" +
                $"Lediga platser:\t\t{availableSpaces} st\n\n" +
                $"Avvisade båtar:\n" +
                $"\tRoddbåtar\t{rejectedRowingBoats} st\n" +
                $"\tMotorbåtar\t{rejectedMotorBoats} st\n" +
                $"\tSegelbåtar\t{rejectedSailingBoats} st\n" +
                $"\tKatamaraner\t{rejectedCatamarans} st\n" +
                $"\tLastfartyg\t{rejectedCargoShips} st\n" +
                $"\tTotalt\t\t{rejectedRowingBoats + rejectedMotorBoats + rejectedSailingBoats + rejectedCatamarans + rejectedCargoShips} st";
        }

        private static int CountAvailableSpaces(HarbourSpace[] harbour)
        {
            var q = harbour
                .Where(s => s.ParkedBoats.Count() == 0);

            return q.Count();
        }

        private static int GenerateSumOfWeight(List<Boat> boatsInHarbour)
        {
            var q = boatsInHarbour
                .Select(b => b.Weight)
                .Sum();

            return q;
        }

        private static double GenerateAverageSpeed(List<Boat> boatsInHarbour)
        {
            var q = boatsInHarbour
                .Select(b => b.MaximumSpeed)
                .Average();

            return q;
        }

        private static string GnerateSummaryOfBoats(List<Boat> boatsInHarbour)
        {
            string summaryOfBoats = "Summering av båtar i hamn\n-------------------------\n";

            var q = boatsInHarbour
                    .GroupBy(b => b.Type);
            /*.OrderBy(g => g.Key)*/  //Vill inte ha bokstavsordning utan bestämma ordningen själv

            foreach (var group in q)
            {
                if (group.Key == "Roddbåt")
                {
                    summaryOfBoats += $"{group.Key}:\t{group.Count()} st\n";
                    break;
                }
            }

            foreach (var group in q)
            {
                if (group.Key == "Motorbåt")
                {
                    summaryOfBoats += $"{group.Key}:\t{group.Count()} st\n";
                    break;
                }
            }

            foreach (var group in q)
            {
                if (group.Key == "Segelbåt")
                {
                    summaryOfBoats += $"{group.Key}:\t{group.Count()} st\n";
                    break;
                }
            }

            foreach (var group in q)
            {
                if (group.Key == "Katamaran")
                {
                    summaryOfBoats += $"{group.Key}:\t{group.Count()} st\n";
                    break;
                }
            }

            foreach (var group in q)
            {
                if (group.Key == "Lastfartyg")
                {
                    summaryOfBoats += $"{group.Key}:\t{group.Count()} st\n";
                    break;
                }
            }

            return summaryOfBoats;
        }

        private static bool RemoveBoats(HarbourSpace[] harbour)
        {
            bool boatRemoved = false;

            foreach (HarbourSpace space in harbour)
            {
                foreach (Boat boat in space.ParkedBoats)
                {
                    if (boat.DaysSinceArrival == boat.DaysStaying)
                    {
                        space.ParkedBoats.Remove(boat);
                        boatRemoved = true;
                        break;
                    }
                }
                if (boatRemoved)
                {
                    break;
                }
            }

            return boatRemoved;
        }

        private static List<Boat> GenerateBoatsInHarbourList(HarbourSpace[] harbour)
        {

            // Större båtar finns på flera platser i harbour, gör lista med endast en kopia av vardera båt
            var q1 = harbour
                .Where(h => h.ParkedBoats.Count != 0);

            List<Boat> allCopies = new List<Boat>();

            foreach (var space in q1)
            {
                foreach (var boat in space.ParkedBoats)
                {
                    allCopies.Add(boat); // Innehåller kopior
                }
            }

            var q2 = allCopies
                .GroupBy(b => b.IdNumber);

            List<Boat> singleBoats = new List<Boat>();

            foreach (var group in q2)
            {
                var q = group
                    .FirstOrDefault();

                singleBoats.Add(q);  // Lista utan kopior
            }

            return singleBoats;
        }

        private static void AddDayToDaysSinceArrival(List<Boat> boats)
        {
            foreach (var boat in boats)
            {
                boat.DaysSinceArrival++;
            }
        }

        private static string PrintHarbour2(HarbourSpace[] harbour)
        {
            string text = "Båtplats\tBåttyp\t\tID\tVikt\tMaxhastighet\tÖvrigt\n" +
                          "        \t      \t\t  \t(kg)\t(km/h)\n" +
                          "--------\t----------\t-----\t-----\t------------\t------------------------------\n";

            foreach (var space in harbour)
            {
                if (space.ParkedBoats.Count() == 0)
                {
                    text += $"{space.SpaceId + 1}  Ledigt\n";
                }
                foreach (var boat in space.ParkedBoats)
                {
                    if (space.SpaceId > 0 && harbour[space.SpaceId - 1].ParkedBoats.Contains(boat))
                    {
                        text += $""; // -> gör ingenting
                    }

                    else
                    {
                        if (boat is RowingBoat || boat is MotorBoat)
                        {
                            text += $"{space.SpaceId + 1}\t\t{boat}\n";
                        }
                        else if (boat is SailingBoat)
                        {
                            text += $"{space.SpaceId + 1}-{space.SpaceId + 2}\t\t{boat}\n";
                        }
                        else if (boat is Catamaran)
                        {
                            text += $"{space.SpaceId + 1}-{space.SpaceId + 3}\t\t{boat}\n";
                        }
                        else if (boat is CargoShip)
                        {
                            text += $"{space.SpaceId + 1}-{space.SpaceId + 4}\t\t{boat}\n";
                        }
                    }
                }
            }

            return text;
        }

        private static void AddBoatsFromFileToHarbour(IEnumerable<string> fileText, HarbourSpace[] harbour)
        {
            // File:
            // index; Id; Weight; MaxSpeed; Type; DaysStaying; DaySinceArrival; Special
            // 0      1   2       3         4     5            6                7


            foreach (var line in fileText)
            {
                int index;
                string[] boatData = line.Split(";");

                switch (boatData[4])
                {
                    case "Roddbåt":
                        index = int.Parse(boatData[0]);
                        harbour[index].ParkedBoats.Add
                            (new RowingBoat(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                            int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7])));
                        break;

                    case "Motorbåt":
                        index = int.Parse(boatData[0]);
                        harbour[index].ParkedBoats.Add
                            (new MotorBoat(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                            int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7])));
                        break;

                    case "Segelbåt":
                        index = int.Parse(boatData[0]);

                        if (harbour[index].ParkedBoats.Count == 0) // När andra halvan av segelbåten kommmer från foreach är den redan tillagd på den platsen annars hade det blivit två kopior av samma båt
                        {
                            SailingBoat sailingBoat = new SailingBoat(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                                int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7]));

                            harbour[index].ParkedBoats.Add(sailingBoat);
                            harbour[index + 1].ParkedBoats.Add(sailingBoat); // samma båt på två platser
                        }
                        break;

                    case "Katamaran":
                        index = int.Parse(boatData[0]);

                        if (harbour[index].ParkedBoats.Count == 0) // När resten av lastfartyget kommmer från foreach är det redan tillagt, annars hade det blivit kopior
                        {
                            Catamaran catamaran = new Catamaran(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                            int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7]));

                            harbour[index].ParkedBoats.Add(catamaran);
                            harbour[index + 1].ParkedBoats.Add(catamaran);
                            harbour[index + 2].ParkedBoats.Add(catamaran);
                        }
                        break;

                    case "Lastfartyg":
                        index = int.Parse(boatData[0]);

                        if (harbour[index].ParkedBoats.Count == 0) // När resten av lastfartyget kommmer från foreach är det redan tillagt, annars hade det blivit kopior
                        {
                            CargoShip cargoship = new CargoShip(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                            int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7]));

                            harbour[index].ParkedBoats.Add(cargoship);
                            harbour[index + 1].ParkedBoats.Add(cargoship);
                            harbour[index + 2].ParkedBoats.Add(cargoship);
                            harbour[index + 3].ParkedBoats.Add(cargoship);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public static double ConvertToKmPerHour(double knot)
        {
            return knot * 1.852;
        }

        private static void SaveToFile(StreamWriter sw, HarbourSpace[] harbour)
        {
            int index = 0;
            foreach (var space in harbour)
            {
                if (space != null)
                {
                    foreach (Boat boat in space.ParkedBoats)
                    {
                        if (space.ParkedBoats != null)
                        {
                            sw.WriteLine(boat.TextToFile(index), System.Text.Encoding.UTF7);
                        }
                    }
                }

                index++;
            }

            sw.Close();
        }

        private static void CreateNewBoats(List<Boat> boats, int newBoats)
        {
            for (int i = 0; i < newBoats; i++)
            {
                int boatType = Utils.r.Next(4 + 1);

                switch (boatType)
                {
                    case 0:
                        RowingBoat.CreateRowingBoat(boats);
                        break;
                    case 1:
                        MotorBoat.CreateMotorBoat(boats);
                        break;
                    case 2:
                        SailingBoat.CreateSailingBoat(boats);
                        break;
                    case 3:
                        Catamaran.CreateCatamaran(boats);
                        break;
                    case 4:
                        CargoShip.CreateCargoShip(boats);
                        break;
                }
            }
        }
    }
}
