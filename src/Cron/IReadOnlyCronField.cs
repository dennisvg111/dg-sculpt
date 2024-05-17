using System.Collections.Generic;

namespace DG.Sculpt.Cron
{
    /// <summary>
    /// Represents a single field in a <see cref="CronExpression"/>.
    /// </summary>
    public interface IReadOnlyCronField
    {
        /// <summary>
        /// Indicates if this field matches any number (e.g. a wildcard).
        /// </summary>
        bool IsWildcard { get; }

        /// <summary>
        /// A list of all values that are allowed for this field. This collection is guaranteed to be in ascending order.
        /// </summary>
        IReadOnlyList<int> AllowedValues { get; }

        /// <summary>
        /// Indicates if this field matches the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool CanBe(int value);

        /// <summary>
        /// Returns the lowest possible value this field can be.
        /// </summary>
        /// <returns></returns>
        int GetLowestValue();

        /// <summary>
        /// Tries to find the lowest possible value this field can be that is higher than or equal to <paramref name="atLeast"/>, and returns a <see cref="bool"/> indicating if such a value exists.
        /// </summary>
        /// <param name="atLeast"></param>
        /// <param name="foundValue">The found value.</param>
        /// <returns></returns>
        bool TryGetLowestOfAtLeast(int atLeast, out int foundValue);

        /// <summary>
        /// Returns a text representation of this field.
        /// </summary>
        /// <returns></returns>
        string AsString();
    }
}
