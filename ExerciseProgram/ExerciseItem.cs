using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseProgram
{
    /*public enum MuscleGroup
    {
        Quadriceps,
        Hamstrings,
        Glutes,
        Calves,
        Pectorals,
        Hips,
        Lats,
        Traps,
        Deltoids,
        Triceps,
        Biceps,
        Forearms,
        Abdominals,
        LowerBack
    }*/

    public enum ExerciseType
    {
        Flexibility,
        Strength,
        Aerobic
    }

    public enum WeightType
    {
        Barbell,
        Dumbbell,
        Bodyweight,
        Machine,
        Unknown
    }

    public class ExerciseItem
    {
        public string Name { get; set; }
        public List<MuscleGroup> Muscles { get; set; }
        public ExerciseType Type { get; set; }
        public WeightType Weight { get; set; }

        public ExerciseItem(string name, List<MuscleGroup> muscles, string type, string weightType) 
        {
            Name = name;
            Muscles = muscles;

            WeightType w;
            Enum.TryParse(weightType, out w);
            Weight = w;

            ExerciseType t;
            Enum.TryParse(type, out t);
            Type = t;
        }
    }
}
