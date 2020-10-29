using System;

namespace Harbour
{
    class Boat
    {
        public string Type { get; set; }
        public string IdNumber { get; set; }
        public int Weight { get; set; }
        public int MaximumSpeed { get; set; }
        public int DaysStaying { get; set; }
        public int DaysSinceArrival { get; set; }

        public Boat(int weight, int maxSpeed, int daysStaying, int daysSinceArrival)
        {
            Weight = weight;
            MaximumSpeed = maxSpeed;
            DaysStaying = daysStaying;
            DaysSinceArrival = daysSinceArrival;
        }

        
        public static string GenerateID()
        {
            string id = "";

            for (int i = 0; i < 3; i++)
            {
                int number = Utils.r.Next(26);
                char c = (char)('A' + number);
                id += c;
            }

            return id;
        }

        public virtual string TextToFile(int index)
        {
            return $"{index};{IdNumber};{Weight};{MaximumSpeed};{Type};{DaysStaying};{DaysSinceArrival};";
        }
    }
}
