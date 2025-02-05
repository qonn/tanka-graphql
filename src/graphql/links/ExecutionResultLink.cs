﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using GraphQLParser.AST;

namespace tanka.graphql.links
{
    public delegate ValueTask<ChannelReader<ExecutionResult>> ExecutionResultLink(
        GraphQLDocument document,
        IReadOnlyDictionary<string, object> variables,
        CancellationToken cancellationToken);
}