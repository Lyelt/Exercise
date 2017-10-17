using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseProgram
{
    public class MuscleGroup
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string BodyPart { get; set; }
        public string Location { get; set; }

        public MuscleGroup (string name, string fullName, string bodyPart, string loc)
        {
            Name = name;
            FullName = fullName;
            BodyPart = bodyPart;
            Location = loc;
        }
    }
}
