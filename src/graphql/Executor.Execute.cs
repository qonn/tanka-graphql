﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQLParser.AST;
using tanka.graphql.execution;

namespace tanka.graphql
{
    /// <summary>
    ///     Execute queries, mutations and subscriptions
    /// </summary>
    public static partial class Executor
    {
        /// <summary>
        ///     Execute query or mutation
        /// </summary>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<ExecutionResult> ExecuteAsync(
            ExecutionOptions options,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var extensions = new ExtensionsRunner(options.Extensions);
            await extensions.BeginExecuteAsync(options);
            var logger = options.LoggerFactory.CreateLogger(typeof(Executor).FullName);

            using (logger.Begin(options.OperationName))
            {
                var (queryContext, validationResult) = await BuildQueryContextAsync(
                    options,
                    extensions,
                    logger);

                if (!validationResult.IsValid)
                    return new ExecutionResult
                    {
                        Errors = validationResult.Errors.Select(e => e.ToError())
                            .ToList(),
                        Extensions = validationResult.Extensions.ToDictionary(kv => kv.Key, kv => kv.Value)
                    };

                ExecutionResult executionResult;
                switch (queryContext.OperationDefinition.Operation)
                {
                    case OperationType.Query:
                        executionResult = await Query.ExecuteQueryAsync(queryContext).ConfigureAwait(false);
                        break;
                    case OperationType.Mutation:
                        executionResult = await Mutation.ExecuteMutationAsync(queryContext).ConfigureAwait(false);
                        break;
                    case OperationType.Subscription:
                        throw new InvalidOperationException($"Use {nameof(SubscribeAsync)}");
                    default:
                        throw new InvalidOperationException(
                            $"Operation type {queryContext.OperationDefinition.Operation} not supported.");
                }

                if (validationResult.Extensions != null)
                    foreach (var validationExtension in validationResult.Extensions)
                        executionResult.AddExtension(validationExtension.Key, validationExtension.Value);

                logger.ExecutionResult(executionResult);
                await extensions.EndExecuteAsync(executionResult);
                return executionResult;
            }
        }
    }
}