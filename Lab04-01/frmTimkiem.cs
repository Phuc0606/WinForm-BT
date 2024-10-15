using Lab04_01.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Lab04_01
{
    public partial class frmTimkiem : Form
    {
        private Model1 StudentDB = new Model1();

        public frmTimkiem()
        {
            InitializeComponent();

        }
        private void frmTimkiem_Load(object sender, EventArgs e)
        {
            rbNu.Checked = true;
            txtKetQuaTimKiem.ReadOnly = true;
            txtKetQuaTimKiem.Text = "0";
            List<Student> listStudent = StudentDB.Student.ToList();
            LoadSinhVien(listStudent);
            List<Faculty> facultyList = StudentDB.Faculty.ToList();
            BindingCmbFaculty(facultyList);
        }
        private void LoadKhoa(List<Faculty> facultyList)
        {
            try
            {
                cbKhoa.DataSource = null;  // Đặt DataSource về null trước khi thiết lập lại

                cbKhoa.DataSource = facultyList;
                cbKhoa.DisplayMember = "FacultyName";
                cbKhoa.ValueMember = "FacultyID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void LoadSinhVien(List<Student> listStudent)
        {
            // Xóa các hàng hiện có trong DataGridView để nạp dữ liệu mới
            dgvSinhvienFind.Rows.Clear();

            // Duyệt qua danh sách sinh viên và thêm vào DataGridView
            foreach (var student in listStudent)
            {
                // Thêm một hàng mới vào DataGridView
                int rowIndex = dgvSinhvienFind.Rows.Add();

                // Điền dữ liệu cho từng cột tương ứng với sinh viên
                dgvSinhvienFind.Rows[rowIndex].Cells[0].Value = student.StudentID;          // Mã sinh viên
                dgvSinhvienFind.Rows[rowIndex].Cells[1].Value = student.FullName;           // Họ tên
                dgvSinhvienFind.Rows[rowIndex].Cells[2].Value = student.Gender;             // Giới tính
                dgvSinhvienFind.Rows[rowIndex].Cells[3].Value = student.Faculty.FacultyName;       // Tên khoa
                dgvSinhvienFind.Rows[rowIndex].Cells[4].Value = student.AverageScore; // Điểm trung bình
            }

            // Cập nhật số kết quả tìm kiếm sau khi tải danh sách
            CapNhatKetQuaTimKiem();
        }

        private void CapNhatKetQuaTimKiem()
        {
            txtKetQuaTimKiem.Text = dgvSinhvienFind.Rows.Count.ToString();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            string maSV = txtMaSV.Text;

            // Tìm sinh viên trong cơ sở dữ liệu
            var sinhVien = StudentDB.Student.SingleOrDefault(s => s.StudentID == maSV);
            if (sinhVien == null)
            {
                MessageBox.Show("Mã sinh viên không tồn tại trong hệ thống.");
                return;
            }

            // Hiển thị cảnh báo xác nhận trước khi xóa
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    // Xóa sinh viên khỏi cơ sở dữ liệu
                    StudentDB.Student.Remove(sinhVien);
                    StudentDB.SaveChanges();

                    MessageBox.Show("Xóa sinh viên thành công!");

                    // Tải lại danh sách sinh viên sau khi xóa
                    List<Student> listStudent = StudentDB.Student.ToList();
                    LoadSinhVien(listStudent);

                    // Xóa nội dung các trường nhập liệu
                    XoaNoiDungNhapLieu();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void XoaNoiDungNhapLieu()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            rbNam.Checked = false;
            rbNu.Checked = false;
            cbKhoa.SelectedIndex = -1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btTim_Click(object sender, EventArgs e)
        {
            var query = StudentDB.Set<Student>().Include("Faculty").AsQueryable();

            // Lọc theo mã sinh viên nếu có nhập
            if (!string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                query = query.Where(s => s.StudentID.Contains(txtMaSV.Text));
            }

            // Lọc theo họ tên nếu có nhập
            if (!string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                query = query.Where(s => s.FullName.Contains(txtHoTen.Text));
            }

            // Lọc theo giới tính
            if (rbNam.Checked)
            {
                query = query.Where(s => s.Gender == "Nam");
            }
            else if (rbNu.Checked)
            {
                query = query.Where(s => s.Gender == "Nữ");
            }

            // Lọc theo khoa
            if (cbKhoa.SelectedValue != null)
            {
                int khoaId = (int)cbKhoa.SelectedValue;
                query = query.Where(s => s.FacultyID == khoaId);
            }

            // Lấy kết quả tìm kiếm
            var results = query.ToList();

            // Hiển thị kết quả tìm kiếm trong DataGridView
            LoadSinhVien(results);

            // Hiển thị thông báo nếu không tìm thấy kết quả
            if (results.Count == 0)
            {
                MessageBox.Show("Không tìm thấy kết quả nào.");
            }
        }

    }

}


