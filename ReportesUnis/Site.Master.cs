using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReportesUnis
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int error;
                LoginAD.Conexion conex = new LoginAD.Conexion(@"LDAP://unis.local/dc=unis,dc=local");
                List<string> respuesta = conex.BuscarGrupos(Global._struser, Global._strdPass, Context.User.Identity.Name.Replace("@unis.edu.gt", ""), out error);
                Session["Grupos"] = respuesta;

                if (respuesta.Contains("RLI_Admin"))
                {
                    MenuLogServipagos.Visible = MenuLogInterfaz.Visible = MenuLogInterfazHCMCS.Visible = MenuHistoricosHCM.Visible = MenuHistoricosCXC.Visible = MenuHistoricosCXP.Visible = MenuHistoricosGL.Visible = MenuDirectorio.Visible = true;
                }
                else
                {
                    MenuLogServipagos.Visible = respuesta.Contains(MenuLogServipagos.ValidationGroup);
                    MenuLogInterfaz.Visible = respuesta.Contains(MenuLogInterfaz.ValidationGroup);
                    MenuLogInterfazHCMCS.Visible = respuesta.Contains(MenuLogInterfazHCMCS.ValidationGroup);
                    MenuHistoricosHCM.Visible = respuesta.Contains(MenuHistoricosHCM.ValidationGroup);
                    MenuHistoricosCXC.Visible = respuesta.Contains(MenuHistoricosCXC.ValidationGroup);
                    MenuHistoricosCXP.Visible = respuesta.Contains(MenuHistoricosCXP.ValidationGroup);
                    MenuHistoricosGL.Visible = respuesta.Contains(MenuHistoricosGL.ValidationGroup);
                    MenuDirectorio.Visible = respuesta.Contains(MenuDirectorio.ValidationGroup);

                }

                if (respuesta.Contains("DATOS_FOTOGRAFIAS"))
                {
                    //RepCamarasEst.Visible = RepCamarasEmp.Visible = true;
                    RepCamarasEst.Visible = true;
                }
                else
                {
                    RepCamarasEst.Visible = respuesta.Contains(RepCamarasEst.ValidationGroup);
                    //RepCamarasEmp.Visible = respuesta.Contains(RepCamarasEmp.ValidationGroup); DESCOMENTAR EN LA FASE 2
                }
                if (respuesta.Contains("RLI_VistaAdmisiones"))
                {
                    ActualizacionGeneralEstudiantes.Visible = true;
                }
                else
                {
                    ActualizacionGeneralEstudiantes.Visible = respuesta.Contains(ActualizacionGeneralEstudiantes.ValidationGroup);
                }

                if (respuesta.Contains("ACCESO_CARNETIZACION"))
                {
                    MantPantallas.Visible = GestionesEstudiantes.Visible = Confirmacion.Visible = GestionesEmpleados.Visible = ConfirmacionEmpleados.Visible = true;
                }
                else
                {
                    MantPantallas.Visible = respuesta.Contains(MantPantallas.ValidationGroup);
                    GestionesEstudiantes.Visible = respuesta.Contains(GestionesEstudiantes.ValidationGroup);
                    GestionesEmpleados.Visible = respuesta.Contains(GestionesEmpleados.ValidationGroup);
                    Confirmacion.Visible = respuesta.Contains(Confirmacion.ValidationGroup);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            // Redireccionar a ~/Account/SignOut después de cerrar sesión.
            string callbackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Response.ApplyAppPathModifier("~/Default");

            HttpContext.Current.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);
        }

        protected void Unnamed_Click(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }


    }
}