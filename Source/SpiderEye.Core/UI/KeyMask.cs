namespace SpiderEye
{
    /// <summary>
    /// Defines mask for different types of keys in <see cref="Key"/>.
    /// </summary>
    internal enum KeyMask
    {
        Function = 1 << 31,
        Number = 1 << 30,
        Alphabet = 1 << 29,
        Additional = 1 << 28,
    }
}
