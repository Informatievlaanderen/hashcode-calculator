namespace Be.Vlaanderen.Basisregisters.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// IEnumerable&lt;object&gt; HashCodeFields(ImportMunicipalityNameFromCrab x)
    /// {
    ///     yield return x.MunicipalityId;
    ///     yield return x.MunicipalityName;
    ///     yield return x.Lifetime;
    ///     yield return x.Timestamp;
    ///     yield return x.Operator;
    ///     yield return x.Modification;
    ///     yield return x.Organisation;
    ///     yield return x.MunicipalityNameId;
    /// }
    ///
    /// return command.GetHashCode(HashCodeFields);
    /// </summary>
    public static class HashCodeCalculator
    {
        // T.GetHashCode(t => ....)
        public static int GetHashCode<T>(this T x, Func<T, IEnumerable<object>> hashFieldValuesFunc) => GetHashCode(hashFieldValuesFunc(x));

        // HashCodeCalculator.GetHashCode(() => ....)
        public static int GetHashCode(Func<IEnumerable<object>> hashFieldValuesFunc) => GetHashCode(hashFieldValuesFunc());

        // HashCodeCalculator.GetHashCode(....)
        public static int GetHashCode(IEnumerable<object> hashFieldValues)
        {
            // Naive .NET implementation
            //var offset = 17;
            //var prime = 59;
            //
            //int HashCodeAggregator(int hashCode, object value) => value == null
            //    ? hashCode * prime + 0
            //    : hashCode * prime + value.GetHashCode();

            // http://www.isthe.com/chongo/tech/comp/fnv/index.html
            const int offset = unchecked((int) 2166136261);
            const int prime = 16777619;

            int HashCodeAggregator(int hashCode, object value) => value == null
                ? (hashCode ^ 0) * prime
                : (hashCode ^ value.GetHashCode()) * prime;

            return hashFieldValues.Aggregate(offset, HashCodeAggregator);
        }
    }
}
