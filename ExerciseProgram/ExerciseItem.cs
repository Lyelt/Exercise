using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseProgram
{
    public enum ExerciseType
    {
        Flexibility,
        Strength,
        Aerobic,
    }

    public enum WeightType
    {
        Barbell,
        Dumbbell,
        Bodyweight,
        Machine,
        Unknown,
    }

    public class ExerciseItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<MuscleGroup> Muscles { get; set; }
        public ExerciseType Type { get; set; }
        public List<WeightType> WeightTypes { get; set; } = new List<WeightType>();
        public string Description { get; set; }

        public ExerciseItem(int id, string name, List<MuscleGroup> muscles, string type, List<string> weightTypes) 
        {
            ID = id;
            Name = name;
            Muscles = muscles;

            ExerciseType t;
            Enum.TryParse(type, true, out t);
            Type = t;
  
            foreach (var wt in weightTypes)
            {
                WeightType w;
                Enum.TryParse(wt, true, out w);
                WeightTypes.Add(w);
            }
        }
    }
}
