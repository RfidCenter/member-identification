using DevExpress.Xpo;

namespace MemberIdentification.DatabaseCode
{

    public partial class Persona
    {
        public Persona(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
