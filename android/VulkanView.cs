using Android.Content;
using Android.Views;

using Java.Interop;
using System.Runtime.InteropServices;
using Android.Graphics;
using Shared;
using Surface = Shared.Surface;

namespace android;

public sealed class VulkanView : SurfaceView, ISurfaceHolderCallback
{
    // public Instance Instance;
    IntPtr _aNativeWindow = IntPtr.Zero;
    readonly HelloEngine _engine;
    Surface _surface;

    public VulkanView(Context context, HelloEngine engine) : base(context)
    {
        _engine = engine;
        Holder!.AddCallback(this);
        SetWillNotDraw(false);
    }

    void AcquireNativeWindow(ISurfaceHolder holder)
    {
        if (_aNativeWindow != IntPtr.Zero)
            NativeMethods.ANativeWindow_release(_aNativeWindow);

        _aNativeWindow = NativeMethods.ANativeWindow_fromSurface(JniEnvironment.EnvironmentPointer, Holder.Surface.Handle);
        NativeWindowAcquired();
    }

    public void SurfaceCreated(ISurfaceHolder holder)
    {
        AcquireNativeWindow(holder);
        var r = new Timer(state => Invalidate(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
    }

    public void SurfaceDestroyed(ISurfaceHolder holder)
    {
        if (_aNativeWindow != IntPtr.Zero)
            NativeMethods.ANativeWindow_release(_aNativeWindow);

        _aNativeWindow = IntPtr.Zero;
    }

    protected override void OnDraw(Canvas? canvas)
    {
        base.OnDraw(canvas);
        _surface.DrawFrame();
    }

    public void SurfaceChanged(ISurfaceHolder holder, Format format, int w, int h)
    {
        _surface.ChangeSize();
    }

    void NativeWindowAcquired()
    {
        _surface = _engine.CreateSurface(() =>
        {
            HelloEngine.CreateAndroidSurface(_engine, _aNativeWindow, out var surfaceKhr);
            return surfaceKhr;
        });
    }

    // protected void CreateDefaultInstance ()
    // {
    //     Instance = new Instance (new InstanceCreateInfo () {
    //         EnabledExtensionNames = new string [] { "VK_KHR_surface", "VK_KHR_android_surface" },
    //         ApplicationInfo = new ApplicationInfo () {
    //             ApiVersion = Vulkan.Version.Make (1, 0, 0)
    //         }
    //     });
    // }
}

internal static class NativeMethods
{
    const string AndroidRuntimeLibrary = "android";

    [DllImport(AndroidRuntimeLibrary)]
    internal static extern IntPtr ANativeWindow_fromSurface(IntPtr jniEnv, IntPtr handle);

    [DllImport(AndroidRuntimeLibrary)]
    internal static extern void ANativeWindow_release(IntPtr window);
}
