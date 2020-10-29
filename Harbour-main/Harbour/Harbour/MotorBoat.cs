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

        public static (int, bool) FindMotorBoatSpace(HarbourSpace[] harbour)
        {
            int selectedSpace = 0;
            bool spaceFound = false;

            // Om index 0 är ledigt och index 1 upptaget
            if (harbour[0].ParkedBoats.Count == 0 && harbour[1].ParkedBoats.Count > 0)
            {
                selectedSpace = 0;
                spaceFound = true;
            }

            // Annars, hitta ensam plats med upptagna platser runtom
            if (spaceFound == false)
            {
                var q1 = harbour
                    .FirstOrDefault(h => h.ParkedBoats.Count == 0
                    && h.SpaceId > 0
                    && h.SpaceId < harbour.Length - 2
                    && harbour[h.SpaceId - 1].ParkedBoats.Count > 0
                    && harbour[h.SpaceId + 1].ParkedBoats.Count > 0);

                if (q1 != null)
                {
                    selectedSpace = q1.SpaceId;
                    spaceFound = true;
                }
            }

            // Annars, hitta första lediga plats
            if (spaceFound == false)
            {
                var q2 = harbour
                   .FirstOrDefault(h => h.ParkedBoats.Count == 0);

                if (q2 != null)
                {
                    selectedSpace = q2.SpaceId;
                    spaceFound = true;
                }
            }

            return (selectedSpace, spaceFound);
        }
    }


}