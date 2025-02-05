﻿using System;
using System.Collections.Generic;
using System.Linq;
using GraphQLParser.AST;
using tanka.graphql.execution;
using tanka.graphql.type;
using tanka.graphql.validation;

namespace tanka.graphql.extensions.analysis
{
    public static class CostAnalyzer
    {
        public static DirectiveType CostDirective = new DirectiveType(
            "cost",
            new[]
            {
                DirectiveLocation.FIELD_DEFINITION
            },
            new Args
            {
                {"complexity", ScalarType.NonNullInt},
                {"multipliers", new List(ScalarType.NonNullString)}
            });


        public static CombineRule MaxCost(
            uint maxCost,
            uint defaultFieldComplexity = 1,
            bool addExtensionData = false,
            Action<(IRuleVisitorContext Context, GraphQLOperationDefinition Operation, uint Cost, uint MaxCost)>
                onCalculated = null
        )
        {
            return (context, rule) =>
            {
                uint cost = 0;
                rule.EnterOperationDefinition += node => { cost = 0; };
                rule.EnterFieldSelection += node =>
                {
                    var fieldDef = context.Tracker.GetFieldDef();

                    if (fieldDef.HasValue)
                    {
                        var field = fieldDef.Value.Field;
                        var costDirective = field.GetDirective("cost");

                        if (costDirective != null)
                        {
                            var complexity = costDirective.GetArgument<int>("complexity");
                            var multipliers = costDirective.GetArgument<IEnumerable<object>>("multipliers");

                            if (multipliers != null)
                                foreach (var multiplier in multipliers)
                                {
                                    var multiplierName = multiplier.ToString();
                                    var multiplierArgDef = field.GetArgument(multiplierName);

                                    if (multiplierArgDef == null)
                                        continue;

                                    var multiplierArg =
                                        node.Arguments.SingleOrDefault(a => a.Name.Value == multiplierName);

                                    if (multiplierArg == null)
                                        continue;

                                    var multiplierValue = (int) Arguments.CoerceArgumentValue(
                                        context.Schema,
                                        context.VariableValues,
                                        multiplierName,
                                        multiplierArgDef,
                                        multiplierArg);

                                    complexity *= multiplierValue;
                                }

                            cost += (uint) complexity;
                        }
                        else
                        {
                            cost += defaultFieldComplexity;
                        }
                    }
                };
                rule.LeaveOperationDefinition += node =>
                {
                    onCalculated?.Invoke((context, node, cost, maxCost));

                    if (addExtensionData)
                        context.Extensions.Set("cost", new
                        {
                            Cost = cost,
                            MaxCost = maxCost
                        });


                    if (cost > maxCost)
                        context.Error(
                            "MAX_COST",
                            $"Query cost '{cost}' is too high. Max allowed: '{maxCost}'",
                            node);
                };
            };
        }
    }
}