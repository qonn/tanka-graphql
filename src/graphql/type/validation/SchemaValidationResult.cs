using System;
using System.Collections.Generic;
using System.Linq;

namespace tanka.graphql.type.validation
{
    public class SchemaValidationResult
    {
        public SchemaValidationResult(IEnumerable<SchemaValidationContext.SchemaValidationError> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public IEnumerable<SchemaValidationContext.SchemaValidationError> Errors { get; }

        public bool IsValid => !Errors.Any();
    }
}