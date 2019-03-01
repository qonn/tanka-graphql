namespace tanka.graphql.type.validation
{
    public class SchemaVisitor
    {
        public SchemaNodeVisitor<ISchema> EnterSchema { get; } = node => { };

        public SchemaNodeVisitor<ISchema> LeaveSchema { get; } = node => { };

        public SchemaNodeVisitor<ObjectType> EnterQuery { get; } = node => { };

        public SchemaNodeVisitor<ObjectType> LeaveQuery { get; } = node => { };

        public SchemaNodeVisitor<ObjectType> EnterMutation { get; } = node => { };

        public SchemaNodeVisitor<ObjectType> LeaveMutation { get; } = node => { };

        public SchemaNodeVisitor<ObjectType> EnterSubscription { get; } = node => { };

        public SchemaNodeVisitor<ObjectType> LeaveSubscription { get; } = node => { };

        public SchemaNodeVisitor<ObjectType> EnterObject { get; } = node => { };

        public SchemaNodeVisitor<ObjectType> LeaveObject { get; } = node => { };

        public SchemaNodeVisitor<InterfaceType> EnterInterface { get; } = node => { };

        public SchemaNodeVisitor<InterfaceType> LeaveInterface { get; } = node => { };

        public SchemaNodeVisitor<(string Name, IField Field)> EnterField { get; } = node => { };

        public SchemaNodeVisitor<(string Name, IField Field)> LeaveField { get; } = node => { };

        public SchemaNodeVisitor<DirectiveType> EnterDirectiveType { get; } = node => { };

        public SchemaNodeVisitor<DirectiveType> LeaveDirectiveType { get; } = node => { };

        public SchemaNodeVisitor<DirectiveInstance> EnterDirective { get; } = node => { };

        public SchemaNodeVisitor<DirectiveInstance> LeaveDirective { get; } = node => { };

        public SchemaNodeVisitor<(string Name, Argument argument)> EnterArgument { get; } = node => { };

        public SchemaNodeVisitor<(string Name, Argument argument)> LeaveArgument { get; } = node => { };

        public SchemaNodeVisitor<List> EnterList { get; } = node => { };

        public SchemaNodeVisitor<List> LeaveList { get; } = node => { };

        public SchemaNodeVisitor<NonNull> EnterNonNull { get; } = node => { };

        public SchemaNodeVisitor<NonNull> LeaveNonNull { get; } = node => { };

        public SchemaNodeVisitor<UnionType> EnterUnion { get; } = node => { };

        public SchemaNodeVisitor<UnionType> LeaveUnion { get; } = node => { };

        public SchemaNodeVisitor<ScalarType> EnterScalar { get; } = node => { };

        public SchemaNodeVisitor<ScalarType> LeaveScalar { get; } = node => { };

        public SchemaNodeVisitor<EnumType> EnterEnum { get; } = node => { };

        public SchemaNodeVisitor<EnumType> LeaveEnum { get; } = node => { };

        public SchemaNodeVisitor<(string VALUE, Meta Meta)> EnterEnumValue { get; } = node => { };

        public SchemaNodeVisitor<(string VALUE, Meta Meta)> LeaveEnumValue { get; } = node => { };

        public SchemaNodeVisitor<InputObjectType> EnterInputObject { get; } = node => { };

        public SchemaNodeVisitor<InputObjectType> LeaveInputObject { get; } = node => { };

        public SchemaNodeVisitor<(string Name, InputObjectField Field)> EnterInputObjectField { get; } =
            node => { };

        public SchemaNodeVisitor<(string Name, InputObjectField Field)> LeaveInputObjectField { get; } =
            node => { };
    }
}