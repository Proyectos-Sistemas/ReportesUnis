<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CargarFotografiaEmpleados.aspx.cs" Inherits="ReportesUnis.CargarFotografiaEmpleados" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center">CARGA DE FOTOGRAFÍAS EMPLEADOS</h2>
        <hr />
    </div>

    <script type="text/javascript">
        function validateFileSize() {
            var uploadControl = document.getElementById('<%= FileUpload1.ClientID %>');
            var SumaTamañoDeArchivos = 0;
            var fsize = 0;
            console.log(uploadControl.files.length);

            if (uploadControl.files.length > 0) {
                for (var i = 0; i <= uploadControl.files.length - 1; i++) {
                    fsize = uploadControl.files.item(i).size;
                    SumaTamañoDeArchivos = SumaTamañoDeArchivos + fsize;
                }

                console.log(SumaTamañoDeArchivos);
                if (SumaTamañoDeArchivos > 1073741824 /*1GB = 1073741824 Bytes*/) {
                    document.getElementById('dvMsg').style.display = "block";

                    var btn = document.getElementById("<%=btnUpload.ClientID%>");
                    btn.disabled = true;
                    return false;
                }
                else {
                    document.getElementById('dvMsg').style.display = "none";
                    var btn = document.getElementById("<%=btnUpload.ClientID%>");
                    btn.disabled = false;
                    return true;
                }
            }
        }
    </script>

    <asp:TextBox ID="TxtURL" runat="server" Visible="false"></asp:TextBox>
    <div class="container"2>
        <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="True" accept="image/jpeg" onchange="validateFileSize();" />
        <div id="dvMsg" style="background-color: Red; color: White; width: 190px; padding: 3px; display: none;">
            El tamaño máximo permitido es de 1 GB
        </div>
        <asp:Button ID="btnUpload" runat="server" Text="Cargar" OnClick="Upload" CssClass="btn-primary" Enabled="false" />
        <hr />
        <asp:Label ID="lblMensaje" runat="server" Font-Bold="true" ForeColor="Black" Text="" Font-Size="Large"></asp:Label>
        <asp:GridView ID="GridView1" class="table table-bordered table-condensed table-responsive table-hover" runat="server"
            AutoGenerateColumns="false" CssClass="table">
            <Columns>
                <asp:BoundField DataField="File Name" HeaderText="File Name" />
                <asp:BoundField DataField="ID" HeaderText="ID" />
            </Columns>
        </asp:GridView>
    </div>
    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>
</asp:Content>
