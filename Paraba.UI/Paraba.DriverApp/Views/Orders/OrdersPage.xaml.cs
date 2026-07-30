namespace Paraba.DriverApp.Views.Orders;

public partial class OrdersPage : ContentView
{
    public OrdersPage()
    {
        InitializeComponent();
    }

    public Task LoadAsync(int? driverId) => OrdersContent.LoadAsync(driverId);
}
