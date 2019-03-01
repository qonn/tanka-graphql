using tanka.graphql.sdl;
using tanka.graphql.type;
using tanka.graphql.type.validation;
using Xunit;

namespace tanka.graphql.tests.type.validation
{
    public class ValidatorFacts
    {
        private SchemaValidationResult Validate(ISchema schema, CombineSchemaRule rule)
        {
            var walker = new SchemaRulesWalker(schema, new[] {rule});
            return walker.Validate();
        }

        [Fact]
        public void ObjectType_validation_1()
        {
            /* Given */
            const string idl = @"
                extend schema {
                    field1: String
                }
            ";

            /* When */
            var schema = Sdl.Schema(Parser.ParseDocument(idl));
            var result = Validate(schema, TypeSystemRules.R322SchemaExtension());

            /* Then */
            Assert.False(result.IsValid);
        }
    }
}