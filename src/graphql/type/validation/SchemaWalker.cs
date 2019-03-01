using System;
using System.Collections.Generic;

namespace tanka.graphql.type.validation
{
    public class SchemaRulesWalker
    {
        public SchemaRulesWalker(ISchema schema, CombineSchemaRule[] rules)
        {
            Schema = schema;
            Context = new SchemaValidationContext(Schema);
            CombineRules(Context, rules);
        }

        public ISchema Schema { get; }

        public SchemaValidationContext Context { get; set; }

        public SchemaVisitor CombinedRule { get; set; }

        public SchemaValidationResult Validate()
        {
            VisitSchema(Schema);
            return BuildResult();
        }

        private SchemaValidationResult BuildResult()
        {
            var errors = Context.Errors;

            return new SchemaValidationResult(errors);
        }

        private void VisitSchema(ISchema schema)
        {
            CombinedRule.EnterSchema(schema);

            // directive types
            VisitDirectiveTypes(schema.QueryDirectives());

            // query
            CombinedRule.EnterQuery(Schema.Query);
            VisitObject(Schema.Query);
            CombinedRule.LeaveQuery(schema.Query);

            // mutation (optional)
            CombinedRule.EnterMutation(Schema.Mutation);
            if (Schema.Mutation != null) VisitObject(Schema.Mutation);
            CombinedRule.LeaveMutation(schema.Mutation);

            // subscription (optional)
            CombinedRule.EnterSubscription(Schema.Subscription);
            if (Schema.Subscription != null) VisitObject(Schema.Subscription);
            CombinedRule.LeaveSubscription(schema.Subscription);

            CombinedRule.LeaveSchema(schema);
        }

        private void VisitDirectiveTypes(IEnumerable<DirectiveType> directiveTypes)
        {
            foreach (var directiveType in directiveTypes) VisitDirectiveType(directiveType);
        }

        private void VisitDirectiveType(DirectiveType directiveType)
        {
            CombinedRule.EnterDirectiveType(directiveType);
            CombinedRule.LeaveDirectiveType(directiveType);
        }

        private void VisitObject(ObjectType objectType)
        {
            CombinedRule.EnterObject(objectType);
            VisitFields(objectType);
            CombinedRule.LeaveObject(objectType);
        }

        private void VisitFields(ComplexType complexType)
        {
            var fields = Schema.GetFields(complexType.Name);

            foreach (var field in fields) VisitField(field);
        }

        private void VisitField(KeyValuePair<string, IField> field)
        {
            var fieldDef = (field.Key, field.Value);
            CombinedRule.EnterField(fieldDef);

            VisitDirectives(fieldDef.Value.Directives);
            VisitArguments(fieldDef.Value.Arguments);
            VisitType(fieldDef.Value.Type);

            CombinedRule.LeaveField(fieldDef);
        }

        private void VisitArguments(IEnumerable<KeyValuePair<string, Argument>> arguments)
        {
            foreach (var argument in arguments) VisitArgument(argument);
        }

        private void VisitArgument(KeyValuePair<string, Argument> argument)
        {
            var argumentDef = (argument.Key, argument.Value);
            CombinedRule.EnterArgument(argumentDef);

            VisitType(argumentDef.Value.Type);

            CombinedRule.LeaveArgument(argumentDef);
        }

        private void VisitType(IType type)
        {
            switch (type)
            {
                case NonNull nonNull:
                    VisitNonNull(nonNull);
                    break;
                case List list:
                    VisitList(list);
                    break;
                case ObjectType objectType:
                    VisitObject(objectType);
                    break;
                case InterfaceType interfaceType:
                    VisitInterface(interfaceType);
                    break;
                case UnionType unionType:
                    VisitUnion(unionType);
                    break;
                case ScalarType scalarType:
                    VisitScalar(scalarType);
                    break;
                case EnumType enumType:
                    VisitEnum(enumType);
                    break;
                case InputObjectType inputObject:
                    VisitInputObject(inputObject);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown type {type}");
            }
        }

        private void VisitInputObject(InputObjectType inputObject)
        {
            CombinedRule.EnterInputObject(inputObject);

            VisitInputFields(inputObject);

            CombinedRule.LeaveInputObject(inputObject);
        }

        private void VisitInputFields(InputObjectType inputObject)
        {
            foreach (var inputField in Schema.GetInputFields(inputObject.Name)) VisitInputField(inputField);
        }

        private void VisitInputField(KeyValuePair<string, InputObjectField> inputField)
        {
            var inputFieldDef = (inputField.Key, inputField.Value);
            CombinedRule.EnterInputObjectField(inputFieldDef);

            VisitDirectives(inputFieldDef.Value.Directives);
            VisitType(inputFieldDef.Value.Type);

            CombinedRule.LeaveInputObjectField(inputFieldDef);
        }

        private void VisitEnum(EnumType enumType)
        {
            CombinedRule.EnterEnum(enumType);

            foreach (var enumTypeValue in enumType.Values) VisitEnumValue(enumTypeValue);

            CombinedRule.LeaveEnum(enumType);
        }

        private void VisitEnumValue(KeyValuePair<string, Meta> enumTypeValue)
        {
            var enumValueDef = (enumTypeValue.Key, enumTypeValue.Value);
            CombinedRule.EnterEnumValue(enumValueDef);
            CombinedRule.LeaveEnumValue(enumValueDef);
        }

        private void VisitScalar(ScalarType scalarType)
        {
            CombinedRule.EnterScalar(scalarType);
            CombinedRule.LeaveScalar(scalarType);
        }

        private void VisitUnion(UnionType unionType)
        {
            CombinedRule.EnterUnion(unionType);

            foreach (var possibleType in Schema.GetPossibleTypes(unionType)) VisitObject(possibleType);

            CombinedRule.LeaveUnion(unionType);
        }

        private void VisitInterface(InterfaceType interfaceType)
        {
            CombinedRule.EnterInterface(interfaceType);

            VisitFields(interfaceType);

            CombinedRule.LeaveInterface(interfaceType);
        }

        private void VisitList(List list)
        {
            CombinedRule.EnterList(list);

            VisitType(list.WrappedType);

            CombinedRule.LeaveList(list);
        }

        private void VisitNonNull(NonNull nonNull)
        {
            CombinedRule.EnterNonNull(nonNull);

            VisitType(nonNull.WrappedType);

            CombinedRule.LeaveNonNull(nonNull);
        }

        private void VisitDirectives(IEnumerable<DirectiveInstance> directives)
        {
            foreach (var directiveInstance in directives) VisitDirective(directiveInstance);
        }

        private void VisitDirective(DirectiveInstance directiveInstance)
        {
            CombinedRule.EnterDirective(directiveInstance);

            VisitArguments(directiveInstance.Arguments);

            CombinedRule.LeaveDirective(directiveInstance);
        }

        private void CombineRules(SchemaValidationContext context, IEnumerable<CombineSchemaRule> rules)
        {
            CombinedRule = new SchemaVisitor();
            foreach (var combineRule in rules) combineRule(context, CombinedRule);
        }
    }
}