﻿using System.Threading.Tasks;

namespace tanka.graphql
{
    public interface IExecutorExtension
    {
        //todo: change to ValueTask ?
        Task<IExtensionScope> BeginExecuteAsync(ExecutionOptions options);
    }
}