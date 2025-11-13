using NAudio.Wave;

namespace Radio357Player;

class Program
{
    private static IWavePlayer waveOut;
    private static MediaFoundationReader mediaReader;
    private static bool isPlaying = false;

    static async Task Main(string[] args)
    {
        Console.WriteLine("=================================");
        Console.WriteLine("   Radio 357 Console Player");
        Console.WriteLine("=================================");
        Console.WriteLine();

        // Radio 357 stream URLs
        string mp3StreamUrl = "https://stream.rcs.revma.com/an1ugyygzk8uv";
        string aacStreamUrl = "https://stream.rcs.revma.com/ye5kghkgcm0uv";

        Console.WriteLine("Select stream format:");
        Console.WriteLine("1. MP3 (128 kbps)");
        Console.WriteLine("2. AAC");
        Console.Write("\nEnter choice (1 or 2): ");
        
        string choice = Console.ReadLine();
        string streamUrl = choice == "2" ? aacStreamUrl : mp3StreamUrl;

        Console.WriteLine($"\nConnecting to Radio 357 ({(choice == "2" ? "AAC" : "MP3")} stream)...");
        Console.WriteLine();

        try
        {
            await PlayStreamAsync(streamUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
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

                Console.WriteLine("Now playing Radio 357!");
                Console.WriteLine("Press 'Q' to quit, 'P' to pause/resume, '+/-' to adjust volume");
                Console.WriteLine();

                float volume = 1.0f;

                // Main loop for user commands
                while (isPlaying)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);

                        switch (key.Key)
                        {
                            case ConsoleKey.Q:
                                Console.WriteLine("\nStopping playback...");
                                StopPlayback();
                                isPlaying = false;
                                break;

                            case ConsoleKey.P:
                                if (waveOut.PlaybackState == PlaybackState.Playing)
                                {
                                    waveOut.Pause();
                                    Console.WriteLine("Paused");
                                }
                                else if (waveOut.PlaybackState == PlaybackState.Paused)
                                {
                                    waveOut.Play();
                                    Console.WriteLine("Resumed");
                                }
                                break;

                            case ConsoleKey.OemPlus:
                            case ConsoleKey.Add:
                                volume = Math.Min(1.0f, volume + 0.1f);
                                waveOut.Volume = volume;
                                Console.WriteLine($"Volume: {(int)(volume * 100)}%");
                                break;

                            case ConsoleKey.OemMinus:
                            case ConsoleKey.Subtract:
                                volume = Math.Max(0.0f, volume - 0.1f);
                                waveOut.Volume = volume;
                                Console.WriteLine($"Volume: {(int)(volume * 100)}%");
                                break;
                        }
                    }

                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
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