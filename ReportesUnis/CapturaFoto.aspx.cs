using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Windows.Media.Capture;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.Foundation;
using System.IO;
using System.Web.Services;
using System.Web.Script.Services;
using Microsoft.Ajax.Utilities;

namespace ReportesUnis
{
    public partial class CapturaFoto : System.Web.UI.Page

    {
        string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //urlPath.Text = "PAGE LOAD";
        }
        public string SaveCanvasImage(string imageData)
        {
            int largo = 0;
            largo = imageData.Length;
            imageData =imageData.Substring(23, largo-23);
            try
            {
                // Nombre del archivo de imagen
                string fileName = "canvas_image.jpg";

                // Ruta de la carpeta donde se almacenará la imagen
                string folderPath = HttpContext.Current.Server.MapPath("~/DPIUsuarios/");

                // Ruta completa del archivo
                string filePath = Path.Combine(folderPath, fileName);

                // Guardar la imagen en el servidor
                byte[] imageBytes = Convert.FromBase64String(Convert.ToString(imageData));
                File.WriteAllBytes(filePath, imageBytes);

                return "Imagen guardada correctamente.";
            }
            catch (Exception ex)
            {
                return "Error al guardar la imagen: " + ex.Message;
            }
        }

        protected void BtnAlmacenar_Click(object sender, EventArgs e)
        {            
            // var valor = Request.Form["urlPath"];
            SaveCanvasImage(Request.Form["urlPath"]);
            //texto.Text = valor;
        }
    }
}