namespace Paraba.DriverApp.Controls;

public class BorderlessEntry : Entry
{
#if ANDROID
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler?.PlatformView is Android.Widget.EditText editText)
        {
            editText.Background = null;
            editText.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            editText.SetBackgroundColor(Android.Graphics.Color.Transparent);
            editText.SetPadding(0, 0, 0, 0);
        }
    }
#endif
}
