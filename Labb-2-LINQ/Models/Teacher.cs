using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_2_LINQ.Models
{
    internal class Teacher
    {
        [Key] [Required] public int TeacherID { get; set; }
        [Required] public string TeacherName { get; set; }
        public ICollection<TeacherSubject> TeacherSubjects { get; set; }
    }
}
