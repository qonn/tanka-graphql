﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tanka.graphql.type
{
    public class TypeScanner
    {
        private readonly IGraphQLType _root;

        public TypeScanner(IGraphQLType root)
        {
            _root = root;
        }

        public virtual async Task<IEnumerable<IGraphQLType>> ScanAsync()
        {
            var foundTypes = new ConcurrentBag<IGraphQLType>();

            await ScanTypeAsync(foundTypes, _root).ConfigureAwait(false);

            return foundTypes;
        }

        private async Task ScanTypeAsync(ConcurrentBag<IGraphQLType> foundTypes, IGraphQLType type)
        {
            if (type == null)
                return;

            if (foundTypes.Contains(type, new GraphQLTypeComparer()))
                return;

            if (type is NamedTypeReference)
                return;

            if (type is IWrappingType wrapper)
            {
                await ScanTypeAsync(foundTypes, wrapper.WrappedType).ConfigureAwait(false);
                return;
            }

            foundTypes.Add(type);

            if (type is ComplexType complexType)
            {
                if (complexType is ObjectType objectType)
                {
                    var interfaceTypes = objectType.Interfaces;

                    foreach (var interfaceType in interfaceTypes)
                    {
                        await ScanTypeAsync(foundTypes, interfaceType).ConfigureAwait(false);
                    }
                }

                foreach (var field in complexType.Fields)
                {
                    await ScanTypeAsync(foundTypes, field.Value.Type).ConfigureAwait(false);

                    foreach (var argument in field.Value.Arguments)
                    {
                        await ScanTypeAsync(foundTypes, argument.Value.Type).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}