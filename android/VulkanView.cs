using Android.Content;
using Android.Views;
using Java.Interop;
using System.Runtime.InteropServices;
using Android.Graphics;
using Examples;
using Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xamarin.Essentials;
using Surface = Shared.Surface;

namespace android;

public sealed class VulkanView : SurfaceView, ISurfaceHolderCallback
{
    // public Instance Instance;
    IntPtr _aNativeWindow = IntPtr.Zero;
    readonly HelloEngine _engine;
    Surface _surface;
    Example1 _example1;
    Timer _timer;

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
        _timer = new Timer(state => Invalidate(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
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
        _surface.Update();
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
        _example1 = new Example1(_surface, GetVertShader, GetFragShader, GetImage1, GetImage2);
    }

    static byte[] GetVertShader() => Read("Shaders/vert.spv");
    static byte[] GetFragShader() => Read("Shaders/frag.spv");

    static byte[] Read(string fileName)
    {
        using var stream = FileSystem.OpenAppPackageFileAsync(fileName);
        using var memStream = new MemoryStream();
        stream.Result.CopyTo(memStream);
        return memStream.ToArray();
    }

    static Image<Rgba32> GetImage1()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("Textures/texture.jpg");
        return Image.Load<Rgba32>(stream.Result);
    }

    static Image<Rgba32> GetImage2()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("Textures/texture2.jpg");
        return Image.Load<Rgba32>(stream.Result);
    }
}

internal static class NativeMethods
{
    const string AndroidRuntimeLibrary = "android";

    [DllImport(AndroidRuntimeLibrary)]
    internal static extern IntPtr ANativeWindow_fromSurface(IntPtr jniEnv, IntPtr handle);

    [DllImport(AndroidRuntimeLibrary)]
    internal static extern void ANativeWindow_release(IntPtr window);
}
