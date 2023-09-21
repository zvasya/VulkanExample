namespace VulkanView.Maui.Views.Primitives;

public sealed class ChangeSizeEventArgs : EventArgs
{
    public ChangeSizeEventArgs()
    {
    }
    
    public ChangeSizeEventArgs(int w, int h)
    {
        Width = w;
        Height = h;
    }

    public int? Width { get; }
    public int? Height { get; }
}
