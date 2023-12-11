using eid.saml20.Schema.Core;

namespace eid.saml20.Validation
{
    internal interface ISaml20StatementValidator
    {
        void ValidateStatement(StatementAbstract statement);
    }
}