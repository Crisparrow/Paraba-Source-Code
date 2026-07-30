using Paraba.DriverApp.Models;

namespace Paraba.DriverApp.Views.Orders;

public partial class TripDetailPage : ContentPage
{
    public TripDetailPage(DriverTripResponse trip)
    {
        InitializeComponent();
        TripDetail.Trip = trip;
    }
}
