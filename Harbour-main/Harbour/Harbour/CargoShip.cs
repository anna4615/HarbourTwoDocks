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

        internal static bool ParkCargoshipInHarbour(Boat boat, HarbourSpace[] dock1, HarbourSpace[] dock2)
        {
            bool boatParked;

            while (true)
            {
                int selectedSpace;

                (selectedSpace, boatParked) = FindFourSpacesBetweenOccupiedSpaces(dock1);
                if (boatParked)
                {
                    dock1[selectedSpace].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 1].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 2].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 3].ParkedBoats.Add(boat);
                    break;
                }

                (selectedSpace, boatParked) = FindFourSpacesBetweenOccupiedSpaces(dock2);
                if (boatParked)
                {
                    dock2[selectedSpace].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 1].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 2].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 3].ParkedBoats.Add(boat);
                    break;
                }

                (selectedSpace, boatParked) = FindFirstFourFreeSpaces(dock1);
                if (boatParked)
                {
                    dock1[selectedSpace].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 1].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 2].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 3].ParkedBoats.Add(boat);
                    break;
                }

                (selectedSpace, boatParked) = FindFirstFourFreeSpaces(dock2);
                if (boatParked)
                {
                    dock2[selectedSpace].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 1].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 2].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 3].ParkedBoats.Add(boat);
                    break;
                }

                break;
            }

            return boatParked;
        }

        private static (int selectedSpace, bool spaceFound) FindFourSpacesBetweenOccupiedSpaces(HarbourSpace[] dock)
        {
            int selectedSpace = 0;
            bool spaceFound = false;

            // Om index 0-3 är ledigt och index 4 upptaget
            if (dock[0].ParkedBoats.Count == 0
                && dock[1].ParkedBoats.Count == 0
                && dock[2].ParkedBoats.Count == 0
                && dock[3].ParkedBoats.Count == 0
                && dock[4].ParkedBoats.Count > 0)
            {
                selectedSpace = 0;
                spaceFound = true;
            }

            // Annars, hitta fyra lediga platser intill varandra med upptagna platser runtom
            if (spaceFound == false)
            {
                var q = dock
                    .FirstOrDefault(h => h.ParkedBoats.Count == 0
                    && h.SpaceId > 0
                    && h.SpaceId < dock.Length - 4
                    && dock[h.SpaceId + 1].ParkedBoats.Count == 0
                    && dock[h.SpaceId + 2].ParkedBoats.Count == 0
                    && dock[h.SpaceId + 3].ParkedBoats.Count == 0
                    && dock[h.SpaceId - 1].ParkedBoats.Count > 0
                    && dock[h.SpaceId + 4].ParkedBoats.Count > 0);

                if (q != null)
                {
                    selectedSpace = q.SpaceId;
                    spaceFound = true;
                }

                // Annars, om fyra sista index är ledigt och index innan upptaget
                if (spaceFound == false)
                {
                    if (dock[dock.Length - 4].ParkedBoats.Count == 0
                        && dock[dock.Length - 3].ParkedBoats.Count == 0
                        && dock[dock.Length - 2].ParkedBoats.Count == 0
                        && dock[dock.Length - 1].ParkedBoats.Count == 0
                        && dock[dock.Length - 5].ParkedBoats.Count > 0)
                    {
                        selectedSpace = dock.Length - 4;
                        spaceFound = true;
                    }
                }
            }

            return (selectedSpace, spaceFound);
        }

        private static (int selectedSpace, bool spaceFound) FindFirstFourFreeSpaces(HarbourSpace[] dock)
        {
            int selectedSpace = 0;
            bool spaceFound = false;

            // Hitta första fyra lediga platser intill varandra
            if (spaceFound == false)
            {
                var q = dock
                   .FirstOrDefault(h => h.ParkedBoats.Count == 0
                   && h.SpaceId < dock.Length - 3
                   && dock[h.SpaceId + 1].ParkedBoats.Count == 0
                   && dock[h.SpaceId + 2].ParkedBoats.Count == 0
                   && dock[h.SpaceId + 3].ParkedBoats.Count == 0);

                if (q != null)
                {
                    selectedSpace = q.SpaceId;
                    spaceFound = true;
                }
            }

            return (selectedSpace, spaceFound);
        }
    }
}
