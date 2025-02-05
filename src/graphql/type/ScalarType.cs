﻿using System;
using System.Collections.Generic;
using GraphQLParser.AST;
using tanka.graphql.type.converters;

namespace tanka.graphql.type
{
    public class ScalarType : INamedType, IValueConverter, IEquatable<ScalarType>, IEquatable<INamedType>, IDescribable,
        IHasDirectives
    {
        public static ScalarType Boolean = new ScalarType(
            "Boolean",
            new BooleanConverter(),
            "The `Boolean` scalar type represents `true` or `false`");

        public static ScalarType Float = new ScalarType(
            "Float",
            new DoubleConverter(),
            "The `Float` scalar type represents signed double-precision fractional values" +
            " as specified by '[IEEE 754](http://en.wikipedia.org/wiki/IEEE_floating_point)");

        public static ScalarType ID = new ScalarType(
            "ID",
            new IdConverter(),
            "The ID scalar type represents a unique identifier, often used to refetch an object" +
            " or as the key for a cache. The ID type is serialized in the same way as a String; " +
            "however, it is not intended to be human‐readable. While it is often numeric, it " +
            "should always serialize as a String.");

        public static ScalarType Int = new ScalarType(
            "Int",
            new IntConverter(),
            "The `Int` scalar type represents non-fractional signed whole numeric values");

        public static NonNull NonNullBoolean = new NonNull(Boolean);
        public static NonNull NonNullFloat = new NonNull(Float);
        public static NonNull NonNullID = new NonNull(ID);
        public static NonNull NonNullInt = new NonNull(Int);

        public static ScalarType String = new ScalarType(
            "String",
            new StringConverter(),
            "The `String` scalar type represents textual data, represented as UTF-8" +
            " character sequences. The String type is most often used by GraphQL to" +
            " represent free-form human-readable text.");

        public static NonNull NonNullString = new NonNull(String);

        public static IEnumerable<ScalarType> Standard = new[]
        {
            String,
            Int,
            Float,
            Boolean,
            ID
        };


        private readonly DirectiveList _directives;


        public ScalarType(
            string name,
            IValueConverter converter,
            string description = null,
            IEnumerable<DirectiveInstance> directives = null)
        {
            Name = name;
            Converter = converter;
            Description = description ?? string.Empty;
            _directives = new DirectiveList(directives);
        }

        protected IValueConverter Converter { get; }

        public string Description { get; }

        public IEnumerable<DirectiveInstance> Directives => _directives;

        public DirectiveInstance GetDirective(string name)
        {
            return _directives.GetDirective(name);
        }

        public bool HasDirective(string name)
        {
            return _directives.HasDirective(name);
        }

        public bool Equals(INamedType other)
        {
            return Equals((object) other);
        }

        public bool Equals(ScalarType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public string Name { get; }

        public object Serialize(object value)
        {
            return Converter.Serialize(value);
        }

        public object ParseValue(object input)
        {
            return Converter.ParseValue(input);
        }

        public object ParseLiteral(GraphQLScalarValue input)
        {
            return Converter.ParseLiteral(input);
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((ScalarType) obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public static bool operator ==(ScalarType left, ScalarType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScalarType left, ScalarType right)
        {
            return !Equals(left, right);
        }
    }
}