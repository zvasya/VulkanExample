using System.Runtime.InteropServices;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Java.Interop;
using Shared;
using Silk.NET.Vulkan;
using VulkanView.Maui.Views.Interfaces;
using Silk.NET.Vulkan.Extensions.KHR;

namespace VulkanView.Maui.Core.Views;

// All the code in this file is only included on Android.
public sealed class MauiVulkanView : SurfaceView, ISurfaceHolderCallback
{
    // Is SurfaceView ready for rendering
    public bool mIsSurfaceReady;

    // public Instance Instance;
    IntPtr aNativeWindow = IntPtr.Zero;

    IVulkanView _vulkanView;
    Timer _timer;

    public MauiVulkanView(Context context, Maui.Views.VulkanView vulkanView) : base(context)
    {
        _vulkanView = vulkanView;
        _vulkanView.Platform = new AndroidPlatform();
        _vulkanView.CreateSurface = CreateSurface;

        Holder?.AddCallback(this);
        SetWillNotDraw(false);
    }


    SurfaceKHR CreateSurface(HelloEngine engine)
    {
        HelloEngine.CreateAndroidSurface(engine, aNativeWindow, out var surfaceKhr);
        return surfaceKhr;
    }


    void AcquireNativeWindow(ISurfaceHolder holder)
    {
        if (aNativeWindow != IntPtr.Zero)
            NativeMethods.ANativeWindow_release(aNativeWindow);

        aNativeWindow = NativeMethods.ANativeWindow_fromSurface(JniEnvironment.EnvironmentPointer, Holder.Surface.Handle);
        NativeWindowAcquired();
    }

    public void SurfaceCreated(ISurfaceHolder holder)
    {
        AcquireNativeWindow(holder);

        _timer = new Timer(_ => Invalidate(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));

        SetWillNotDraw(false);
    }

    public void SurfaceDestroyed(ISurfaceHolder holder)
    {
        if (aNativeWindow != IntPtr.Zero)
            NativeMethods.ANativeWindow_release(aNativeWindow);

        _timer?.Dispose();
        aNativeWindow = IntPtr.Zero;
    }

    protected override void OnDraw(Canvas? canvas)
    {
        base.OnDraw(canvas);
        _vulkanView.DrawFrame();
    }

    protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
    {
        base.OnSizeChanged(w, h, oldw, oldh);
        _vulkanView.ChangeSize();
    }

    public void SurfaceChanged(ISurfaceHolder holder, global::Android.Graphics.Format format, int w, int h)
    {
        _vulkanView.ChangeSize(w, h);
    }


    void NativeWindowAcquired()
    {
        _vulkanView.ViewCreated();
    }
}

public class AndroidPlatform : Shared.IPlatform
{
    public bool EnableValidationLayers => false;
    public string[] InstanceExtensions => new[] {KhrAndroidSurface.ExtensionName};
    public string[] DeviceExtensions => Array.Empty<string>();
}

internal static class NativeMethods
{
    const string AndroidRuntimeLibrary = "android";

    [DllImport(AndroidRuntimeLibrary)]
    internal static extern IntPtr ANativeWindow_fromSurface(IntPtr jniEnv, IntPtr handle);

    [DllImport(AndroidRuntimeLibrary)]
    internal static extern void ANativeWindow_release(IntPtr window);
}
