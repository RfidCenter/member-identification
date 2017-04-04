using DevExpress.Xpo;

namespace PrintMembers.DatabaseCode
{

    public partial class Persona
    {
        public Persona(Session session) : base(session) { }
        public override void AfterConstruction() { base.AfterConstruction(); }
    }

}
