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
    
    public partial class LeaguesRulesMaster
    {
        public long RecordID { get; set; }
        public Nullable<long> LeagueID { get; set; }
        public string Rule { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    
        public virtual LeaguesMaster LeaguesMaster { get; set; }
    }
}
