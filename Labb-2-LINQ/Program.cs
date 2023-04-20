using Labb_2_LINQ.Data;
using Labb_2_LINQ.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Labb_2_LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //WriteDataToDatabase();

            GetStudentsFromSubject("Math");

            Console.WriteLine("\n");

            GetTeachersForEachStudent();

            Console.WriteLine("");

            CheckIfSubjectExists("Programming 2");
            CheckIfSubjectExists("Object Oriented Programming");

            ChangeSubjectName("Object Oriented Programming", "Programming 2");

            CheckIfSubjectExists("Programming 2");
            CheckIfSubjectExists("Object Oriented Programming");

            Console.WriteLine();
            GetTeachersForEachStudent();
            Console.WriteLine();
            ChangeCourse(21, 6, 8);
            Console.WriteLine();
            GetTeachersForEachStudent();
            Console.WriteLine();
            Console.ReadKey();
        }

        private static void ChangeCourse(int studentID, int oldCourseID, int newCourseID)
        {
            using (var dbContext = new SchoolDbContext())
            {
                var student = dbContext.Students.FirstOrDefault(s => s.StudentID == studentID);
                var oldCourse = dbContext.Courses.FirstOrDefault(c => c.CourseID == oldCourseID);
                var newCourse = dbContext.Courses.FirstOrDefault(c => c.CourseID == newCourseID);

                if (student == null) {
                    Console.WriteLine("\tError: Student with ID = {0} was not found!", studentID);
                }
                else if (oldCourse == null)
                {
                    Console.WriteLine("\tError: Course with ID = {0} was not found!", oldCourseID);
                } else if (newCourse == null)
                {
                    Console.WriteLine("\tError: Course with ID = {0} was not found!", newCourseID);
                } else if (student.CourseID != oldCourse.CourseID)
                {
                    Console.WriteLine("\tError: Student {0} does not study {1}", student.StudentName, oldCourse.CourseName);
                }
                else
                {
                    student.Course = newCourse;
                    dbContext.SaveChanges();
                    Console.WriteLine("\tStudent {0}'s current course {1} was change to {2}",
                        student.StudentName, oldCourse.CourseName, newCourse.CourseName);
                }
            }
        }

        private static void ChangeSubjectName(string subjectNameToChange, string newSubjectName)
        {
            using (var dbContext = new SchoolDbContext())
            {
                var subjectToChange = dbContext.Subjects.FirstOrDefault(s => s.SubjectName == subjectNameToChange);
                if (subjectToChange != null)
                {
                    subjectToChange.SubjectName = newSubjectName;
                    dbContext.SaveChanges();
                    Console.WriteLine("\t{0} was successfully changed to {1}", subjectNameToChange, newSubjectName);
                }
                else
                {
                    Console.WriteLine("\tError: Subject {0} was not found!", subjectNameToChange);
                }
            }
        }

        private static void CheckIfSubjectExists(string subjectName)
        {
            using (var dbContext = new SchoolDbContext())
            {
                bool isProgramming1 = dbContext.Subjects.Any(s => s.SubjectName == subjectName);
                Console.WriteLine(
                    isProgramming1 ?
                    "\t{0} exist in Subjects" :
                    "\t{0} does not exist in Subjects",
                    subjectName
                );
            }
        }

        private static void GetTeachersForEachStudent()
        {
            using (var dbContext = new SchoolDbContext())
            {
                var studentAndTeacher = dbContext.Students
                    .Join(dbContext.Courses,
                        s => s.CourseID,
                        c => c.CourseID,
                        (s, c) => new
                        {
                            Student = s,
                            Course = c
                        })
                    .Join(dbContext.TeachersSubjects,
                        sc => sc.Course.SubjectID,
                        ts => ts.SubjectID,
                        (sc, ts) => new
                        {
                            sc.Student,
                            TeacherSubject = ts
                        })
                    .Join(dbContext.Teachers,
                        scs => scs.TeacherSubject.TeacherID,
                        t => t.TeacherID,
                        (scs, t) => new
                        {
                            scs.Student,
                            Teacher = t
                        }).ToList();

                foreach (var student in studentAndTeacher)
                {
                    Console.WriteLine("\t{0} is taught by {1}", student.Student.StudentName, student.Teacher.TeacherName);
                }
            }
        }

        private static void GetStudentsFromSubject(string subjectName)
        {
            using (var dbContext = new SchoolDbContext())
            {
                var students = dbContext.Students
                    .Where(s => s.Course.Subject.SubjectName == subjectName).ToList();

                Console.Write("\t{0} Sudents: ", subjectName);
                Console.Write(string.Join(", ", students.Select(s => s.StudentName)));
            }
        }

        private static void WriteDataToDatabase()
        {
            using (var dbContext = new SchoolDbContext())
            {
                // Create teachers list of Teacher
                List<Teacher> teachers = new List<Teacher>
                {
                    new Teacher { TeacherName = "John Smith" },
                    new Teacher { TeacherName = "Jane Doe" },
                    new Teacher { TeacherName = "Anna Gustafsson" },
                    new Teacher { TeacherName = "Fredrik Einarsson" },
                    new Teacher { TeacherName = "Gustaf Svensson" },
                };

                // Add Teacher from teachers list to the DbSet<Teacher>
                dbContext.Teachers.AddRange(teachers);

                // Create Subjects
                var mathSubject = new Subject { SubjectName = "Math" };
                var swedishSubject = new Subject { SubjectName = "Swedish" };
                var englishSubject = new Subject { SubjectName = "English" };
                var programming1Subject = new Subject { SubjectName = "Programming 1" };
                var programming2Subject = new Subject { SubjectName = "Programming 2" };
                var websiteDevelopmentSubject = new Subject { SubjectName = "Website Development" };

                dbContext.Subjects.AddRange(mathSubject, swedishSubject, englishSubject,
                    programming1Subject, programming2Subject, websiteDevelopmentSubject);

                // Create Courses
                var mathCourse = new Course { CourseName = "Mathematics", Subject = mathSubject };
                var swedishCourse = new Course { CourseName = "Swedish Literature", Subject = swedishSubject };
                var englishCourse = new Course { CourseName = "English Language", Subject = englishSubject };
                var programmingCourse = new Course { CourseName = "Introduction to Programming", Subject = programming1Subject };
                var websiteDevCourse = new Course { CourseName = "Website Development Fundamentals", Subject = websiteDevelopmentSubject };

                dbContext.Courses.AddRange(mathCourse, swedishCourse, englishCourse, programmingCourse, websiteDevCourse);

                // Create List of Students
                var students = new List<Student>
                {
                    new Student { StudentName = "Alice", Course = mathCourse },
                    new Student { StudentName = "Bob", Course = mathCourse },
                    new Student { StudentName = "Charlie", Course = mathCourse },
                    new Student { StudentName = "Dave", Course = englishCourse },
                    new Student { StudentName = "Eve", Course = englishCourse },
                    new Student { StudentName = "Frank", Course = englishCourse },
                    new Student { StudentName = "Grace", Course = swedishCourse },
                    new Student { StudentName = "Henry", Course = swedishCourse },
                    new Student { StudentName = "Isabelle", Course = swedishCourse },
                    new Student { StudentName = "Jack", Course = programmingCourse },
                    new Student { StudentName = "Katie", Course = programmingCourse },
                    new Student { StudentName = "Luke", Course = programmingCourse },
                    new Student { StudentName = "Mary", Course = websiteDevCourse },
                    new Student { StudentName = "Nick", Course = websiteDevCourse },
                    new Student { StudentName = "Olivia", Course = websiteDevCourse },
                };

                // Add Student from list Students to the DbSet<Student>
                dbContext.Students.AddRange(students);

                var teachersSubjects = new List<TeacherSubject>
                {
                    new TeacherSubject { Teacher = teachers[0], Subject = mathSubject },
                    new TeacherSubject { Teacher = teachers[1], Subject = swedishSubject },
                    new TeacherSubject { Teacher = teachers[2], Subject = englishSubject },
                    new TeacherSubject { Teacher = teachers[3], Subject = programming1Subject },
                    new TeacherSubject { Teacher = teachers[3], Subject = programming2Subject },
                    new TeacherSubject { Teacher = teachers[4], Subject = websiteDevelopmentSubject },
                };

                dbContext.TeachersSubjects.AddRange(teachersSubjects);

                // Save changes to the database
                dbContext.SaveChanges();
            }
        }
    }
}