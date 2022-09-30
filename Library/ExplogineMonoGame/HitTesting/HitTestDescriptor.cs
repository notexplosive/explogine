using System;

namespace ExplogineMonoGame.HitTesting;

/// <summary>
/// The ID of a HitTestTarget for this frame.
/// </summary>
/// <param name="Value">Internal value</param>
/// <param name="Callback">Internal value</param>
public readonly record struct HitTestDescriptor(int Value, Action? Callback);
