using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CommandPipe
{
    /// <summary>
    /// Represents methods that can be used to check that parameter values meet expected conditions.
    /// </summary>
    [DebuggerStepThrough]
    internal static class Check
    {
        /// <summary>
        /// Check that the value of a parameter is not null.
        /// </summary>
        /// <typeparam name="T">Type type of the value.</typeparam>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="System.ArgumentNullException">If value or parameterName is null</exception>
        public static T NotNull<T>(T value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            return value;
        }

        /// <summary>
        /// Check that the value of a parameter is not null or default value of T.
        /// </summary>
        /// <typeparam name="T">Type type of the value.</typeparam>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="System.ArgumentNullException">If value or parameterName is null or default value</exception>
        public static T NotNullOrDefaultValue<T>(T value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            if (value.Equals(default(T)))
                throw new ArgumentNullException(parameterName);

            return value;
        }

        /// <summary>
        /// Check that the value of the string is not null or empty.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="System.ArgumentNullException">If value or parameterName is null</exception>
        public static string NotNullOrWhiteSpace(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(parameterName);

            return value;
        }

        /// <summary>
        /// Check that the value of the bool is not false.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="System.ArgumentNullException">If value or parameterName is false</exception>
        public static bool NotFalse(bool value, string parameterName)
        {
            if (!value)
                throw new ArgumentNullException(parameterName);

            return true;
        }

        /// <summary>
        /// Check that the value of a parameter is not null or empty.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="System.ArgumentNullException">If value or parameterName is null</exception>
        public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T> value, string parameterName)
        {
            if (value == null || !value.Any())
                throw new ArgumentNullException(parameterName);

            return value;
        }

        /// <summary>
        /// Check that the value of a first parameter is equal to the value of the second parameter.
        /// </summary>
        /// <typeparam name="T">Type type of the value.</typeparam>
        /// <typeparam name="TValue">Type type of the value to compare.</typeparam>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="compare">The value to compare.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="System.ArgumentNullException">If value or parameterName is null</exception>
        public static T NotEqual<T, TValue>(T value, TValue compare, string parameterName)
            where T : struct
        {
            if (value.Equals(compare))
                throw new ArgumentNullException(parameterName);

            return value;
        }

        /// <summary>
        /// Checks if a <see cref="TimeSpan"/> parameter is not zero or less.
        /// </summary>
        /// <param name="time">The <see cref="TimeSpan"/> parameter to be checked.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="time"/> is a <see cref="TimeSpan"/> with zero length o less.
        /// </exception>
        public static TimeSpan NotZeroOrLess(TimeSpan time, string parameterName)
        {
            if (time <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be greater than zero");

            return time;
        }

        /// <summary>
        /// Checks if a <see cref="int"/> parameter is not negative.
        /// </summary>
        /// <param name="value">The <see cref="int"/> parameter to be checked.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the parameter is negative.</exception>
        public static int NotNegative(int value, string parameterName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be greater or equal to zero");

            return value;
        }

        /// <summary>
        /// Checks if a <see cref="DateTime"/> parameter is not in the past.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> parameter to be checked.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DateTime NotInThePast(DateTime date, string parameterName)
        {
            if (date.ToUniversalTime() < DateTime.UtcNow)
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be a future date");

            return date;
        }
    }
}
