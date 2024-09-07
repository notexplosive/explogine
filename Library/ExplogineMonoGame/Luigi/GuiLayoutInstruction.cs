using ExplogineMonoGame.Luigi.Description;

namespace ExplogineMonoGame.Luigi;

/// <summary>
///     Instruction that that describes a Gui Element that should be drawn to a specific element id
/// </summary>
public record struct GuiLayoutInstruction(string ElementId, IGuiDescription GuiDescription);
