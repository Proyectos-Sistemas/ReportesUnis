﻿using Microsoft.Owin.Security;
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
                    MenuLogServipagos.Visible = MenuLogInterfaz.Visible = MenuLogInterfazHCMCS.Visible = MenuHistoricosHCM.Visible = MenuHistoricosCXC.Visible = MenuHistoricosCXP.Visible = MenuHistoricosGL.Visible = true;
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

                }

                //if (respuesta.Contains("RLI_VistaAlumnos"))
                //{
                //    ActEstudiantes.Visible = true;
                //}
                //else
                //{
                //    ActEstudiantes.Visible = respuesta.Contains(ActEstudiantes.ValidationGroup);
                //}
                
                //if (respuesta.Contains("RLI_VistaEmpleados"))
                //{
                //    ActEmpleados.Visible = true;
                //}
                //else
                //{
                //    ActEmpleados.Visible = respuesta.Contains(ActEmpleados.ValidationGroup);
                //}

                if (respuesta.Contains("DATOS_FOTOGRAFIAS"))
                {
                    RepEstudiantes.Visible = RepEmpleados.Visible = RepCamarasEst.Visible = RepCamarasEmp.Visible = CargaCTEst.Visible = CargaCTEmp.Visible = MantPantallas.Visible = GestionesEstudiantes.Visible =  true;
                }
                else
                {
                    RepEstudiantes.Visible = respuesta.Contains(RepEstudiantes.ValidationGroup);
                    RepEmpleados.Visible = respuesta.Contains(RepEmpleados.ValidationGroup);
                    RepCamarasEst.Visible = respuesta.Contains(RepCamarasEst.ValidationGroup);
                    RepCamarasEmp.Visible = respuesta.Contains(RepCamarasEmp.ValidationGroup);
                    CargaCTEst.Visible = respuesta.Contains(CargaCTEst.ValidationGroup);
                    CargaCTEmp.Visible = respuesta.Contains(CargaCTEmp.ValidationGroup);
                    MantPantallas.Visible = respuesta.Contains(MantPantallas.ValidationGroup);
                    GestionesEstudiantes.Visible = respuesta.Contains(GestionesEstudiantes.ValidationGroup);
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
            string callbackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Response.ApplyAppPathModifier("~/Account/SignOut");

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