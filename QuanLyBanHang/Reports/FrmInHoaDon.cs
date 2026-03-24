using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QuanLyBanHang.Data;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyBanHang.Reports
{
    public partial class FrmInHoaDon : Form
    {
        QLBHDbContext context = new QLBHDbContext();
        string reportsFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Reports");
        int id; // Mã hóa đơn truyền từ form ngoài vào

        public FrmInHoaDon(int maHoaDon = 0)
        {
            InitializeComponent();
            id = maHoaDon;
            this.Controls.Add(reportViewer1); // Chống trắng form
        }

        private void FrmInHoaDon_Load(object sender, EventArgs e)
        {
            try
            {
                var hoaDon = context.HoaDon
                                    .Include(r => r.KhachHang)
                                    .Where(r => r.ID == id)
                                    .SingleOrDefault();

                if (hoaDon != null)
                {
                    var rawChiTiet = context.HoaDon_ChiTiet
                                            .Where(r => r.HoaDonID == id)
                                            .Select(r => new
                                            {
                                                r.ID,
                                                r.HoaDonID,
                                                r.SanPhamID,
                                                TenSanPham = r.SanPham.TenSanPham,
                                                r.SoLuongBan,
                                                r.DonGiaBan
                                            }).ToList();

                    // CÁCH MỚI: TỰ TẠO BẢNG BẰNG CODE C# ĐỂ VƯỢT MẶT DATASET
                    DataTable dtChiTiet = new DataTable();
                    dtChiTiet.Columns.Add("ID", typeof(int));
                    dtChiTiet.Columns.Add("HoaDonID", typeof(int));
                    dtChiTiet.Columns.Add("SanPhamID", typeof(int));
                    dtChiTiet.Columns.Add("TenSanPham", typeof(string));
                    dtChiTiet.Columns.Add("SoLuongBan", typeof(int));
                    dtChiTiet.Columns.Add("DonGiaBan", typeof(int));
                    dtChiTiet.Columns.Add("ThanhTien", typeof(int));

                    int tongTienHoaDon = 0;

                    foreach (var r in rawChiTiet)
                    {
                        int thanhTien = (int)r.SoLuongBan * (int)r.DonGiaBan;
                        tongTienHoaDon += thanhTien;

                        // Truyền dữ liệu trực tiếp vào bảng tự tạo
                        dtChiTiet.Rows.Add(r.ID, r.HoaDonID, r.SanPhamID, r.TenSanPham, r.SoLuongBan, r.DonGiaBan, thanhTien);
                    }

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "DanhSachHoaDon_ChiTiet"; // Khớp với tên Dataset trong RDLC
                    reportDataSource.Value = dtChiTiet;

                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                    reportViewer1.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptInHoaDon.rdlc");

                    string tenKhachHang = (hoaDon.KhachHang != null) ? hoaDon.KhachHang.HoVaTen : "Khách mua lẻ";
                    string diaChiKhach = (hoaDon.KhachHang != null) ? hoaDon.KhachHang.DiaChi : "";

                    IList<ReportParameter> param = new List<ReportParameter>
                    {
                        new ReportParameter("NgayLap", string.Format("Ngày {0} Tháng {1} Năm {2}", hoaDon.NgayLap.Day, hoaDon.NgayLap.Month, hoaDon.NgayLap.Year)),
                        new ReportParameter("NguoiBan_Ten", "CỬA HÀNG THIẾT BỊ IT TRẦN TRÍ NHÂN"),
                        new ReportParameter("NguoiBan_DiaChi", "Đại học An Giang, TP. Long Xuyên"),
                        new ReportParameter("NguoiBan_MaSoThue", "123456789"),
                        new ReportParameter("NguoiMua_Ten", tenKhachHang),
                        new ReportParameter("NguoiMua_DiaChi", diaChiKhach),
                        new ReportParameter("NguoiMua_MaSoThue", ""),
                        new ReportParameter("TongTien", tongTienHoaDon.ToString("N0") + " VNĐ") // Dùng biến C# tính tổng để an toàn tuyệt đối
                    };

                    reportViewer1.LocalReport.SetParameters(param);
                    reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                    reportViewer1.ZoomMode = ZoomMode.Percent;
                    reportViewer1.ZoomPercent = 100;
                    reportViewer1.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}