using System.Collections.Generic;

namespace tanka.graphql.type.validation
{
    public class SchemaValidationContext
    {
        public SchemaValidationContext(ISchema schema)
        {
            Schema = schema;
        }

        public ISchema Schema { get; }

        public IEnumerable<SchemaValidationError> Errors { get; set; }

        public class SchemaValidationError
        {
            private readonly List<object> _nodes = new List<object>();

            public SchemaValidationError(string message, params object[] nodes)
            {
                Message = message;
                _nodes.AddRange(nodes);                    
            }

            public string Message { get; }

            public IEnumerable<object> Nodes => _nodes;
        }
    }
}