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

            var fileText = File.ReadLines("BoatsInDock1.txt", System.Text.Encoding.UTF7);

            Console.WriteLine(Utils.PrintTextFromFile(fileText));

            HarbourSpace[] dock1 = new HarbourSpace[32];
            for (int i = 0; i < dock1.Length; i++)
            {
                dock1[i] = new HarbourSpace(i);
            }
            AddBoatsFromFileToHarbour(fileText, dock1);

            fileText = File.ReadLines("BoatsInDock2.txt", System.Text.Encoding.UTF7);

            HarbourSpace[] dock2 = new HarbourSpace[32];
            for (int i = 0; i < dock2.Length; i++)
            {
                dock2[i] = new HarbourSpace(i);
            }
            AddBoatsFromFileToHarbour(fileText, dock2);

            Console.WriteLine("Båtar i hamn efter uppstart\n");
            Console.WriteLine("Kaj 1");
            Console.WriteLine(PrintHarbour(dock1));
            Console.WriteLine();
            Console.WriteLine("Kaj 2");
            Console.WriteLine(PrintHarbour(dock2));
            Console.WriteLine();
            Console.WriteLine();

            bool goToNextDay = true;

            while (goToNextDay)
            {
                List<Boat> boatsInDock1 = GenerateBoatsInHarbourList(dock1);
                AddDayToDaysSinceArrival(boatsInDock1);

                List<Boat> boatsInDock2 = GenerateBoatsInHarbourList(dock2);
                AddDayToDaysSinceArrival(boatsInDock2);

                bool boatRemoved = true;
                while (boatRemoved)
                {
                    boatRemoved = RemoveBoats(dock1);
                }
                boatRemoved = true;
                while (boatRemoved)
                {
                    boatRemoved = RemoveBoats(dock2);
                }

                Console.WriteLine("Båtar i hamn efter dagens avfärder");
                Console.WriteLine("Kaj 1");
                Console.WriteLine(PrintHarbour(dock1));
                Console.WriteLine();
                Console.WriteLine("Kaj 2");
                Console.WriteLine(PrintHarbour(dock2));
                Console.WriteLine();

                int rejectedRowingBoats = 0;
                int rejectedMotorBoats = 0;
                int rejectedSailingBoats = 0;
                int rejectedCatamarans = 0;
                int rejectedCargoShips = 0;

                List<Boat> arrivingBoats = new List<Boat>();
                int NumberOfArrivingBoats = 5;

                CreateNewBoats(arrivingBoats, NumberOfArrivingBoats); // Tar bor tillfälligt, för att kunna styra vilka båtar som läggs till

                //        // Skapar båtar för test, ta bort sedan
                //arrivingBoats.Add(new MotorBoat("M-" + Boat.GenerateID(), 10, 2, 3, 0, 4));
                //arrivingBoats.Add(new RowingBoat("R-" + Boat.GenerateID(), 10, 2, 1, 0, 4));
                //arrivingBoats.Add(new SailingBoat("S-" + Boat.GenerateID(), 10, 2, 4, 0, 4));
                //arrivingBoats.Add(new CargoShip("L-" + Boat.GenerateID(), 10, 2, 6, 0, 4));
                //arrivingBoats.Add(new RowingBoat("R-" + Boat.GenerateID(), 10, 2, 1, 0, 4));
                //arrivingBoats.Add(new Catamaran("K-" + Boat.GenerateID(), 10, 2, 1, 0, 4));

                Console.WriteLine("Anländande båtar");
                Console.WriteLine("----------------");
                Console.WriteLine( "Båttyp\t\tID\tVikt\tMaxhastighet\tÖvrigt\n" +
                                   "      \t\t  \t(kg)\t(km/h)\n" +
                                   "----------\t-----\t-----\t------------\t------------------------------\n");
                foreach (var boat in arrivingBoats)  //Kontroll, ta bort sedan
                {
                    Console.WriteLine($"{boat}");
                }
                Console.WriteLine();
                Console.WriteLine();

                foreach (var boat in arrivingBoats)
                {
                    bool boatParked;

                    if (boat is RowingBoat)
                    {
                        boatParked = RowingBoat.ParkRowingBoatInHarbour(boat, dock1, dock2);

                        if (boatParked == false)
                        {
                            rejectedRowingBoats++;
                        }
                    }

                    else if (boat is MotorBoat)
                    {
                        boatParked = MotorBoat.ParkMotorBoatInHarbour(boat, dock1, dock2);

                        if (boatParked == false)
                        {
                            rejectedRowingBoats++;
                        }
                    }

                    else if (boat is SailingBoat)
                    {
                        boatParked = SailingBoat.ParkSailingBoatInHarbour(boat, dock1, dock2);

                        if (boatParked == false)
                        {
                            rejectedSailingBoats++;
                        }
                    }

                    else if (boat is Catamaran)
                    {
                        boatParked = Catamaran.ParkCatamaranInHarbour(boat, dock1, dock2);

                        if (boatParked == false)
                        {
                            rejectedCatamarans++;
                        }
                    }

                    else if (boat is CargoShip)
                    {
                        boatParked = CargoShip.ParkCargoshipInHarbour(boat, dock1, dock2);

                        if (boatParked == false)
                        {
                            rejectedCargoShips++;
                        }
                    }
                }

                Console.WriteLine("Båtar i hamn\n------------\n");
                Console.WriteLine("Kaj 1");
                Console.WriteLine(PrintHarbour(dock1));
                Console.WriteLine();
                Console.WriteLine("Kaj 2");
                Console.WriteLine(PrintHarbour(dock2));
                Console.WriteLine();
                Console.WriteLine();

                boatsInDock1 = GenerateBoatsInHarbourList(dock1);
                boatsInDock2 = GenerateBoatsInHarbourList(dock2);

                var boatsInBothDocks = boatsInDock1
                    .Concat(boatsInDock2);

                Console.WriteLine(GnerateSummaryOfBoats(boatsInBothDocks));

                int sumOfWeight = GenerateSumOfWeight(boatsInBothDocks);
                double averageSpeed = GenerateAverageSpeed(boatsInBothDocks);
                int availableSpacesDock1 = CountAvailableSpaces(dock1);
                int availableSpacesDock2 = CountAvailableSpaces(dock2);

                Console.WriteLine(PrintStatistics(sumOfWeight, averageSpeed, availableSpacesDock1, availableSpacesDock2,
                    rejectedRowingBoats, rejectedMotorBoats, rejectedSailingBoats, rejectedCatamarans, rejectedCargoShips));

                Console.WriteLine();
                Console.WriteLine();

                Console.Write("Tryck \"Q\" för att avsluta eller valfri annan tangent för att gå till nästa dag ");

                ConsoleKey input = Console.ReadKey().Key;

                goToNextDay = input != ConsoleKey.Q;

                Console.WriteLine();
                Console.WriteLine();
            }


            StreamWriter sw1 = new StreamWriter("BoatsInDock1.txt", false, System.Text.Encoding.UTF7);
            SaveToFile(sw1, dock1);
            sw1.Close();

            StreamWriter sw2 = new StreamWriter("BoatsInDock2.txt", false, System.Text.Encoding.UTF7);
            SaveToFile(sw2, dock2);
            sw2.Close();

        }

        private static string PrintStatistics(int sumOfWeight, double averageSpeed, int availableSpacesDock1, int availableSpacesDock2,
            int rejectedRowingBoats, int rejectedMotorBoats, int rejectedSailingBoats, int rejectedCatamarans, int rejectedCargoShips)
        {
            return $"Statistik\n---------\n" +
                $"Total båtvikt i hamn:\t\t{sumOfWeight} kg\n" +
                $"Medel av maxhastighet:\t\t{Math.Round(Utils.ConvertKnotToKmPerHour(averageSpeed), 1)} km/h\n" +
                $"Lediga platser vid kaj 1:\t{availableSpacesDock1} st\n" +
                $"Lediga platser vid kaj 2:\t{availableSpacesDock2} st\n\n" +
                $"Avvisade båtar:\n" +
                $"\tRoddbåtar\t{rejectedRowingBoats} st\n" +
                $"\tMotorbåtar\t{rejectedMotorBoats} st\n" +
                $"\tSegelbåtar\t{rejectedSailingBoats} st\n" +
                $"\tKatamaraner\t{rejectedCatamarans} st\n" +
                $"\tLastfartyg\t{rejectedCargoShips} st\n" +
                $"\tTotalt\t\t{rejectedRowingBoats + rejectedMotorBoats + rejectedSailingBoats + rejectedCatamarans + rejectedCargoShips} st";
        }

        private static int CountAvailableSpaces(HarbourSpace[] dock)
        {
            var q = dock
                .Where(s => s.ParkedBoats.Count() == 0);

            return q.Count();
        }

        private static int GenerateSumOfWeight(IEnumerable<Boat> boatsInHarbour)
        {
            var q = boatsInHarbour
                .Select(b => b.Weight)
                .Sum();

            return q;
        }

        private static double GenerateAverageSpeed(IEnumerable<Boat> boatsInHarbour)
        {
            var q = boatsInHarbour
                .Select(b => b.MaximumSpeed)
                .Average();

            return q;
        }

        private static string GnerateSummaryOfBoats(IEnumerable<Boat> boatsInHarbour)
        {
            string summaryOfBoats = "Summering av båtar i hamn\n-------------------------\n";

            var q = boatsInHarbour
                .GroupBy(b => b.Type)
                .OrderBy(g => g.Key);

            foreach (var group in q)
            {
                summaryOfBoats += $"{group.Key}:\t{group.Count()} st\n";
            }

            return summaryOfBoats;
        }

        private static bool RemoveBoats(HarbourSpace[] dock)
        {
            bool boatRemoved = false;

            foreach (HarbourSpace space in dock)
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

        private static List<Boat> GenerateBoatsInHarbourList(HarbourSpace[] dock)
        {

            // Större båtar finns på flera platser i harbour, gör lista med endast en kopia av vardera båt
            var q1 = dock
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

        private static string PrintHarbour(HarbourSpace[] dock)
        {
            string text = "Båtplats\tBåttyp\t\tID\tVikt\tMaxhastighet\tÖvrigt\n" +
                          "        \t      \t\t  \t(kg)\t(km/h)\n" +
                          "--------\t----------\t-----\t-----\t------------\t------------------------------\n";

            foreach (var space in dock)
            {
                if (space.ParkedBoats.Count() == 0)
                {
                    text += $"{space.SpaceId + 1}  Ledigt\n";
                }
                foreach (var boat in space.ParkedBoats)
                {
                    if (space.SpaceId > 0 && dock[space.SpaceId - 1].ParkedBoats.Contains(boat))
                    {
                        text += $""; // Samma båt som på space innan -> skriv ingenting
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

        private static void AddBoatsFromFileToHarbour(IEnumerable<string> fileText, HarbourSpace[] dock)
        {
            // File:
            // spaceId;Id;Weight;MaxSpeed;Type;DaysStaying;DaySinceArrival;Special
            // 0       1  2      3        4    5           6               7

            foreach (var line in fileText)
            {
                int index;
                string[] boatData = line.Split(";");

                switch (boatData[4])
                {
                    case "Roddbåt":
                        index = int.Parse(boatData[0]);
                        dock[index].ParkedBoats.Add
                            (new RowingBoat(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                            int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7])));
                        break;

                    case "Motorbåt":
                        index = int.Parse(boatData[0]);
                        dock[index].ParkedBoats.Add
                            (new MotorBoat(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                            int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7])));
                        break;

                    case "Segelbåt":
                        index = int.Parse(boatData[0]);
                        if (dock[index].ParkedBoats.Count == 0) // När andra halvan av segelbåten kommmer från foreach är den redan tillagd på den platsen annars hade det blivit två kopior av samma båt
                        {
                            SailingBoat sailingBoat = new SailingBoat(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                                int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7]));

                            dock[index].ParkedBoats.Add(sailingBoat);
                            dock[index + 1].ParkedBoats.Add(sailingBoat); // samma båt på två platser
                        }
                        break;

                    case "Katamaran":
                        index = int.Parse(boatData[0]);
                        if (dock[index].ParkedBoats.Count == 0) // När resten av lastfartyget kommmer från foreach är det redan tillagt, annars hade det blivit kopior
                        {
                            Catamaran catamaran = new Catamaran(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                            int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7]));

                            dock[index].ParkedBoats.Add(catamaran);
                            dock[index + 1].ParkedBoats.Add(catamaran);
                            dock[index + 2].ParkedBoats.Add(catamaran);
                        }
                        break;

                    case "Lastfartyg":
                        index = int.Parse(boatData[0]);
                        if (dock[index].ParkedBoats.Count == 0) // När resten av lastfartyget kommmer från foreach är det redan tillagt, annars hade det blivit kopior
                        {
                            CargoShip cargoship = new CargoShip(boatData[1], int.Parse(boatData[2]), int.Parse(boatData[3]),
                            int.Parse(boatData[5]), int.Parse(boatData[6]), int.Parse(boatData[7]));

                            dock[index].ParkedBoats.Add(cargoship);
                            dock[index + 1].ParkedBoats.Add(cargoship);
                            dock[index + 2].ParkedBoats.Add(cargoship);
                            dock[index + 3].ParkedBoats.Add(cargoship);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private static void SaveToFile(StreamWriter sw, HarbourSpace[] harbour)
        {
            //int index = 0;
            foreach (var space in harbour)
            {
                if (space != null)
                {
                    foreach (Boat boat in space.ParkedBoats)
                    {
                        if (space.ParkedBoats != null)
                        {
                            sw.WriteLine(boat.TextToFile(space.SpaceId), System.Text.Encoding.UTF7);
                        }
                    }
                }
                //index++;
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
