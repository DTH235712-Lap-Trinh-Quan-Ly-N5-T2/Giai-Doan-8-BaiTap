using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using QuanLyBanHang.Data;
using static QuanLyBanHang.Data.SanPham;

namespace QuanLyBanHang.Reports
{
    public partial class FrmThongKeSanPham : Form
    {
        QLBHDbContext context = new QLBHDbContext();
        QLBHDataSet.DanhSachSanPhamDataTable danhSachSanPhamDataTable = new QLBHDataSet.DanhSachSanPhamDataTable();
        string reportsFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Reports");
        public FrmThongKeSanPham()
        {
            InitializeComponent();
            this.Controls.Add(reportViewer1);
            panel1.SendToBack();
            reportViewer1.BringToFront();
        }
        public void LayLoaiSanPhamVaoComboBox()
        {
            cboLoaiSanPham.DataSource = context.LoaiSanPham.ToList();
            cboLoaiSanPham.ValueMember = "ID";
            cboLoaiSanPham.DisplayMember = "TenLoai";
        }

        public void LayHangSanXuatVaoComboBox()
        {
            cboHangSanXuat.DataSource = context.HangSanXuat.ToList();
            cboHangSanXuat.ValueMember = "ID";
            cboHangSanXuat.DisplayMember = "TenHangSanXuat";
        }

        private void FrmThongKeSanPham_Load(object sender, EventArgs e)
        {
            LayLoaiSanPhamVaoComboBox();
            LayHangSanXuatVaoComboBox();
            var danhSachSanPham = context.SanPham.Select(r => new DanhSachSanPham
            {
                ID = r.ID,
                HangSanXuatID = r.HangSanXuatID,
                TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                LoaiSanPhamID = r.LoaiSanPhamID,
                TenLoai = r.LoaiSanPham.TenLoai,
                TenSanPham = r.TenSanPham,
                DonGia = r.DonGia,
                SoLuong = r.SoLuong,
                HinhAnh = r.HinhAnh,
                MoTa = r.Mota
            }).ToList();

            danhSachSanPhamDataTable.Clear();
            foreach (var row in danhSachSanPham)
            {
                danhSachSanPhamDataTable.AddDanhSachSanPhamRow(row.ID,
                 row.HangSanXuatID,
                 row.TenHangSanXuat,
                 row.LoaiSanPhamID,
                 row.TenLoai,
                 row.TenSanPham,
                 row.DonGia,
                 row.SoLuong,
                 row.HinhAnh,
                 row.MoTa);
            }

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DanhSachSanPham";
            reportDataSource.Value = danhSachSanPhamDataTable;

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptThongKeSanPham.rdlc");

            ReportParameter reportParameter = new ReportParameter("MoTaKetQuaHienThi", "(Tất cả sản phẩm)");
            reportViewer1.LocalReport.SetParameters(reportParameter);

            reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer1.ZoomMode = ZoomMode.Percent;
            reportViewer1.ZoomPercent = 100;

            reportViewer1.RefreshReport();
        }

        private void btnLocDuLieu_Click(object sender, EventArgs e)
        {
            if (cboHangSanXuat.Text == "" && cboLoaiSanPham.Text == "")
            {
                // Nếu cả 2 ComboBox đều bỏ trống thì hiển thị tất cả 
                FrmThongKeSanPham_Load(sender, e);
            }
            else
            {
                var danhSachSanPham = context.SanPham.Select(r => new DanhSachSanPham
                {
                    ID = r.ID,
                    HangSanXuatID = r.HangSanXuatID,
                    TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                    LoaiSanPhamID = r.LoaiSanPhamID,
                    TenLoai = r.LoaiSanPham.TenLoai,
                    TenSanPham = r.TenSanPham,
                    DonGia = r.DonGia,
                    SoLuong = r.SoLuong,
                    HinhAnh = r.HinhAnh,
                    MoTa = r.Mota
                });

                string hangSanXuat = null;
                string loaiSanPham = null;

                if (cboHangSanXuat.Text != "")
                {
                    int hangSanXuatID = Convert.ToInt32(cboHangSanXuat.SelectedValue.ToString());
                    hangSanXuat = "Hãng sản xuất: " + cboHangSanXuat.Text;
                    danhSachSanPham = danhSachSanPham.Where(r => r.HangSanXuatID == hangSanXuatID);
                }
                if (cboLoaiSanPham.Text != "")
                {
                    int loaiSanPhamID = Convert.ToInt32(cboLoaiSanPham.SelectedValue.ToString());
                    loaiSanPham += "Phân loại: " + cboLoaiSanPham.Text;
                    danhSachSanPham = danhSachSanPham.Where(r => r.LoaiSanPhamID == loaiSanPhamID);
                }

                danhSachSanPhamDataTable.Clear();
                foreach (var row in danhSachSanPham)
                {
                    danhSachSanPhamDataTable.AddDanhSachSanPhamRow(row.ID,
                        row.HangSanXuatID,
                        row.TenHangSanXuat,
                        row.LoaiSanPhamID,
                        row.TenLoai,
                        row.TenSanPham,
                        row.DonGia,
                        row.SoLuong,
                        row.HinhAnh,
                        row.MoTa);
                }

                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "DanhSachSanPham";
                reportDataSource.Value = danhSachSanPhamDataTable;

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                reportViewer1.LocalReport.ReportPath = Path.Combine(reportsFolder, "rptThongKeSanPham.rdlc");

                ReportParameter reportParameter = new ReportParameter("MoTaKetQuaHienThi", "(" + hangSanXuat + " - " + loaiSanPham + ")");
                reportViewer1.LocalReport.SetParameters(reportParameter);
                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.Percent;
                reportViewer1.ZoomPercent = 100;
                reportViewer1.RefreshReport();
            }
        }
    }
}
