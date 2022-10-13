using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportesUnis.Models
{
    public class Modelos
    {


    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Item
    {
        public string clave_documento_ident { get; set; }
        public string documento_identidad { get; set; }
        public string nombres { get; set; }
        public string apellido_paterno { get; set; }
        public string fecha_efectiva { get; set; }
        public DateTime fecha_registro { get; set; }
        public string direc_correo_electronico { get; set; }
        public string ciclo_admision { get; set; }
        public string descripcion_estatus { get; set; }
        public string grado_academico { get; set; }
        public string id_party_id { get; set; }
    }

    public class Contactos
    {
        public List<Item> items { get; set; }
    }
    public class ItemEmpleado
    {
        public string tipoproceso { get; set; }
        public string estadoproceso { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string personid { get; set; }
        public string nationalidentifiernumber { get; set; }
        public string estatus { get; set; }
        public string descripcion_estatus { get; set; }
        public DateTime fecha_creacion { get; set; }
        public DateTime fecha_ultima_actualizacion { get; set; }

    }

    public class Empleados
    {
        public List<ItemEmpleado> Items { get; set; }
    }


    public class SerbiPagos
    {
        public List<ItemSerbiPagos> Items { get; set; }
    }

    public class ItemSerbiPagos
    {
        public string dtfecharegistro{ get; set; }
        public string tiposolicitud { get; set; }
        public string txtxmlrequest { get; set; }
        public string txtxmlresponseerror { get; set; }
    }

    public class ItemEmpleadoH
    {
        public string Personnumber { get; set; }
        public DateTime Effectivestartdate { get; set; }
        public string Lastname { get; set; }
        public string Previouslastname { get; set; }
        public string Firstname { get; set; }
        public string Middlenames { get; set; }
        public string Nameinformation1 { get; set; }
        public string Nationalidentifiertype { get; set; }
        public string Nationalidentifiernumber { get; set; }
        public string Dateofbirth { get; set; }
        public string Maritalstatus { get; set; }
        public string Sex { get; set; }
        public string Ethnicity { get; set; }
        public string Religion { get; set; }
        public string Bloodtype { get; set; }
        public string Townofbirth { get; set; }
        public string Countryofbirth { get; set; }
        public string Regionofbirth { get; set; }
        public string Legislationcode { get; set; }
        public string Highesteducationlevel { get; set; }
        public string Addressline1 { get; set; }
        public string Addladdressattribute3 { get; set; }
        public string Postalcode { get; set; }
        public string Townorcity { get; set; }
        public string Country { get; set; }
        public string Addresstype { get; set; }
        public string Emailaddress { get; set; }
        public string Emailtype { get; set; }
        public string Phonenumber { get; set; }
        public string Phonetype { get; set; }
        public string Workertype { get; set; }
        public string Actioncode { get; set; }
        public DateTime Effectiveenddate { get; set; }
        public string Assignmentcategory { get; set; }
        public string Workercategory { get; set; }
        public string Hourlysalariedcode { get; set; }
        public string Hourlysalariedname { get; set; }
        public string Gradecode { get; set; }
        public string Gradename { get; set; }
        public string Positioncode { get; set; }
        public string Positionname { get; set; }
        public string Jobcode { get; set; }
        public string Jobname { get; set; }
        public string Locationcode { get; set; }
        public string Departmentname { get; set; }
        public string Frequency { get; set; }
        public string Normalhours { get; set; }
        public string Fullparttime { get; set; }
        public string Persontypecode { get; set; }
        public string Bankname { get; set; }
        public string Accountnumber { get; set; }
        public string Accounttype { get; set; }
        public string Salaryamount { get; set; }
        public string Externalidentifiernumber { get; set; }
        public string Externalidentifiertype { get; set; }
        public string Assignmentnumber { get; set; }
        public string Emailaddress1 { get; set; }
        public string Emailaddress2 { get; set; }
        public string Phonenumber1 { get; set; }
        public string Phonenumber2 { get; set; }
        public string Phonenumber3 { get; set; }
        public string Phonenumber4 { get; set; }
        public string Phonenumber5 { get; set; }
        public string Emailtype1 { get; set; }
        public string Emailtype2 { get; set; }
        public string Phonetype1 { get; set; }
        public string Phonetype2 { get; set; }
        public string Phonetype3 { get; set; }
        public string Phonetype4 { get; set; }
        public string Phonetype5 { get; set; }
        public string IdRegistro { get; set; }

    }

    public class EmpleadosH
    {
        public List<ItemEmpleadoH> Items { get; set; }
    }

    public class CuentasXCobrar
    {
        public List<ItemCXC> Items { get; set; }
    }

    public class ItemCXC
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Unidadnegocio_code { get; set; }
        public string Name_un { get; set; }
        public string Codecliente { get; set; }
        public string Nit { get; set; }
        public string Razonsocial { get; set; }
        public string Fecha { get; set; }
        public string Codtypedocument { get; set; }
        public string Documenttype { get; set; }
        public string Numdocument { get; set; }
        public string Diascredito { get; set; }
        public string Valordocument { get; set; }
        public string Saldo { get; set; }
    }

    public class CuentasXPagar
    {
        public List<ItemCXP> Items { get; set; }
    }

    public class ItemCXP
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Code_un { get; set; }
        public string Name_un { get; set; }
        public string Codeproveedor { get; set; }
        public string Nit { get; set; }
        public string Razonsocial { get; set; }
        public string Fecha { get; set; }
        public string Codtypedocument { get; set; }
        public string Typedocument { get; set; }
        public string Numdocument { get; set; }
        public string Diascredito { get; set; }
        public string Descripcion { get; set; }
        public string Valordocument { get; set; }
        public string Saldo { get; set; }
    }

    public class ContabilidadGeneral
    {
        public List<ItemContabilidadGeneral> Items { get; set; }
    }

    public class ItemContabilidadGeneral
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Fecha { get; set; }
        public string Unidadnegocio_code { get; set; }
        public string Name_un { get; set; }
        public string Centrointegracion_code { get; set; }
        public string Name_ci { get; set; }
        public string Cuentacontable_code { get; set; }
        public string Name_cc { get; set; }
        public string Debe { get; set; }
        public string Haber { get; set; }
        public string Descripcion { get; set; }
        public string Periodo { get; set; }

    }
}