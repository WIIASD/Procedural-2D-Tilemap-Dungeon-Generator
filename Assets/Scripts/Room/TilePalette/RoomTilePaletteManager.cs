using System.Collections.Generic;

/// <summary>
/// Manager class that manages palette scriptable objects
/// </summary>
public class RoomTilePaletteManager : ZMonoSingleton<RoomTilePaletteManager>
{
    public List<RoomTilePalette> Palettes;
    public int currentPaletteIndex;

    public RoomTilePalette GetCurrentPalette() => Palettes[currentPaletteIndex];

    protected override void Awake()
    {
        base.Awake();
        currentPaletteIndex = 0;
        foreach (RoomTilePalette p in Palettes)
        {
            p.InitializeTilesLists();
        }
    }

    public void NextPalette()
    {
        if(Palettes.Count !> 0)
        {
            print("No palette in the manager u idiot");
            return;
        }
        if (currentPaletteIndex < Palettes.Count - 1)
        {
            currentPaletteIndex++;
        }
        else
        {
            currentPaletteIndex = 0;
        }
    }
}
