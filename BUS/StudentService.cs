using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            StudentContexDB context = new StudentContexDB();
            return context.Students.ToList();
        }
        public List<Student> GetAllHasNoMajor()
        {
            StudentContexDB context = new StudentContexDB();
            return context.Students.Where(s => s.MajorID == null).ToList();
        }
        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            StudentContexDB context = new StudentContexDB();
            return context.Students.Where(s => s.MajorID == null && s.FacultyID == facultyID).ToList();
        }
        public Student FindById(string studentID)
        {
            StudentContexDB context = new StudentContexDB();
            return context.Students.FirstOrDefault(s => s.StudentID == studentID);
        }
        public void InsertUpdate(Student student)
        {
            StudentContexDB contex = new StudentContexDB();
            contex.Students.AddOrUpdate(student);
            contex.SaveChanges();
        }
    }
}
