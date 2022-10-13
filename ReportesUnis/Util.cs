using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReportesUnis
{
    public class Util
    {
        public static string LeerArchivoRecursos(int clave, string ruta)
        {
            int counter = 0;
            string line;
            string respuesta = "";
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(ruta);
            while ((line = file.ReadLine()) != null)
            {
                if (counter == clave)
                    respuesta = line;
                counter++;
            }

            file.Close();

            // Suspend the screen.
            return respuesta;
        }
    }
}