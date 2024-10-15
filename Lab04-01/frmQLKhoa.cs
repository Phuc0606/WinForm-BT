using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04_01
{
    public partial class frmQLKhoa : Form
    {
        public frmQLKhoa()
        {
            InitializeComponent();
            InitializeTotalGS();
        }

        private void InitializeTotalGS()
        {
            // Đặt tổng số giáo sư ban đầu là 0 và vô hiệu hóa TextBox
            txtTotalGS1.Text = "0"; // Giả sử txtTotalGS là tên của TextBox
            txtTotalGS1.Enabled = false; // Vô hiệu hóa TextBox
        }


        private void button4_Click(object sender, EventArgs e)
        {
          
                // Ẩn form hiện tại
                this.Hide();

                // Tạo một thể hiện mới của form quản lý thông tin sinh viên
                frmQLSV quanLySinhVienForm = new frmQLSV();

            // Hiển thị form quản lý thông tin sinh viên
            quanLySinhVienForm.Show();
            }
        private bool IsValidMaKhoa(string maKhoa)
        {
            // Kiểm tra mã khoa có chứa ký tự đặc biệt không
            return System.Text.RegularExpressions.Regex.IsMatch(maKhoa, @"^[a-zA-Z0-9]+$");
        }
        private bool IsValidTotalGS(string totalGS)
        {
            if (int.TryParse(totalGS, out int result))
            {
                return result >= 0 && result <= 15;
            }
            return false; // Không phải là số hợp lệ
        }
        private bool IsValidTenKhoa(string tenKhoa)
        {
            // Kiểm tra tên khoa phải là chữ và không chứa ký tự đặc biệt
            return tenKhoa.Length >= 3 && tenKhoa.Length <= 100 &&
                   System.Text.RegularExpressions.Regex.IsMatch(tenKhoa, @"^[\p{L}\s]{3,100}$");
        }
        private void AddKhoa()
        {
            // Thêm khoa vào DataGridView hoặc thực hiện hành động khác
            
            dataGridView.Rows.Add(txtMaKhoa.Text, txtTenKhoa.Text, txtTotalGS.Text);
            MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reset các ô nhập liệu
            txtMaKhoa.Clear();
            txtTenKhoa.Clear();
            txtTotalGS.Clear();
            UpdateTotalGS();
        }
        private void button1_Click(object sender, EventArgs e)
        {

            // Kiểm tra các trường nhập liệu có trống hay không
            if (string.IsNullOrWhiteSpace(txtMaKhoa.Text) ||
                string.IsNullOrWhiteSpace(txtTenKhoa.Text) ||
                string.IsNullOrWhiteSpace(txtTotalGS.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Ngưng thực thi nếu có trường để trống
            }

            // Kiểm tra mã khoa
            if (!IsValidMaKhoa(txtMaKhoa.Text))
            {
                MessageBox.Show("Mã khoa không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra tổng số giáo sư
            if (!IsValidTotalGS(txtTotalGS.Text))
            {
                MessageBox.Show("Tổng số giáo sư không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra tên khoa
            if (!IsValidTenKhoa(txtTenKhoa.Text))
            {
                MessageBox.Show("Tên khoa không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string maKhoa = txtMaKhoa.Text.Trim();
            string tenKhoa = txtTenKhoa.Text.Trim();
            int totalGS = int.Parse(txtTotalGS.Text.Trim());

            // Kiểm tra mã khoa đã tồn tại trong DataGridView hay chưa
            bool exists = false;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["MaKhoa"].Value != null && row.Cells["MaKhoa"].Value.ToString() == maKhoa)
                {
                    exists = true; // Đánh dấu là đã tồn tại
                    break; // Thoát khỏi vòng lặp nếu tìm thấy
                }
            }
            if (exists)
            {
                MessageBox.Show("Mã khoa đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Nếu mã khoa chưa tồn tại, thêm mới vào DataGridView
                AddKhoa();
            }

            // Đặt lại các ô nhập liệu về trạng thái mặc định
            txtMaKhoa.Clear();
            txtTenKhoa.Clear();
            txtTotalGS.Clear();

            // Gọi lại hàm cập nhật tổng số GS
            UpdateTotalGS();
        }

            private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
             {
                DataGridViewRow row = dataGridView.Rows[e.RowIndex];

                // Hiển thị thông tin của dòng được chọn vào các ô nhập liệu
                txtMaKhoa.Text = row.Cells["MaKhoa"].Value.ToString();
                txtTenKhoa.Text = row.Cells["TenKhoa"].Value.ToString();
                txtTotalGS.Text = row.Cells["TotalGS1"].Value.ToString();
            }
        }
        private void UpdateTotalGS()
        {
            int totalGS = 0;

            // Duyệt qua tất cả các dòng trong DataGridView để tính tổng số giáo sư
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["TotalGS1"].Value != null) // Sử dụng tên cột chính xác
                {
                    // Cộng dồn tổng số giáo sư từ các dòng
                    totalGS += Convert.ToInt32(row.Cells["TotalGS1"].Value);
                }
            }

            // Hiển thị tổng số giáo sư lên TextBox
            txtTotalGS1.Text = totalGS.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string maKhoa = txtMaKhoa.Text.Trim(); // Lấy mã khoa từ TextBox
            bool exists = false;

            // Kiểm tra mã khoa trong DataGridView
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["MaKhoa"].Value.ToString() == maKhoa)
                {
                    exists = true; // Nếu tồn tại mã khoa
                    var result = MessageBox.Show("Bạn có chắc chắn muốn xóa khoa này không?", "Xác nhận",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // Thực hiện xóa
                        dataGridView.Rows.Remove(row);
                        MessageBox.Show("Xóa khoa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break; // Thoát khỏi vòng lặp sau khi xử lý
                }
            }

            if (!exists)
            {
                MessageBox.Show("Mã khoa không tồn tại trong hệ thống.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void SortByTotalGS(bool ascending)
        {
            List<DataGridViewRow> rows = dataGridView.Rows.Cast<DataGridViewRow>().ToList();
            rows = ascending
                ? rows.OrderBy(r => Convert.ToInt32(r.Cells["TotalGS1"].Value)).ToList()
                : rows.OrderByDescending(r => Convert.ToInt32(r.Cells["TotalGS1"].Value)).ToList();

            dataGridView.Rows.Clear();
            foreach (var row in rows)
            {
                dataGridView.Rows.Add(row.Cells["MaKhoa"].Value, row.Cells["TenKhoa"].Value, row.Cells["TotalGS1"].Value);
            }
        }

            // Ví dụ gọi hàm sắp xếp theo số lượng giáo sư tăng dần
            private void btnSortAscending_Click(object sender, EventArgs e)
        {
            SortByTotalGS(true);
        }

        // Ví dụ gọi hàm sắp xếp theo số lượng giáo sư giảm dần
        private void btnSortDescending_Click(object sender, EventArgs e)
        {
            SortByTotalGS(false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
           string maKhoa = txtMaKhoa.Text.Trim();
            string tenKhoa = txtTenKhoa.Text.Trim();
            int totalGS;

            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(maKhoa) || string.IsNullOrWhiteSpace(tenKhoa) ||
                !int.TryParse(txtTotalGS.Text.Trim(), out totalGS) || totalGS < 0 || totalGS > 15)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra mã khoa đã tồn tại trong DataGridView
            bool exists = false;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells["MaKhoa"].Value.ToString() == maKhoa)
                {
                    // Cập nhật thông tin
                    row.Cells["TenKhoa"].Value = tenKhoa;
                    row.Cells["TotalGS1"].Value = totalGS;
                    exists = true;
                    MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break; // Thoát khỏi vòng lặp sau khi cập nhật
                }
            }

            if (!exists)
            {
                MessageBox.Show("Mã khoa không tồn tại để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Gọi lại hàm UpdateTotalGS sau khi cập nhật
                UpdateTotalGS();
            }

            // Đặt lại các ô nhập liệu về trạng thái mặc định
            txtMaKhoa.Clear();
            txtTenKhoa.Clear();
            txtTotalGS.Clear();
        }


        private void Form2_Load(object sender, EventArgs e)
        {
            comboBoxSort.Items.Add("Tăng dần"); // Thêm mục sắp xếp tăng dần
            comboBoxSort.Items.Add("Giảm dần"); // Thêm mục sắp xếp giảm dần
            comboBoxSort.SelectedIndex = 0; // Chọn mục mặc định
        }
            private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
        
        }
        }
    }

   
    

    
    
   
    
    
    

