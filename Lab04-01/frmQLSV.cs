using Lab04_01.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Lab04_01
{
    public partial class frmQLSV : Form
    {
        private Model1 StudentDB = new Model1();

        public frmQLSV()
        {
            InitializeComponent();

        }
        

        private void ResetForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            txtDiemTB.Clear();
            rbNu.Checked = true;
            cbKhoa.SelectedIndex = 0;
        }
        private void FillDataDGV(List<Student> listStudent)
        {
            dgvSinhVien.Rows.Clear();
            foreach (var student in listStudent)
            {
                int RowNew = dgvSinhVien.Rows.Add();
                dgvSinhVien.Rows[RowNew].Cells[0].Value = student.StudentID;
                dgvSinhVien.Rows[RowNew].Cells[1].Value = student.FullName;
                dgvSinhVien.Rows[RowNew].Cells[2].Value = student.Gender;
                dgvSinhVien.Rows[RowNew].Cells[3].Value = student.AverageScore;
                dgvSinhVien.Rows[RowNew].Cells[4].Value = student.Faculty.FacultyName;
            }
        }
        private void BindingCmbFaculty(List<Faculty> facultyList)
        {
            try
            {
                cbKhoa.Items.Clear();
                cbKhoa.DataSource = facultyList;
                cbKhoa.DisplayMember = "FacultyName";
                cbKhoa.ValueMember = "FacultyID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($" Lỗi khi tải dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            // Kiểm tra mã sinh viên, họ tên và điểm trung bình không được để trống
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) || string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtDiemTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return false;
            }

            // Kiểm tra mã số sinh viên: 10 ký tự, chỉ chứa số
            if (!Regex.IsMatch(txtMaSV.Text, @"^\d{10}$"))
            {
                MessageBox.Show("Mã số sinh viên không hợp lệ.");
                return false;
            }

            // Kiểm tra điểm trung bình: số thập phân từ 0 đến 10
            if (!decimal.TryParse(txtDiemTB.Text, out decimal diemTB) || diemTB < 0 || diemTB > 10)
            {
                MessageBox.Show("Điểm trung bình sinh viên không hợp lệ.");
                return false;
            }

            // Kiểm tra tên sinh viên: chỉ chứa chữ, từ 3 đến 100 ký tự
            if (!Regex.IsMatch(txtHoTen.Text, @"^[\p{L}\s]{3,100}$"))
            {
                MessageBox.Show("Tên sinh viên không hợp lệ.");
                return false;
            }

            return true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Bạn có muốn thoát chương trình?", "Xác nhận thoát", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            List<Faculty> facultyList = StudentDB.Faculty.ToList();
            BindingCmbFaculty(facultyList);
            List<Student> listStudent = StudentDB.Student.ToList();
            FillDataDGV(listStudent);
            rbNu.Checked = true;

            txtTongNam.Text = "0";
            txtTongNu.Text = "0";
            txtTongNam.Enabled = false;
            txtTongNu.Enabled = false;
            dgvSinhVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            
            if (!ValidateInput()) return;
            Student newStudent = new Student
            {
                StudentID = txtMaSV.Text,
                FullName = txtHoTen.Text,
                Gender = rbNam.Checked ? "Nam" : "Nữ",
                AverageScore = Convert.ToDouble(decimal.Parse(txtDiemTB.Text)),
                FacultyID = (int)cbKhoa.SelectedValue 
            };

            StudentDB.Student.Add(newStudent);
            StudentDB.SaveChanges();
            dgvSinhVien.Rows.Add(newStudent.StudentID, newStudent.FullName, newStudent.Gender, newStudent.AverageScore, cbKhoa.Text);
            MessageBox.Show("Thêm mới dữ liệu thành công!");
            ResetForm();
            UpdateStudentCount();
        }


        private void button3_Click(object sender, EventArgs e)
        {
           
            bool studentExists = false;

            foreach (DataGridViewRow row in dgvSinhVien.Rows)
            {
                if (row.Cells[0].Value?.ToString() == txtMaSV.Text)
                {
                    studentExists = true;
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            // Xóa sinh viên từ cơ sở dữ liệu
                            var studentToDelete = StudentDB.Student.SingleOrDefault(s => s.StudentID == txtMaSV.Text);
                            if (studentToDelete != null)
                            {
                                StudentDB.Student.Remove(studentToDelete);
                                StudentDB.SaveChanges();
                            }

                            // Xóa sinh viên khỏi DataGridView
                            dgvSinhVien.Rows.Remove(row);
                            MessageBox.Show("Xóa sinh viên thành công!");
                            ResetForm();
                            UpdateStudentCount();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}");
                        }
                    }
                    break;
                }
            }

            if (!studentExists)
            {
                MessageBox.Show("Mã sinh viên không tồn tại trong hệ thống.");
            }
        }

        private void UpdateStudentCount()
        {
            int maleCount = 0;
            int femaleCount = 0;

            foreach (DataGridViewRow row in dgvSinhVien.Rows)
            {
                if (row.Cells[2].Value?.ToString() == "Nam")
                {
                    maleCount++;
                }
                else if (row.Cells[2].Value?.ToString() == "Nữ")
                {
                    femaleCount++;
                }
            }

            txtTongNam.Text = maleCount.ToString();
            txtTongNu.Text = femaleCount.ToString();
        }

        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvSinhVien.Rows[e.RowIndex];
                txtMaSV.Text = selectedRow.Cells[0].Value.ToString();
                txtHoTen.Text = selectedRow.Cells[1].Value.ToString();
                string gender = selectedRow.Cells[2].Value.ToString();
                rbNam.Checked = (gender == "Nam");
                rbNu.Checked = (gender == "Nữ");
                txtDiemTB.Text = selectedRow.Cells[3].Value.ToString();
                cbKhoa.Text = selectedRow.Cells[4].Value.ToString();
            }
        }

        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow selectedRow = dgvSinhVien.Rows[e.RowIndex];
                    txtMaSV.Text = selectedRow.Cells[0].Value.ToString();
                    txtHoTen.Text = selectedRow.Cells[1].Value.ToString();
                    string gender = selectedRow.Cells[2].Value.ToString();
                    rbNam.Checked = (gender == "Nam");
                    rbNu.Checked = (gender == "Nữ");
                    txtDiemTB.Text = selectedRow.Cells[3].Value.ToString();
                    cbKhoa.Text = selectedRow.Cells[4].Value.ToString();
                    selectedRowIndex = e.RowIndex;
                }
            }
        }
        private int selectedRowIndex = -1;

        private void button2_Click(object sender, EventArgs e)
        {

            if (!ValidateInput()) return;

            // Kiểm tra nếu người dùng đã chọn một dòng
            if (selectedRowIndex >= 0)
            {
                // Lấy dòng hiện tại từ DataGridView
                DataGridViewRow row = dgvSinhVien.Rows[selectedRowIndex];

                // Kiểm tra mã sinh viên đã tồn tại hay chưa (trừ dòng hiện tại đang chỉnh sửa)
                foreach (DataGridViewRow otherRow in dgvSinhVien.Rows)
                {
                    if (otherRow.Index != selectedRowIndex && otherRow.Cells[0].Value != null && otherRow.Cells[0].Value.ToString() == txtMaSV.Text)
                    {
                        MessageBox.Show("Mã số sinh viên mới đã tồn tại. Vui lòng nhập mã khác.");
                        return;
                    }
                }

                try
                {
                    // Tìm sinh viên trong cơ sở dữ liệu theo StudentID hiện tại
                    var studentToUpdate = StudentDB.Student.SingleOrDefault(s => s.StudentID == row.Cells[0].Value.ToString());
                    if (studentToUpdate != null)
                    {
                        // Cập nhật thông tin trong cơ sở dữ liệu
                        studentToUpdate.StudentID = txtMaSV.Text;
                        studentToUpdate.FullName = txtHoTen.Text;
                        studentToUpdate.Gender = rbNam.Checked ? "Nam" : "Nữ";
                        studentToUpdate.AverageScore = Convert.ToDouble(txtDiemTB.Text);  
                        studentToUpdate.FacultyID = (int)cbKhoa.SelectedValue;
                        StudentDB.SaveChanges();

                        // Cập nhật thông tin của dòng trong DataGridView
                        row.Cells[0].Value = txtMaSV.Text;
                        row.Cells[1].Value = txtHoTen.Text;
                        row.Cells[2].Value = rbNam.Checked ? "Nam" : "Nữ";
                        row.Cells[3].Value = txtDiemTB.Text;
                        row.Cells[4].Value = cbKhoa.Text;

                        MessageBox.Show("Cập nhật dữ liệu thành công!");
                        ResetForm();
                        UpdateStudentCount();

                        // Đặt lại selectedRowIndex sau khi cập nhật
                        selectedRowIndex = -1;
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên trong cơ sở dữ liệu.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật sinh viên: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần cập nhật.");
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmQLKhoa formFaculty = new frmQLKhoa();
            formFaculty.ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
       
            frmTimkiem  formFind = new frmTimkiem();
            formFind.Show();
        }

   
    }
}
  

 
  

