//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cricket.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Knockouts
    {
        public long RecordID { get; set; }
        public Nullable<int> Number { get; set; }
        public Nullable<long> LeagueID { get; set; }
        public Nullable<long> Player1ID { get; set; }
        public Nullable<long> Player2ID { get; set; }
        public string Stage { get; set; }
        public Nullable<long> WinnerID { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual PlayersMaster PlayersMaster { get; set; }
        public virtual PlayersMaster PlayersMaster1 { get; set; }
        public virtual PlayersMaster PlayersMaster2 { get; set; }
        public virtual LeaguesMaster LeaguesMaster { get; set; }
    }
}
