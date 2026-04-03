using BgituGradesLoader.Menu;
using BgituGradesLoader.Menu.Panels;
using BgituGradesLoader.Save;
using BgituGradesLoader.Table;

if (args.Contains("--headless"))
{
    SaveManager saveManager = await SaveManager.CreateFromApiAsync();
    TableManager tableManager = new(saveManager);
    await DataLoaderPanel.RunHeadless(saveManager, tableManager);
    return;
}

MenuManager consoleManager = new();
await consoleManager.Run();