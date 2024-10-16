using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace GUI
{
    public partial class frmQLSV : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();

        public frmQLSV()
        {
            InitializeComponent();
        }

        private void frmQLSV_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Thoát không?", "Xác nhận thoát",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Cái này Copilot nó làm.
        private void setGridViewStyle(DataGridView dgv)
        {
            // Đặt màu nền của DataGridView
            dgv.BackgroundColor = Color.WhiteSmoke;

            // Đặt kiểu cho các ô
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.ForeColor = Color.Navy;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.CornflowerBlue;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            // Đặt kiểu cho tiêu đề cột
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Đặt kiểu cho hàng
            dgv.RowHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgv.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.RowHeadersDefaultCellStyle.SelectionBackColor = Color.CornflowerBlue;
            dgv.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Thay đổi độ cao của hàng
            dgv.RowTemplate.Height = 30;

            // Kích hoạt đường viền cho các ô
            dgv.EnableHeadersVisualStyles = false;
        }

        //private void BindGrid(List<Student> listStudents)
        //{
        //    dgvStudent.Rows.Clear();
        //    foreach (Student student in listStudents)
        //    {
        //        int index = dgvStudent.Rows.Add();
        //        dgvStudent.Rows[index].Cells[0].Value = student.StudentID;
        //        dgvStudent.Rows[index].Cells[1].Value = student.FullName;
        //        dgvStudent.Rows[index].Cells[2].Value = student.Faculty.FacultyName;
        //        dgvStudent.Rows[index].Cells[3].Value = student.AverageScore;
        //    }
        //}

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach (var student in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = student.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = student.FullName;
                if (student.Faculty != null)
                    dgvStudent.Rows[index].Cells[2].Value = student.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = student.AverageScore + "";
                if (student.MajorID != null)
                    dgvStudent.Rows[index].Cells[4].Value = student.Major.Name + "";
            }
        }
        private void FillFalcultyCombobox(List<Faculty> faculties)
        {
            cbbKhoa.ForeColor = Color.Navy;
            cbbKhoa.DataSource = faculties;
            cbbKhoa.DisplayMember = "FacultyName";
            cbbKhoa.ValueMember = "FacultyID";
        }
        private void frmQLSV_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkBox_CheckedChanged(object sender, EventArgs e)
        {
                var listStudents = new List<Student>();
                if (this.chkBox.Checked)
                    listStudents = studentService.GetAllHasNoMajor();
                else
                    listStudents = studentService.GetAll();
                BindGrid(listStudents);
        }

        private void ResetForm()
        {
            txtMaSo.Clear();
            txtHoTen.Clear();
            cbbKhoa.SelectedIndex = 0;
            txtDTB.Clear();
        }

        private void LoadData()
        {
            using (StudentContexDB context = new StudentContexDB())
            {

                List<Student> listStudents = context.Students.ToList();
                BindGrid(listStudents);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMaSo.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                }
                else if (string.IsNullOrWhiteSpace(txtHoTen.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên và điểm trung bình!");

                }
                else if (string.IsNullOrWhiteSpace(txtDTB.Text))
                {
                    MessageBox.Show("Vui lòng nhập điểm trung bình!");
                }
                else if (!float.TryParse(txtDTB.Text.Trim(), out float avgScore) || avgScore < 0 || avgScore > 10)
                {
                    MessageBox.Show("Điểm trung bình từ 0.00 đến 10.00!");
                    txtDTB.Clear();
                }
                Student newStudent = new Student
                {
                    StudentID = txtMaSo.Text.Trim(),
                    FullName = txtHoTen.Text.Trim(),
                    FacultyID = (int)cbbKhoa.SelectedIndex + 1,
                    AverageScore = float.Parse(txtDTB.Text),
                };
                studentService.InsertUpdate(newStudent);
                LoadData();
                ResetForm();
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudent.SelectedRows.Count > 0)
            {
                // Lấy mã số sinh viên từ hàng được chọn trong DataGridView
                string studentID = dgvStudent.SelectedRows[0].Cells[0].Value.ToString();

                using (StudentContexDB context = new StudentContexDB())
                {
                    // Tìm sinh viên trong cơ sở dữ liệu
                    var student = context.Students.SingleOrDefault(s => s.StudentID == studentID);

                    if (student != null)
                    {
                        // Xóa sinh viên khỏi cơ sở dữ liệu
                        context.Students.Remove(student);
                        context.SaveChanges();

                        // Xóa sinh viên khỏi DataGridView
                        dgvStudent.Rows.RemoveAt(dgvStudent.SelectedRows[0].Index);

                        MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên với mã số này.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
