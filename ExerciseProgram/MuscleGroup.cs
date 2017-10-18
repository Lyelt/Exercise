using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseProgram
{
    public class MuscleGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string BodyPart { get; set; }

        public MuscleGroup (int id, string name, string fullName, string bodyPart)
        {
            ID = id;
            Name = name;
            FullName = fullName;
            BodyPart = bodyPart;
        }
    }
}
