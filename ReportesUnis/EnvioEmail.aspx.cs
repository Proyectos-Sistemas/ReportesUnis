using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.IO;

namespace ReportesUnis
{
    public partial class EnvioEmail : System.Web.UI.Page
    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void BtnConfirmar_Click(object sender, EventArgs e)
        {
            string htmlBody = LeerBodyEmail();
            string[] datos = LeerInfoEmail();

            //Creación de instancia de la aplicacion de outlook
            var outlook = new Outlook.Application();

            //Crear un objeto MailItem
            var mailItem = (Outlook.MailItem)outlook.CreateItem(Outlook.OlItemType.olMailItem);


            //Configuracion campos para envio del correo
            mailItem.Subject = datos[0]; //Asunto del correo
            //mailItem.Body = "Se ha detectado una nueva actualización";

            mailItem.HTMLBody = htmlBody;
            mailItem.To = datos[1];

            //Enviar coreo
            mailItem.Send();

            //liberar recursos utilizados
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(mailItem);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(outlook);

        }

        

        public string LeerBodyEmail()
        {
            string rutaCompleta = CurrentDirectory + "/Emails/bodyIngresoEstudiantes.txt";
            string line = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                line = file.ReadToEnd();
                file.Close();
            }
            return line;
        }
        public string[] LeerInfoEmail()
        {
            string rutaCompleta = CurrentDirectory + "/Emails/DatosIngresoEstudiantes.txt";
            string[] datos;
            string subjet = "";
            string to = "";
            using (StreamReader file = new StreamReader(rutaCompleta))
            {
                string linea1 = file.ReadLine();
                string linea2 = file.ReadLine();
                string linea3 = file.ReadLine();
                string linea4 = file.ReadLine();
                subjet = linea2;
                to = linea4;
                file.Close();

                // Corrección: Inicializa un nuevo array y asigna los valores
                datos = new string[] { subjet, to };
            }

            return datos;
        }


    }
}