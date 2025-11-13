namespace Radio357Player;

public class ConsoleMenu
{
    private readonly string title;
    private readonly List<MenuItem> menuItems;
    private int selectedIndex;
    private readonly bool clearOnExit;

    public ConsoleMenu(string title, List<MenuItem> menuItems, bool clearOnExit = true)
    {
        this.title = title;
        this.menuItems = menuItems;
        this.selectedIndex = 0;
        this.clearOnExit = clearOnExit;
    }

    public MenuItem? Show()
    {
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            DisplayMenu();

            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = selectedIndex > 0 ? selectedIndex - 1 : menuItems.Count - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = selectedIndex < menuItems.Count - 1 ? selectedIndex + 1 : 0;
                    break;

                case ConsoleKey.Enter:
                    Console.CursorVisible = true;
                    if (clearOnExit)
                    {
                        Console.Clear();
                    }
                    return menuItems[selectedIndex];

                case ConsoleKey.Escape:
                    Console.CursorVisible = true;
                    if (clearOnExit)
                    {
                        Console.Clear();
                    }
                    return null;
            }
        }
    }

    public MenuItem? ShowNonBlocking(Action<string>? statusDisplayCallback = null)
    {
        Console.CursorVisible = false;
        DisplayMenu(statusDisplayCallback);

        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = selectedIndex > 0 ? selectedIndex - 1 : menuItems.Count - 1;
                    break;

                case ConsoleKey.DownArrow:
                    selectedIndex = selectedIndex < menuItems.Count - 1 ? selectedIndex + 1 : 0;
                    break;

                case ConsoleKey.Enter:
                    return menuItems[selectedIndex];

                case ConsoleKey.Escape:
                    return new MenuItem("Back", "back");
            }
        }

        return null;
    }

    private void DisplayMenu(Action<string>? statusDisplayCallback = null)
    {
        // Display title
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔" + new string('═', title.Length + 2) + "╗");
        Console.WriteLine("║ " + title + " ║");
        Console.WriteLine("╚" + new string('═', title.Length + 2) + "╝");
        Console.ResetColor();
        Console.WriteLine();

        // Display status if callback provided
        if (statusDisplayCallback != null)
        {
            statusDisplayCallback("status");
            Console.WriteLine();
        }

        // Display menu items
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write("  ► ");
            }
            else
            {
                Console.Write("    ");
            }

            Console.Write(menuItems[i].DisplayText);

            if (i == selectedIndex)
            {
                Console.ResetColor();
            }

            Console.WriteLine();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Use ↑↓ arrows to navigate, Enter to select, Esc to cancel");
        Console.ResetColor();
    }
}

public class MenuItem
{
    public string DisplayText { get; set; }
    public string Value { get; set; }
    public object? Tag { get; set; }

    public MenuItem(string displayText, string value, object? tag = null)
    {
        DisplayText = displayText;
        Value = value;
        Tag = tag;
    }
}
