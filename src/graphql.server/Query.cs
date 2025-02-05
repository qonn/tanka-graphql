﻿using System.Collections.Generic;
using GraphQLParser.AST;

namespace tanka.graphql.requests
{
    public class Query
    {
        public GraphQLDocument Document { get; set; }

        public Dictionary<string, object> Variables { get; set; }

        public string OperationName { get; set; }

        public Dictionary<string, object> Extensions { get; set; }
    }
}