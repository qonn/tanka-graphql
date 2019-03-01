namespace tanka.graphql.type.validation
{
    public delegate void SchemaNodeVisitor<in T>(T node);
}