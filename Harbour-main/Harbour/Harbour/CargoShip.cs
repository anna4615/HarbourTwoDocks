using System;
using System.Collections.Generic;
using System.Linq;

namespace Harbour
{
    class CargoShip : Boat
    {
        public int Containers { get; set; }

        public CargoShip(string id, int weight, int maxSpeed, int daysStaying, int daysSinceArrival, int containers)
            : base(weight, maxSpeed, daysStaying, daysSinceArrival)
        {
            IdNumber = id;
            Type = "Lastfartyg";
            Containers = containers;
        }
        public override string ToString()
        {
            return $"{Type}\t{IdNumber}\t{Weight}\t{Math.Round(Utils.ConvertKnotToKmPerHour(MaximumSpeed), 0)}\t\tContainers:\t{Containers} stycken";
        }
        public override string TextToFile(int index)
        {
            return base.TextToFile(index) + $"{Containers}";
        }
        public static void CreateCargoShip(List<Boat> boats)
        {
            string id = "L-" + GenerateID();
            int weight = Utils.r.Next(3000, 20000 + 1);
            int maxSpeed = Utils.r.Next(20 + 1);
            int daysStaying = 6;
            int daysSinceArrival = 0;
            int containers = Utils.r.Next(500 + 1);

            boats.Add(new CargoShip(id, weight, maxSpeed, daysStaying, daysSinceArrival, containers));
        }

        public static (int, bool) FindCargoShipSpace(HarbourSpace[] harbour)
        {
            int selectedSpace = 0;
            bool spaceFound = false;

            // Om index 0-3 är ledigt och index 4 upptaget
            if (harbour[0].ParkedBoats.Count == 0
                && harbour[1].ParkedBoats.Count == 0
                && harbour[2].ParkedBoats.Count == 0
                && harbour[3].ParkedBoats.Count == 0
                && harbour[4].ParkedBoats.Count > 0)
            {
                selectedSpace = 0;
                spaceFound = true;
            }

            // Annars, hitta fyra lediga platser intill varandra med upptagna platser runtom
            if (spaceFound == false)
            {
                var q1 = harbour
                    .FirstOrDefault(h => h.ParkedBoats.Count == 0
                    && h.SpaceId > 0
                    && h.SpaceId < harbour.Length - 4
                    && harbour[h.SpaceId + 1].ParkedBoats.Count == 0
                    && harbour[h.SpaceId + 2].ParkedBoats.Count == 0
                    && harbour[h.SpaceId + 3].ParkedBoats.Count == 0
                    && harbour[h.SpaceId - 1].ParkedBoats.Count > 0
                    && harbour[h.SpaceId + 4].ParkedBoats.Count > 0);

                if (q1 != null)
                {
                    selectedSpace = q1.SpaceId;
                    spaceFound = true;
                }
            }

            // Annars hitta första fyra lediga platser intill varandra
            if (spaceFound == false)
            {
                var q2 = harbour
                   .FirstOrDefault(h => h.ParkedBoats.Count == 0
                   && h.SpaceId < harbour.Length - 3
                   && harbour[h.SpaceId + 1].ParkedBoats.Count == 0
                   && harbour[h.SpaceId + 2].ParkedBoats.Count == 0
                   && harbour[h.SpaceId + 3].ParkedBoats.Count == 0);

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
