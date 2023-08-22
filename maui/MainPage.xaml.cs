using Shared;

namespace maui;

public partial class MainPage : ContentPage
{
	uint count = 0;

	public MainPage()
	{
		InitializeComponent();
		//count = VulkanLoader.CreateVulkan(Array.Empty<string>());
    }

	private void OnCounterClicked(object sender, EventArgs e)
	{
		CounterBtn.Text = $"Devices count: {count}";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}


