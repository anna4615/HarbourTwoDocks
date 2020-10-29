using System;
using System.Collections.Generic;
using System.Linq;

namespace Harbour
{
    class SailingBoat : Boat
    {
        public int Length { get; set; }

        public SailingBoat(string id, int weight, int maxSpeed, int daysStaying, int daysSinceArrival, int length)
            : base(weight, maxSpeed, daysStaying, daysSinceArrival)
        {
            IdNumber = id;
            Type = "Segelbåt";
            Length = length;
        }

        public override string ToString()
        {
            return $"{Type}\t{IdNumber}\t{Weight}\t{Math.Round(Utils.ConvertKnotToKmPerHour(MaximumSpeed), 0)}\t\t" +
                $"Längd:\t\t{Math.Round(Utils.ConvertFeetToMeter(Length))} meter";
        }
        public override string TextToFile(int index)
        {
            return base.TextToFile(index) + $"{Length}";
        }

        public static void CreateSailingBoat(List<Boat> boats)
        {
            string id = "S-" + GenerateID();
            int weight = Utils.r.Next(800, 6000 + 1);
            int maxSpeed = Utils.r.Next(12 + 1);
            int daysStaying = 4;
            int daysSinceArrival = 0;
            int length = Utils.r.Next(10, 60 + 1);

            boats.Add(new SailingBoat(id, weight, maxSpeed, daysStaying, daysSinceArrival, length));
        }

        public static (int, bool) FindSailingBoatSpace(HarbourSpace[] harbour)
        {
            int selectedSpace = 0;
            bool spaceFound = false;

            // Om index 0 och 1 är ledigt och index 2 upptaget
            if (harbour[0].ParkedBoats.Count == 0 && harbour[1].ParkedBoats.Count == 0
                && harbour[2].ParkedBoats.Count > 0)
            {
                selectedSpace = 0;
                spaceFound = true;
            }

            //Annars, hitta två lediga platser intill varandra med upptagna platser runtom
            if (spaceFound == false)
            {
                var q1 = harbour
                    .FirstOrDefault(h => h.ParkedBoats.Count == 0
                    && h.SpaceId > 0
                    && h.SpaceId < harbour.Length - 2
                    && harbour[h.SpaceId + 1].ParkedBoats.Count == 0
                    && harbour[h.SpaceId - 1].ParkedBoats.Count > 0
                    && harbour[h.SpaceId + 2].ParkedBoats.Count > 0);

                if (q1 != null)
                {
                    selectedSpace = q1.SpaceId;
                    spaceFound = true;
                }
            }

            // Annars, hitta första två lediga intill varandra
            if (spaceFound == false)
            {
                var q2 = harbour
                   .FirstOrDefault(h => h.ParkedBoats.Count == 0
                   && h.SpaceId < harbour.Length - 1
                   && harbour[h.SpaceId + 1].ParkedBoats.Count == 0);

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
