using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using SnapObjects.Data;
using DWNet.Data;
using Newtonsoft.Json;
using System.Collections;

namespace PSRESTAuthentication
{
    [DataWindow("dw_address", DwStyle.Grid)]
    [Table("address", Schema = "person")]
    #region DwSelectAttribute  
    [DwSelect("PBSELECT( VERSION(400) TABLE(NAME=\"person.address\" ) COLUMN(NAME=\"person.address.addressid\") COLUMN(NAME=\"person.address.addressline1\") COLUMN(NAME=\"person.address.city\") COLUMN(NAME=\"person.address.stateprovinceid\") COLUMN(NAME=\"person.address.postalcode\") COLUMN(NAME=\"person.address.modifieddate\")WHERE(    EXP1 =\"\\\"person\\\".\\\"address\\\".\\\"addressid\\\"\"   OP =\">\"    EXP2 =\":id\" ) ) ORDER(NAME=\"person.address.addressid\" ASC=yes ) ARG(NAME = \"id\" TYPE = number)")]
    #endregion
    [DwParameter("id", typeof(double?))]
    [UpdateWhereStrategy(UpdateWhereStrategy.KeyColumns)]
    [DwKeyModificationStrategy(UpdateSqlStrategy.Update)]
    public class Dw_Address
    {
        [Key]
        [Identity]
        [PropertySave(SaveStrategy.Ignore)]
        [SqlDefaultValue("autoincrement")]
        [DwColumn("addressid")]
        public int? Addressid { get; set; }

        [DwColumn("addressline1")]
        public string Addressline1 { get; set; }

        [DwColumn("city")]
        public string City { get; set; }

        [DwColumn("stateprovinceid")]
        public int? Stateprovinceid { get; set; }

        [DwColumn("postalcode")]
        public string Postalcode { get; set; }

        [PropertySave(SaveStrategy.Ignore)]
        [SqlDefaultValue("\"getdate\"()")]
        [DwColumn("modifieddate", TypeName = "timestamp")]
        public DateTime? Modifieddate { get; set; }

    }

}