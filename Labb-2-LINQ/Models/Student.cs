using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_2_LINQ.Models
{
    internal class Student
    {
        [Key] [Required] public int StudentID { get; set; }
        [Required] public string StudentName { get; set; }
        public int CourseID { get; set; }

        public Course Course { get; set; }

    }
}
