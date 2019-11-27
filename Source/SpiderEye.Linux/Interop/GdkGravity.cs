namespace SpiderEye.Linux.Interop
{
    internal enum GdkGravity
    {
        /// <summary>
        /// The reference point is at the top left corner.
        /// </summary>
        NorthWest = 1,

        /// <summary>
        /// The reference point is in the middle of the top edge.
        /// </summary>
        North,

        /// <summary>
        /// The reference point is at the top right corner.
        /// </summary>
        NorthEast,

        /// <summary>
        /// The reference point is at the middle of the left edge.
        /// </summary>
        West,

        /// <summary>
        /// The reference point is at the center of the window.
        /// </summary>
        Center,

        /// <summary>
        /// The reference point is at the middle of the right edge.
        /// </summary>
        East,

        /// <summary>
        /// The reference point is at the lower left corner.
        /// </summary>
        SouthWest,

        /// <summary>
        /// The reference point is at the middle of the lower edge.
        /// </summary>
        South,

        /// <summary>
        /// The reference point is at the lower right corner.
        /// </summary>
        SouthEast,

        /// <summary>
        /// The reference point is at the top left corner of the window itself, ignoring window manager decorations.
        /// </summary>
        Static,
    }
}
