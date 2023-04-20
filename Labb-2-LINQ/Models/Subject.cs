using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_2_LINQ.Models
{
    internal class Subject
    {
        [Key] [Required] public int SubjectID { get; set; }
        [Required] public string SubjectName { get; set; }

        public ICollection<Course> Courses { get; set; }
        public ICollection<TeacherSubject> TeacherSubjects { get; set; }
    }
}
