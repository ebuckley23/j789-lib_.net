using System;
using System.Collections.Generic;
using System.Linq;

namespace J789.Library.Core
{
    /// <summary>
    /// Enumeration base class that provides an alternative to using Enum types.
    /// To Note: Careful when defining static enums and utilizing static heavy libraries such as MassTransit as this
    /// causes issues with references
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types"/>
    /// </summary>
    public abstract class Enumeration : IComparable
    {
        public string Name { get; private set; }
        public int Id { get; private set; }
        public string Description { get; private set; }
        protected Enumeration(int id, string name, string description) 
            => (Id, Name, Description) = (id, name, description);

        public override string ToString() => Name;

        /// <summary>
        /// Get all Enumerations of specified type
        /// </summary>
        /// <typeparam name="TEnumeration"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TEnumeration> GetAll<TEnumeration>() where TEnumeration : Enumeration =>
            typeof(TEnumeration).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly)
            .Select(x => x.GetValue(null))
            .Cast<TEnumeration>();

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue) return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }
        public int CompareTo(object obj) 
            => Id.CompareTo(((Enumeration)obj).Id);

        public override int GetHashCode() 
            => Id.GetHashCode();

        public static bool operator ==(Enumeration left, Enumeration right)
        {
            if(ReferenceEquals(left, null))
            {
                if(ReferenceEquals(right, null))
                {
                    // null == null = true
                    return true;
                }
                // only left side is null
                return false;
            }
            return left.Equals(right);
        }
        public static bool operator !=(Enumeration left, Enumeration right) 
            => !left.Equals(right);
        public static int AbsoluteDifference(Enumeration first, Enumeration second) 
            => Math.Abs(first.Id - second.Id);
        public static TEnumeration FromId<TEnumeration>(int value) where TEnumeration : Enumeration
            => Parse<TEnumeration, int>(value, "Id", item => item.Id == value);

        public static TEnumeration FromName<TEnumeration>(string name) where TEnumeration : Enumeration
            => Parse<TEnumeration, string>(name, "Name", item => item.Name == name);

        private static TEnumeration Parse<TEnumeration, TValue>(TValue value, string description, Func<TEnumeration, bool> predicate) where TEnumeration : Enumeration
        {
            var matchingItem = GetAll<TEnumeration>().FirstOrDefault(predicate);
            if (matchingItem == null) throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(TEnumeration)}");
            return matchingItem;
        }

        public static implicit operator int(Enumeration e) => e.Id;
    }
}
