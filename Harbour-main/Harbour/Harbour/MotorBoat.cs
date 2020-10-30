using System;
using System.Collections.Generic;
using System.Linq;

namespace Harbour
{
    class MotorBoat : Boat
    {
        public int Power { get; set; }

        public MotorBoat(string id, int weight, int maxSpeed, int daysStaying, int daysSinceArrival, int power)
            : base(weight, maxSpeed, daysStaying, daysSinceArrival)
        {
            IdNumber = id;
            Type = "Motorbåt";
            Power = power;
        }


        public override string ToString()
        {
            return $"{Type}\t{IdNumber}\t{Weight}\t{Math.Round(Utils.ConvertKnotToKmPerHour(MaximumSpeed), 0)}" +
                $"\t\tMotoreffekt:\t{Power} hästkrafter";
        }
        public override string TextToFile(int index)
        {
            return base.TextToFile(index) + $"{Power}";
        }

        public static void CreateMotorBoat(List<Boat> boats)
        {
            string id = "M-" + GenerateID();
            int weight = Utils.r.Next(200, 3000 + 1);
            int maxSpeed = Utils.r.Next(60 + 1);
            int daysStaying = 3;
            int daysSinceArrival = 0;
            int power = Utils.r.Next(10, 1000 + 1);

            boats.Add(new MotorBoat(id, weight, maxSpeed, daysStaying, daysSinceArrival, power));
        }

        internal static bool ParkMotorBoatInHarbour(Boat boat, HarbourSpace[] dock1, HarbourSpace[] dock2)
        {
            bool boatParked;

            while (true)
            {
                int selectedSpace;

                (selectedSpace, boatParked) = RowingBoat.FindSingleSpaceBetweenOccupiedSpaces(dock1);
                if (boatParked)
                {
                    dock1[selectedSpace].ParkedBoats.Add(boat);
                    break;
                }

                (selectedSpace, boatParked) = RowingBoat.FindSingleSpaceBetweenOccupiedSpaces(dock2);
                if (boatParked)
                {
                    dock2[selectedSpace].ParkedBoats.Add(boat);
                    break;
                }

                (selectedSpace, boatParked) = RowingBoat.FindFirstFreeSpace(dock1);
                if (boatParked)
                {
                    dock1[selectedSpace].ParkedBoats.Add(boat);
                    break;
                }

                (selectedSpace, boatParked) = RowingBoat.FindFirstFreeSpace(dock2);
                if (boatParked)
                {
                    dock2[selectedSpace].ParkedBoats.Add(boat);
                    break;
                }

                break;
            }

            return boatParked;
        }

       
    }


}