using Microsoft.Extensions.DependencyInjection;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Implementations;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Implementations;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // --- 1. DATA ACCESS LAYER (REPOSITORY & UNIT OF WORK) ---
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
            services.AddScoped<ILopHocRepository, LopHocRepository>();
            services.AddScoped<IHocVienRepository, HocVienRepository>();
            services.AddScoped<IDangKyHocRepository, DangKyHocRepository>();
            services.AddScoped<IBuoiHocRepository, BuoiHocRepository>();
            services.AddScoped<IKetQuaHocTapRepository, KetQuaHocTapRepository>();
            services.AddScoped<IChuongTrinhDaoTaoRepository, ChuongTrinhDaoTaoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // --- 2. BUSINESS LOGIC LAYER (SERVICES) ---
            services.AddScoped<ITaiKhoanService, TaiKhoanService>();
            services.AddScoped<ILopHocService, LopHocService>();
            services.AddScoped<IDangKyHocService, DangKyHocService>();
            services.AddScoped<IHocVienService, HocVienService>();
            services.AddScoped<IBuoiHocService, BuoiHocService>();
            services.AddScoped<IKetQuaHocTapService, KetQuaHocTapService>();
            services.AddScoped<IChuongTrinhDaoTaoService, ChuongTrinhDaoTaoService>();
            services.AddScoped<IThongKeService, ThongKeService>();

            return services;
        }
    }
}
