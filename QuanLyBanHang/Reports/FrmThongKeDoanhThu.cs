using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using QuanLyBanHang.Data;

namespace QuanLyBanHang.Reports
{
    public partial class FrmThongKeDoanhThu : Form
    {
        QLBHDbContext context = new QLBHDbContext();
        string reportsFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Reports");

        // Dùng đúng bảng DanhSachHoaDon theo chuẩn Buổi 8
        QLBHDataSet.DanhSachHoaDonDataTable danhSachHoaDonDataTable = new QLBHDataSet.DanhSachHoaDonDataTable();

        public FrmThongKeDoanhThu()
        {
            InitializeComponent();
            this.Controls.Add(reportViewer1);
            panel1.SendToBack();
            reportViewer1.BringToFront();
        }

        private void FrmThongKeDoanhThu_Load(object sender, EventArgs e)
        {
            // Mặc định gọi hàm tải dữ liệu (không lọc)
            LoadReportData(null, null);
        }

        private void btnLocKetQua_Click(object sender, EventArgs e)
        {
            // Lọc theo ngày được chọn trên DateTimePicker
            LoadReportData(dtptuNgay.Value, dtpDenNgay.Value);
        }

        private void btnHienTatCa_Click(object sender, EventArgs e)
        {
            // Bấm hiện tất cả thì bỏ lọc
            LoadReportData(null, null);
        }

        // Hàm xử lý chung để tính toán và tránh lỗi Int16 EF Core
        private void LoadReportData(DateTime? tuNgay, DateTime? denNgay)
        {
            // Bước 1: Lấy dữ liệu thô (Tránh lỗi ép kiểu Int16 của EF Core)
            var query = context.HoaDon.Select(hd => new
            {
                hd.ID,
                hd.NhanVienID,
                HoVaTenNhanVien = hd.NhanVien.HoVaTen,
                hd.KhachHangID,
                HoVaTenKhachHang = hd.KhachHang.HoVaTen,
                hd.NgayLap,
                hd.GhiChuHoaDon,
                // Chỉ lấy số lượng và đơn giá để C# tự tính tổng
                ChiTiet = hd.HoaDon_ChiTiet.Select(ct => new { ct.SoLuongBan, ct.DonGiaBan })
            });

            // Bước 2: Áp dụng điều kiện Lọc nếu có
            if (tuNgay.HasValue && denNgay.HasValue)
            {
                DateTime start = tuNgay.Value.Date;
                DateTime end = denNgay.Value.Date.AddDays(1).AddSeconds(-1); // Lấy tới 23:59:59 của ngày kết thúc
                query = query.Where(hd => hd.NgayLap >= start && hd.NgayLap <= end);
            }

            var rawData = query.ToList(); // Ngắt SQL tại đây

            // Bước 3: Dùng C# tính tổng và đổ vào DataTable
            danhSachHoaDonDataTable.Clear();
            foreach (var r in rawData)
            {
                danhSachHoaDonDataTable.AddDanhSachHoaDonRow(
                    r.ID,
                    r.NhanVienID ,
                    r.HoVaTenNhanVien,
                    r.KhachHangID ,
                    r.HoVaTenKhachHang,
                    r.NgayLap,
                    r.GhiChuHoaDon,
                    r.ChiTiet.Sum(ct => (int)ct.SoLuongBan * ct.DonGiaBan) // Tính tổng siêu an toàn
                );
            }

            // Bước 4: Đẩy lên ReportViewer
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DanhSachHoaDon"; // Trùng tên cấu hình trong RDLC
            reportDataSource.Value = danhSachHoaDonDataTable;

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptThongKeDoanhThu.rdlc");

            // Xử lý tham số Mô tả hiển thị
            string moTa = (tuNgay.HasValue && denNgay.HasValue)
                ? $"Từ ngày {tuNgay.Value:dd/MM/yyyy} - Đến ngày: {denNgay.Value:dd/MM/yyyy}"
                : "(Tất cả thời gian)";

            ReportParameter reportParameter = new ReportParameter("MoTaKetQuaHienThi", moTa);
            reportViewer1.LocalReport.SetParameters(reportParameter);

            reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer1.ZoomMode = ZoomMode.Percent;
            reportViewer1.ZoomPercent = 100;
            reportViewer1.RefreshReport();
        }
    }
}