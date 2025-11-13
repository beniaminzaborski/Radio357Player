using NAudio.Wave;

namespace Radio357Player;

class Program
{
    private static IWavePlayer? waveOut;
    private static MediaFoundationReader? mediaReader;
    private static bool isPlaying = false;

    static async Task Main(string[] args)
    {
        // Radio 357 stream URLs
        string mp3StreamUrl = "https://stream.rcs.revma.com/an1ugyygzk8uv";
        string aacStreamUrl = "https://stream.rcs.revma.com/ye5kghkgcm0uv";

        // Create menu
        var menuItems = new List<MenuItem>
        {
            new MenuItem("MP3 Stream (128 kbps)", "mp3", mp3StreamUrl),
            new MenuItem("AAC Stream", "aac", aacStreamUrl),
            new MenuItem("Exit", "exit")
        };

        var menu = new ConsoleMenu("Radio 357 Console Player", menuItems);
        var selectedItem = menu.Show();

        if (selectedItem == null || selectedItem.Value == "exit")
        {
            return;
        }

        string? streamUrl = selectedItem.Tag as string;
        if (string.IsNullOrEmpty(streamUrl))
        {
            return;
        }

        try
        {
            await PlayStreamAsync(streamUrl);
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    static async Task PlayStreamAsync(string streamUrl)
    {
        await Task.Run(() =>
        {
            try
            {
                // Initialize the media reader with the stream URL
                mediaReader = new MediaFoundationReader(streamUrl);

                // Initialize the wave output device
                waveOut = new WaveOutEvent();
                waveOut.Init(mediaReader);

                // Start playback
                waveOut.Play();
                isPlaying = true;

                float volume = 1.0f;
                waveOut.Volume = volume;

                // Create control menu
                var controlMenu = new ConsoleMenu("Now Playing: Radio 357", new List<MenuItem>
                {
                    new MenuItem("Pause", "pause"),
                    new MenuItem("Volume Up (+10%)", "volume_up"),
                    new MenuItem("Volume Down (-10%)", "volume_down"),
                    new MenuItem("Stop & Quit", "quit")
                }, clearOnExit: false);

                // Main loop for user commands
                while (isPlaying)
                {
                    Console.SetCursorPosition(0, 0);

                    var selectedItem = controlMenu.ShowNonBlocking((status) =>
                    {
                        string playbackState = waveOut.PlaybackState == PlaybackState.Playing ? "▶ Playing" : "⏸ Paused";
                        Console.WriteLine($"Status: {playbackState} | Volume: {(int)(volume * 100)}%");
                    });

                    if (selectedItem != null)
                    {
                        switch (selectedItem.Value)
                        {
                            case "pause":
                                if (waveOut.PlaybackState == PlaybackState.Playing)
                                {
                                    waveOut.Pause();
                                    // Update menu item text
                                    selectedItem.DisplayText = "Resume";
                                    selectedItem.Value = "resume";
                                }
                                else if (waveOut.PlaybackState == PlaybackState.Paused)
                                {
                                    waveOut.Play();
                                    // Update menu item text
                                    selectedItem.DisplayText = "Pause";
                                    selectedItem.Value = "pause";
                                }
                                break;

                            case "resume":
                                waveOut.Play();
                                selectedItem.DisplayText = "Pause";
                                selectedItem.Value = "pause";
                                break;

                            case "volume_up":
                                volume = Math.Min(1.0f, volume + 0.1f);
                                waveOut.Volume = volume;
                                break;

                            case "volume_down":
                                volume = Math.Max(0.0f, volume - 0.1f);
                                waveOut.Volume = volume;
                                break;

                            case "quit":
                                StopPlayback();
                                isPlaying = false;
                                Console.Clear();
                                break;
                        }
                    }

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine($"Playback error: {ex.Message}");
                StopPlayback();
            }
        });
    }

    static void StopPlayback()
    {
        if (waveOut != null)
        {
            waveOut.Stop();
            waveOut.Dispose();
            waveOut = null;
        }

        if (mediaReader != null)
        {
            mediaReader.Dispose();
            mediaReader = null;
        }
    }
}