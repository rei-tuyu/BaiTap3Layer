﻿using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class MajorService
    {
        public List<Major> GetAllByFaculty(int facultyId)
        {
            StudentContexDB context = new StudentContexDB();
            return context.Majors.Where(s => s.FacultyID == facultyId).ToList();
        }
    }
}
