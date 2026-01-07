using laythoikhoabieucuataoios17;
using System.Runtime.CompilerServices;

namespace laythoikhoabieucuataoios17
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Gọi hàm lấy dữ liệu ngay khi khởi tạo
            LoadSchedule();
        }

        private async void LoadSchedule()
        {
            // Gọi service lấy dữ liệu
            var data = await ScheduleService.GetScheduleAsync();

            // Gán dữ liệu vào giao diện
            // Visual Studio sẽ tự nhận diện cvSchedule và loading nếu file XAML đã lưu đúng
            cvSchedule.ItemsSource = data;

            loading.IsRunning = false;
            loading.IsVisible = false;
        }
    }
}