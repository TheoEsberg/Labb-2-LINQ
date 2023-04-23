using Labb_2_LINQ.Data;
using Labb_2_LINQ.Models;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Labb_2_LINQ
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
        }

        private static void MainMenu()
        {
            var choise = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Main menu!")
                .AddChoices(
                    "Show information about all Students",
                    "Change Course for Student",
                    "Change name of Subject",
                    "Check if Subject exists",
                    "Show each Students current Teacher",
                    "Show all Students of specific Subject",
                    "Write data to Database",
                    "Quit"
                ));

            switch (choise)
            {
                case "Show information about all Students":
                    ShowAllStudents();
                    MainMenu();
                    break;
                case "Change Course for Student":
                    ChangeCourse();
                    MainMenu();
                    break;
                case "Change name of Subject":
                    ChangeSubjectName();
                    MainMenu();
                    break;
                case "Check if Subject exists":
                    CheckIfSubjectExists();
                    MainMenu();
                    break;
                case "Show each Students current Teacher":
                    GetTeachersForEachStudent();
                    MainMenu();
                    break;
                case "Show all Students of specific Subject":
                    GetStudentsFromSubject();
                    MainMenu();
                    break;
                case "Write data to Database":
                    WriteDataToDatabase();
                    MainMenu();
                    break;
                case "Quit":
                    break;
            }
        }

        private static void ShowAllStudents()
        {
            var studentTable = new Table().Centered();
            studentTable.AddColumn("Student Name");
            studentTable.AddColumn("Student ID");
            studentTable.AddColumn("Course");
            studentTable.AddColumn("Subject");
            studentTable.AddColumn("Teacher");

            using (var dbContext = new SchoolDbContext())
            {
                var dbData = (from student in dbContext.Students
                              join course in dbContext.Courses
                              on student.CourseID equals course.CourseID
                              join subject in dbContext.Subjects
                              on course.SubjectID equals subject.SubjectID
                              join teacherSubject in dbContext.TeachersSubjects
                              on subject.SubjectID equals teacherSubject.SubjectID
                              join teacher in dbContext.Teachers
                              on teacherSubject.TeacherID equals teacher.TeacherID
                              select new
                              {
                                  student.StudentName,
                                  student.StudentID,
                                  Course = course.CourseName,
                                  Subject = subject.SubjectName,
                                  Teacher = teacher.TeacherName,
                              });


                foreach (var data in dbData)
                {
                    List<string> user = new List<string>()
                    {
                        data.StudentName,
                        data.StudentID.ToString(),
                        data.Course,
                        data.Subject,
                        data.Teacher
                    };

                    studentTable.AddRow(user.ToArray());
                }

                AnsiConsole.Write(studentTable);
            }

            AnsiConsole.WriteLine("\n\tPress any key to return to the main menu.");
            Console.ReadKey();
            Console.Clear();
        }

        private static void ChangeCourse()
        {
            using (var dbContext = new SchoolDbContext())
            {

                var students = dbContext.Students.Select(s => s.StudentName).ToList();
                var studentName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Choose student.")
                .AddChoices(students));
                var student = dbContext.Students.FirstOrDefault(s => s.StudentName == studentName);

                var courses = dbContext.Courses.Select(s => s.CourseName).ToList();
                var courseName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("Choose new course.")
                    .AddChoices(courses));
                var newCourse = dbContext.Courses.FirstOrDefault(c => c.CourseName == courseName);

                student.Course = newCourse;
                dbContext.SaveChanges();

                AnsiConsole.WriteLine("\t{0} has changed course to {1}", studentName, courseName);
                AnsiConsole.WriteLine("\t\nPress any key to return to the main menu.");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void ChangeSubjectName()
        {
            using (var dbContext = new SchoolDbContext())
            {
                var subjects = dbContext.Subjects.Select(s => s.SubjectName).ToList();
                var subjectName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Choose subject.")
                .AddChoices(subjects));
                var subject = dbContext.Subjects.FirstOrDefault(s => s.SubjectName == subjectName);

                AnsiConsole.Write("New subject name: ");
                var newSubjectName = Console.ReadLine();
                subject.SubjectName = newSubjectName;
                dbContext.SaveChanges();

                AnsiConsole.WriteLine("The subject {0} has been changed to {1}", subjectName, newSubjectName);
                AnsiConsole.WriteLine("\nPress any key to return to the main menu.");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void CheckIfSubjectExists()
        {
            using (var dbContext = new SchoolDbContext())
            {
                AnsiConsole.Write("Name to search: ");

                var searchName = Console.ReadLine();
                bool subject = dbContext.Subjects.Any(s => s.SubjectName == searchName);

                AnsiConsole.WriteLine(
                    subject ?
                    "{0} does exist in Subjects" :
                    "{0} does not exist in Subjects",
                    searchName
                );

                AnsiConsole.WriteLine("\nPress any key to return to the main menu.");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void GetTeachersForEachStudent()
        {
            var studentTeacherTable = new Table().Centered();
            studentTeacherTable.AddColumn("Student Name");
            studentTeacherTable.AddColumn("Student ID");
            studentTeacherTable.AddColumn("Teacher Name");
            studentTeacherTable.AddColumn("Teacher ID");

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


                foreach (var data in studentAndTeacher)
                {
                    List<string> user = new List<string>()
                    {
                        data.Student.StudentName,
                        data.Student.StudentID.ToString(),
                        data.Teacher.TeacherName,
                        data.Teacher.TeacherID.ToString(),
                    };

                    studentTeacherTable.AddRow(user.ToArray());
                }

                AnsiConsole.Write(studentTeacherTable);
                AnsiConsole.WriteLine("\n\t\t\t\t\tPress any key to return to the main menu.");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void GetStudentsFromSubject()
        {
            var studentSubjectTable = new Table().Centered();

            using (var dbContext = new SchoolDbContext())
            {
                var subjects = dbContext.Subjects.Select(s => s.SubjectName).ToList();
                var subjectName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Choose subject.")
                .AddChoices(subjects));

                studentSubjectTable.AddColumn(subjectName + " Students");

                var students = dbContext.Students
                    .Where(s => s.Course.Subject.SubjectName == subjectName).ToList();

                foreach (var student in students)
                {
                    studentSubjectTable.AddRow(student.StudentName);
                }
            }

            AnsiConsole.Write(studentSubjectTable);
            AnsiConsole.WriteLine("\n\t\t\t\t\tPress any key to return to the main menu.");
            Console.ReadKey();
            Console.Clear();
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
