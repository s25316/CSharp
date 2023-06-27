using Lesson3.Models;
using Lesson3.Models.DTO;

namespace Lesson3.Services
{
    public class StudentsDb : IStudentsDb
    {
        private static readonly string WorkingDir = Environment.CurrentDirectory;
        private static string PathToFIle = WorkingDir + "\\dane.csv";
        private static int _numOfVaildElements = 9; //Magic numbers
        private static StatementListStudentsDTO StudentsList ;
        private static bool IsLoaded = false;



        public async Task<StatementStudentDTO> DeleteStudentAsync(int IndexNumber)
        {
            if (IsLoaded == false)
            {
                Console.WriteLine("a");
                IsLoaded = true;
                StudentsList = await GetListOfStudentsAsync();
            }

            if (StudentsList.Success == false)
            {
                return new StatementStudentDTO()
                {
                    Success = false,
                    Information = StudentsList.Information,
                    Student = new Student()
                };
            }



            return IsExistStudentAndDelete(IndexNumber);
            
        }

        public async Task<StatementListStudentsDTO> GetAllStudents()
        {
            if (IsLoaded == false)
            {
                
                IsLoaded = true;
                StudentsList = await GetListOfStudentsAsync();
            }
            return StudentsList;
        }

        public async Task<StatementStudentDTO> GetStudentAsync(int IndexNumber)
        {
            
            if (IsLoaded == false)
            {                
                IsLoaded = true;
                StudentsList = await GetListOfStudentsAsync();
            }

            if (StudentsList.Success == false)
            {
                return new StatementStudentDTO()
                {
                    Success = false,
                    Information = StudentsList.Information,
                    Student = new Student()
                };
            }

            foreach (Student student in StudentsList.Students)
            {
               
                if (student.IndexNumber.Equals(IndexNumber))
                {
                    return new StatementStudentDTO()
                    {
                        Success = true,
                        Information = "Sztudent znaleziony",
                        Student = student
                    };
                }
            }

            return new StatementStudentDTO()
            {
                Success = false,
                Information = "Sztudent nie znaleziony",
                Student = new Student()
            };
        }

        public async Task<StatementStudentDTO> PostStudentAsync(Student s)
        {
            if (IsLoaded == false)
            {
                Console
                    .WriteLine(IsLoaded);
                IsLoaded = true;
                Console
                    .WriteLine(IsLoaded);
                StudentsList = await GetListOfStudentsAsync();
            }

            if (StudentsList.Success == false)
            {
                return new StatementStudentDTO()
                {
                    Success = false,
                    Information = StudentsList.Information,
                    Student = new Student()
                };
            }


            if (!IsExistStudent(StudentsList.Students, s))
            {

                if (IsStudentComplete(s))
                {
                    StudentsList.Students.Add(s);
                    using (StreamWriter writer = File.AppendText(PathToFIle))
                    {
                        writer.WriteLine($"{s.Fname}, {s.Lname}, {s.Studies.Name}, {s.Studies.Mode}, {s.IndexNumber.ToString()}, {s.Birthdate.ToString()}, {s.Email}, {s.MothersName}, {s.FathersName}");

                    }
                    return new StatementStudentDTO()
                    {
                        Success = true,
                        Information = "Sztudent za podanym IndexNumber został dodany",
                        Student = s
                    };
                }
                return new StatementStudentDTO()
                {
                    Success = false,
                    Information = "Sztudent niekompletny",
                    Student = s
                };

            }

            return new StatementStudentDTO()
            {
                Success = false,
                Information = "Sztudent za podanym IndexNumber  instnieje, brak dodania",
                Student = s
            };

        }

        public async Task<StatementStudentDTO> PutStudentAsync(int IndexNumber, StudentPutDTO studentPutDTO)
        {
            if (IsLoaded == false)
            {
                Console
                    .WriteLine(IsLoaded);
                IsLoaded = true;
                Console
                    .WriteLine(IsLoaded);
                StudentsList = await GetListOfStudentsAsync();
            }

            if (StudentsList.Success == false)
            {
                return new StatementStudentDTO()
                {
                    Success = false,
                    Information = StudentsList.Information,
                    Student = new Student()
                };
            }

            Student student = new Student()
            {
                IndexNumber = IndexNumber,
                Fname = studentPutDTO.Fname,
                Lname = studentPutDTO.Lname,
                Birthdate = studentPutDTO.Birthdate,
                Email = studentPutDTO.Email,
                MothersName = studentPutDTO.MothersName,
                FathersName = studentPutDTO.FathersName,
                Studies = studentPutDTO.Studies

            };

            if (IsExistStudentAndDelete(IndexNumber).Success)
            {
                StudentsList.Students.Add(student);
                return new StatementStudentDTO()
                {
                    Success = true,
                    Information = "Sztudent za podanym IndexNumber został zaktualizowany",
                    Student = student
                };
            }

            return new StatementStudentDTO()
            {
                Success = false,
                Information = "Sztudent za podanym IndexNumber nie instnieje, brak dodania",
                Student = student
            };

        }


        private static async Task<StatementListStudentsDTO> GetListOfStudentsAsync() 
        {
            
            IList<Student> students = new List<Student>();

            FileInfo file = new FileInfo(PathToFIle);

            if (!File.Exists(file.FullName))
            {
                if (Directory.Exists(file.FullName))
                {
                    
                    return new StatementListStudentsDTO() 
                    { 
                        Success = false,
                        Information = "Podana ścieżka jest niepoprawna",
                        Students = students
                    };
                }
                return new StatementListStudentsDTO()
                {
                    Success = false,
                    Information = "Plik nazwa nie istnieje",
                    Students = students
                };
            }

            using StreamReader streamReader = new(file.OpenRead());

            string line = "";

            while ((line = await streamReader.ReadLineAsync()) != null)
            {
                string[] table = line.Split(',');
                if (table.Length != _numOfVaildElements)
                {
                    //Console.Write($"Podany ciąg danych niepoprawny: {line} ;");
                    continue;
                }
                if (!IsNotNullInTable(table))
                {
                    //Console.Write($"Podany ciąg danych zawiera wartości NULL lub białe znaki : {line} ;");
                    continue;
                }
                //Paweł,Nowak1,Informatyka dzienne,Dzienne,459,2000-02-12,1@pjwstk.edu.pl,Alina,Adam

                Student student = ParseToStudent(table);

                if (!IsExistStudent(students, student))
                {
                    students.Add(student);
                }
            }
            streamReader.Close();

            return new StatementListStudentsDTO()
            {
                Success = true,
                Information = "Utworzona lista studentów",
                Students = students
            };
        }

        private static Student ParseToStudent(string[] table)
        {

            Student student = new Student()
            {
                IndexNumber = int.Parse(table[4]),
                Fname = table[0],
                Lname = table[1],
                Birthdate = DateTime.Parse(table[5]),
                Email = table[6],
                MothersName = table[7],
                FathersName = table[8],
                Studies = new()
                {
                    Name = table[2],
                    Mode = table[3]
                }
            };
            return student;
        }

        private static bool IsNotNullInTable(string[] table)
        {
            foreach (string s in table)
            {
                if (string.IsNullOrWhiteSpace(s) || s.Equals(""))
                {
                    //Console.WriteLine(s);
                    return false;
                }
            }
            return true;
        }

        private static bool IsExistStudent(IList<Student> students, Student student)
        {
            foreach (Student s in students)
            {
                if (s.IndexNumber.Equals(student.IndexNumber))
                {
                    return true;
                }
            }
            return false;
        }

        private StatementStudentDTO IsExistStudentAndDelete(int IndexNumber)
        {
            foreach (Student s in StudentsList.Students)
            {
                if (s.IndexNumber.Equals(IndexNumber))
                {
                    Student student = s;
                    StudentsList.Students.Remove(s);

                    return new StatementStudentDTO()
                    {
                        Success = true,
                        Information = "Sztudent usuniety",
                        Student = student
                    };
                }

            }
            return  new StatementStudentDTO()
            {
                Success = false,
                Information = "Sztudent nie usuniety, student o podanym IndexNumber nie istnieje",
                Student = new Student()
            };
        }

        private static bool IsStudentComplete(Student s)
        {
            string[] table = { s.IndexNumber.ToString(), s.Lname, s.Birthdate.ToString(), s.Email, s.MothersName, s.FathersName, s.Studies.Name, s.Studies.Mode };
            return IsNotNullInTable(table);
        }
    }
}
