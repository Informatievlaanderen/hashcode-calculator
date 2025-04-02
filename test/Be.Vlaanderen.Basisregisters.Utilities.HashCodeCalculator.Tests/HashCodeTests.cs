namespace Be.Vlaanderen.Basisregisters.Utilities.HashCodeCalculatorTests
{
    using System;
    using System.Collections.Generic;
    using Shouldly;
    using Xunit;

    public class HashCodeTests
    {
        private interface IHashCodeFields
        {
            IEnumerable<object?> HashCodeFields();
        }

        private class Address : IHashCodeFields
        {
            public string? Address1 { get; }

            public string? City { get; }

            public string? State { get; }

            public Address(string? address1, string? city, string? state)
            {
                Address1 = address1;
                City = city;
                State = state;
            }

            public IEnumerable<object?> HashCodeFields()
            {
                yield return Address1;
                yield return City;
                yield return State;
            }

            public override int GetHashCode()
                => HashCodeCalculator.GetHashCode(HashCodeFields());
        }

        private class ExpandedAddress : Address, IHashCodeFields
        {
            public string Address2 { get; }

            public ExpandedAddress(string address1, string address2, string city, string state)
                : base(address1, city, state)
            {
                Address2 = address2;
            }

            public new IEnumerable<object> HashCodeFields()
            {
                yield return base.HashCodeFields();
                yield return Address2;
            }

            public override int GetHashCode()
                => HashCodeCalculator.GetHashCode(HashCodeFields());
        }

        private static void TestAll<T>(
            T a,
            T b,
            Action<int, int> assertFunc,
            Func<T, IEnumerable<object?>> localHashCodeFields)
            where T : IHashCodeFields
        {
            // T.GetHashCode(t => ....)
            var hashA = a.GetHashCode(x => x.HashCodeFields());
            var hashB = b.GetHashCode(x => x.HashCodeFields());
            assertFunc(hashA, hashB);

            // T.GetHashCode(t => ....)
            hashA = HashCodeCalculator.GetHashCode(a, localHashCodeFields);
            hashB = HashCodeCalculator.GetHashCode(b, localHashCodeFields);
            assertFunc(hashA, hashB);

            // HashCodeCalculator.GetHashCode(() => ....)
            hashA = HashCodeCalculator.GetHashCode(() => a.HashCodeFields());
            hashB = HashCodeCalculator.GetHashCode(() => b.HashCodeFields());
            assertFunc(hashA, hashB);

            // HashCodeCalculator.GetHashCode(....)
            hashA = HashCodeCalculator.GetHashCode(a.HashCodeFields());
            hashB = HashCodeCalculator.GetHashCode(b.HashCodeFields());
            assertFunc(hashA, hashB);
        }

        IEnumerable<object?> LocalHashCodeFields(Address x)
        {
            yield return x.Address1;
            yield return x.City;
            yield return x.State;
        }

        IEnumerable<object?> LocalHashCodeFields(ExpandedAddress x)
        {
            yield return ((Address)x).HashCodeFields();
            yield return x.Address2;
        }

        [Fact]
        public void EqualValueObjectsHaveSameHashCode()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address1", "Austin", "TX");

            TestAll(address, address2, (hashA, hashB) => hashA.ShouldBe(hashB), LocalHashCodeFields);
        }

        [Fact]
        public void TransposedValuesGiveDifferentHashCodes()
        {
            var address = new Address(null, "Austin", "TX");
            var address2 = new Address("TX", "Austin", null);

            TestAll(address, address2, (hashA, hashB) => hashA.ShouldNotBe(hashB), LocalHashCodeFields);
        }

        [Fact]
        public void UnequalValueObjectsHaveDifferentHashCodes()
        {
            var address = new Address("Address1", "Austin", "TX");
            var address2 = new Address("Address2", "Austin", "TX");

            TestAll(address, address2, (hashA, hashB) => hashA.ShouldNotBe(hashB), LocalHashCodeFields);
        }

        [Fact]
        public void TransposedValuesOfFieldNamesGivesDifferentHashCodes()
        {
            var address = new Address("_city", null, null);
            var address2 = new Address(null, "_address1", null);

            TestAll(address, address2, (hashA, hashB) => hashA.ShouldNotBe(hashB), LocalHashCodeFields);
        }

        [Fact]
        public void DerivedTypesHashCodesBehaveCorrectly()
        {
            var address = new ExpandedAddress("Address99999", "Apt 123", "New Orleans", "LA");
            var address2 = new ExpandedAddress("Address1", "Apt 123", "Austin", "TX");

            TestAll(address, address2, (hashA, hashB) => hashA.ShouldNotBe(hashB), LocalHashCodeFields);
        }

        [Fact]
        public void DerivedTypesHashCodesBehaveCorrectly2()
        {
            var address = new ExpandedAddress("Address99999", "Apt 123", "New Orleans", "LA");
            var address2 = new ExpandedAddress("Address99999", "Apt x", "New Orleans", "LA");

            TestAll(address, address2, (hashA, hashB) => hashA.ShouldNotBe(hashB), LocalHashCodeFields);
        }
    }
}
