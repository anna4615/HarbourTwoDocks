using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static (int, bool) FindCatamaranSpace(HarbourSpace[] harbour)
        {
            int selectedSpace = 0;
            bool spaceFound = false;

            // Om index 0-2 är ledigt och index 3 upptaget
            if (harbour[0].ParkedBoats.Count == 0
                && harbour[1].ParkedBoats.Count == 0
                && harbour[2].ParkedBoats.Count == 0
                && harbour[3].ParkedBoats.Count > 0)
            {
                selectedSpace = 0;
                spaceFound = true;
            }

            //Annars, hitta tre lediga platser intill varandra med upptagna platser runtom
            if (spaceFound == false)
            {
                var q1 = harbour
                    .FirstOrDefault(h => h.ParkedBoats.Count == 0
                    && h.SpaceId > 0
                    && h.SpaceId < harbour.Length - 3
                    && harbour[h.SpaceId + 1].ParkedBoats.Count == 0
                    && harbour[h.SpaceId + 2].ParkedBoats.Count == 0
                    && harbour[h.SpaceId - 1].ParkedBoats.Count > 0
                    && harbour[h.SpaceId + 3].ParkedBoats.Count > 0);

                if (q1 != null)
                {
                    selectedSpace = q1.SpaceId;
                    spaceFound = true;
                }
            }

            // Annars, hitta första tre lediga intill varandra
            if (spaceFound == false)
            {
                var q2 = harbour
                   .FirstOrDefault(h => h.ParkedBoats.Count == 0
                   && h.SpaceId < harbour.Length - 2
                   && harbour[h.SpaceId + 1].ParkedBoats.Count == 0
                   && harbour[h.SpaceId + 2].ParkedBoats.Count == 0);

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
