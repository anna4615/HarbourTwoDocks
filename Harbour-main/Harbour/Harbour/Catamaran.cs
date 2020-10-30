using System;
using System.Collections.Generic;
using System.Linq;

namespace Harbour
{
    class Catamaran : Boat
    {
        public int Beds { get; set; }
        public Catamaran(string id, int weight, int maxSpeed, int daysStaying, int daysSinceArrival, int beds)
            : base(weight, maxSpeed, daysStaying, daysSinceArrival)
        {
            Type = "Katamaran";
            IdNumber = id;
            Beds = beds;
        }

        public override string ToString()
        {
            return $"{Type}\t{IdNumber}\t{Weight}\t{Math.Round(Utils.ConvertKnotToKmPerHour(MaximumSpeed), 0)}" +
                $"\t\tSängplatser:\t{Beds} stycken";
        }

        public override string TextToFile(int index)
        {
            return base.TextToFile(index) + $"{Beds}";
        }

        public static void CreateCatamaran(List<Boat> boats)
        {
            string id = "K-" + GenerateID();
            int weight = Utils.r.Next(1200, 8000 + 1);
            int maxSpeed = Utils.r.Next(12 + 1);
            int daysStaying = 3;
            int daysSinceArrival = 0;
            int beds = Utils.r.Next(1, 4 + 1);

            boats.Add(new Catamaran(id, weight, maxSpeed, daysStaying, daysSinceArrival, beds));
        }

        internal static bool ParkCatamaranInHarbour(Boat boat, HarbourSpace[] dock1, HarbourSpace[] dock2)
        {
            bool boatParked;

            while (true)
            {
                int selectedSpace;

                (selectedSpace, boatParked) = FindTripleSpaceBetweenOccupiedSpaces(dock1);
                if (boatParked)
                {
                    dock1[selectedSpace].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 1].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 2].ParkedBoats.Add(boat);
                    break;
                }

                (selectedSpace, boatParked) = FindTripleSpaceBetweenOccupiedSpaces(dock2);
                if (boatParked)
                {
                    dock2[selectedSpace].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 1].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 2].ParkedBoats.Add(boat);
                    break;
                }

                (selectedSpace, boatParked) = FindFirstThreeFreeSpaces(dock1);
                if (boatParked)
                {
                    dock1[selectedSpace].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 1].ParkedBoats.Add(boat);
                    dock1[selectedSpace + 2].ParkedBoats.Add(boat);
                    break;
                }

                (selectedSpace, boatParked) = FindFirstThreeFreeSpaces(dock2);
                if (boatParked)
                {
                    dock2[selectedSpace].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 1].ParkedBoats.Add(boat);
                    dock2[selectedSpace + 2].ParkedBoats.Add(boat);
                    break;
                }

                break;
            }

            return boatParked;
        }

        private static (int selectedSpace, bool spaceFound) FindTripleSpaceBetweenOccupiedSpaces(HarbourSpace[] dock)
        {
            int selectedSpace = 0;
            bool spaceFound = false;

            // Om index 0-2 är ledigt och index 3 upptaget
            if (dock[0].ParkedBoats.Count == 0
                && dock[1].ParkedBoats.Count == 0
                && dock[2].ParkedBoats.Count == 0
                && dock[3].ParkedBoats.Count > 0)
            {
                selectedSpace = 0;
                spaceFound = true;
            }

            //Annars, hitta tre lediga platser intill varandra med upptagna platser runtom
            if (spaceFound == false)
            {
                var q = dock
                    .FirstOrDefault(h => h.ParkedBoats.Count == 0
                    && h.SpaceId > 0
                    && h.SpaceId < dock.Length - 3
                    && dock[h.SpaceId + 1].ParkedBoats.Count == 0
                    && dock[h.SpaceId + 2].ParkedBoats.Count == 0
                    && dock[h.SpaceId - 1].ParkedBoats.Count > 0
                    && dock[h.SpaceId + 3].ParkedBoats.Count > 0);

                if (q != null)
                {
                    selectedSpace = q.SpaceId;
                    spaceFound = true;
                }
            }

            // Annars, om tre sista index är ledigt och index innan upptaget
            if (spaceFound == false)
            {
                if (dock[dock.Length - 3].ParkedBoats.Count == 0
                    && dock[dock.Length - 2].ParkedBoats.Count == 0
                    && dock[dock.Length - 1].ParkedBoats.Count == 0
                    && dock[dock.Length - 4].ParkedBoats.Count > 0)
                {
                    selectedSpace = dock.Length - 3;
                    spaceFound = true;
                }
            }

            return (selectedSpace, spaceFound);
        }

        private static (int selectedSpace, bool spaceFound) FindFirstThreeFreeSpaces(HarbourSpace[] dock)
        {
            int selectedSpace = 0;
            bool spaceFound = false;

            var q = dock
                   .FirstOrDefault(h => h.ParkedBoats.Count == 0
                   && h.SpaceId < dock.Length - 2
                   && dock[h.SpaceId + 1].ParkedBoats.Count == 0
                   && dock[h.SpaceId + 2].ParkedBoats.Count == 0);

            if (q != null)
            {
                selectedSpace = q.SpaceId;
                spaceFound = true;
            }

            return (selectedSpace, spaceFound);
        }
    }
}
