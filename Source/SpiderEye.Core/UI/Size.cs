using System;

namespace SpiderEye
{
    /// <summary>
    /// Stores a width and height of something.
    /// </summary>
    public readonly struct Size : IEquatable<Size>
    {
        /// <summary>
        /// A <see cref="Size"/> with <see cref="Width"/> and <see cref="Height"/> set to zero.
        /// </summary>
        public static readonly Size Zero = new Size(0, 0);

        /// <summary>
        /// The width.
        /// </summary>
        public readonly double Width;

        /// <summary>
        /// The height.
        /// </summary>
        public readonly double Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Size size && Equals(size);
        }

        /// <inheritdoc/>
        public bool Equals(Size other)
        {
            return Width == other.Width &&
                   Height == other.Height;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height);
        }

        /// <summary>
        /// Compares if two <see cref="Size"/> are equal.
        /// </summary>
        /// <param name="left">The left size.</param>
        /// <param name="right">The right size.</param>
        /// <returns>True if they are equal; False otherwise.</returns>
        public static bool operator ==(Size left, Size right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares if two <see cref="Size"/> are not equal.
        /// </summary>
        /// <param name="left">The left size.</param>
        /// <param name="right">The right size.</param>
        /// <returns>True if they are not equal; False otherwise.</returns>
        public static bool operator !=(Size left, Size right)
        {
            return !(left == right);
        }
    }
}
