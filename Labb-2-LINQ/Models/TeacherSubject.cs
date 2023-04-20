using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_2_LINQ.Models
{
    internal class TeacherSubject
    {
        public int TeacherID { get; set; }
        public int SubjectID { get; set; }

        public Teacher Teacher { get; set; }
        public Subject Subject { get; set; }
    }
}
