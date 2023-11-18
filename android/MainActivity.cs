using Android.Content.PM;
using Shared;

namespace android;

[Activity(Label = "@string/app_name", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
public class MainActivity : Activity
{
    HelloEngine? _engine;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        _engine = HelloEngine.Create(new AndroidPlatform());

        var layout = FindViewById<LinearLayout>(Resource.Id.Layout);
        var vulkanView = new VulkanView(ApplicationContext!, _engine);
        layout!.AddView(vulkanView);
    }
}
