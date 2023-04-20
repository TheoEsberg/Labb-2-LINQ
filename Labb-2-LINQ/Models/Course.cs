using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_2_LINQ.Models
{
    internal class Course
    {
        [Key] [Required] public int CourseID { get; set; }
        [Required] public string CourseName { get; set; }
        public int SubjectID { get; set; }

        public Subject Subject { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
